using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.Rendering;

public class PlatformMoving : MonoBehaviour
{
    private readonly int PIC_CAPACITY = 5; // picturestatus code의 piccapicty와 같아야 함. 수동으로 수정 필요 

    [SerializeField] public GameObject currentPicture; // 현재 속한 사진의 오브젝트. 부모 관계의 사진으로 set 되어야 함. 
    private PictureStatus curPicStatusCode; // 현재 속한 사진의 코드 
    public GameObject[] overLappedPicture; // 현재 속한 사진과 겹친 사진 

    // 플랫폼 이동 시 사용하는 변수 
    [SerializeField] public int directionChoose = 0;
    [SerializeField] private float horizonSpeed = 5f;
    private int direction = 1;

    private Rigidbody2D thisRigid;
    private Rigidbody2D curPicRigid;
    private Rigidbody2D[] otherPicsRigid;
   


    // Start is called before the first frame update
    void Start()
    { 

        currentPicture = GetComponentInParent<PictureStatus>().gameObject;
        curPicStatusCode = currentPicture.GetComponent<PictureStatus>();
        overLappedPicture = new GameObject[PIC_CAPACITY];

        thisRigid = GetComponent<Rigidbody2D>();
        curPicRigid = currentPicture.GetComponent<Rigidbody2D>();
        otherPicsRigid = new Rigidbody2D[PIC_CAPACITY];
    }

    void Update()
    {
        for(int i= 0; i < PIC_CAPACITY; i++) 
            overLappedPicture[i] = curPicStatusCode.otherPics[i];

        //Crossing_Regulation();
        MovingFunction(directionChoose);

    }


    void MovingFunction(int distinguisher)
    {
        bool[] isInsideArea = new bool[PIC_CAPACITY]; // 겹친 사진의 영역에 포함되는지 판별하는 bool
        var pictureAxis = new(float picCenter, float picHalfWidth)[PIC_CAPACITY];

        Transform curPicTrans = currentPicture.transform;
  
        float scaleX = transform.localScale.x * curPicTrans.localScale.x;
        float scaleY = transform.localScale.y * curPicTrans.localScale.y;

        for(int i= 0;i < PIC_CAPACITY;i++) 
        {
            if (overLappedPicture[i] != null) // 겹친 사진이 있는 경우.
            {
                //overLappedPicture = currentPicStatus.otherPic; 
                switch (distinguisher)
                {
                    case 0: // 가로 이동 case
                        isInsideArea[i]= ((transform.position.y + scaleY / 2) <= overLappedPicture[i].transform.position.y + overLappedPicture[i].transform.localScale.y / 2)
                                    && ((transform.position.y - scaleY / 2) >= overLappedPicture[i].transform.position.y - overLappedPicture[i].transform.localScale.y / 2)
                                    ? true : false; // 가로 이동의 경우 겹친 사진의 세로 영역에 포함되는지 판단.
                        break;
                    case 1: // 세로 이동 case
                        isInsideArea[i] = ((transform.position.x + scaleX / 2) <= overLappedPicture[i].transform.position.x + overLappedPicture[i].transform.localScale.x / 2)
                                    && ((transform.position.x - scaleX / 2) >= overLappedPicture[i].transform.position.x - overLappedPicture[i].transform.localScale.x / 2)
                                    ? true : false; // 세로 이동의 경우 겹친 사진의 가로 영역에 포함되는지 판단. 
                        break;
                }
            }

            if (overLappedPicture[i] == null || !isInsideArea[i]) // 겹치지 않는 경우 또는 위에서 판별했을 때 발판이 겹친 사진의 영역 안에 들어가지 않는지 판단. 사진이 겹쳤어도 영역 안에 없으면 이것 실행됨
            {
                switch (distinguisher)
                {
                    case 0:
                        pictureAxis[i] = (curPicTrans.position.x, curPicTrans.localScale.x * 0.5f); // 겹친 사진끼리의 중심 좌표 , 원래 있던 사진의 가로길이 절반
                        break;
                    case 1:
                        pictureAxis[i] = (curPicTrans.position.y, curPicTrans.localScale.y * 0.5f); 
                        break;
                }
            }
            else if (overLappedPicture[i] != null && isInsideArea[i]) // 사진이 겹치는 경우와 발판이 겹친 사진의 영역 안에 들어가는 조건 다 만족해야 함.
            {
                switch (distinguisher)
                {
                    case 0:
                        pictureAxis[i] = ((curPicTrans.position.x + overLappedPicture[i].transform.position.x) / 2,
                                          (curPicStatusCode.Calculate_FullLengthX()[i] - curPicStatusCode.Calculate_OverlapAreaX()[i]) / 2);
                        print(pictureAxis[i].picCenter +" " + pictureAxis[i].picHalfWidth);
                        break;
                    case 1:
                        pictureAxis[i] = ((curPicTrans.position.y + overLappedPicture[i].transform.position.y) / 2,
                                         (curPicStatusCode.Calculate_FullLengthY()[i] - curPicStatusCode.Calculate_OverlapAreaY()[i]) / 2);
                        break;

                }
            }

            bool isoverArea = false;
            switch (distinguisher) // 사진과 부딪쳤을 때 방향 바꿔주는 구문 
            {
                case 0:
                    if (((transform.position.x + scaleX * 0.5f) >= (pictureAxis[i].picCenter + pictureAxis[i].picHalfWidth)
                        || (transform.position.x - scaleX * 0.5f) <= (pictureAxis[i].picCenter - pictureAxis[i].picHalfWidth)) && isoverArea)
                    {
                        direction *= -1;
                        isoverArea = true;
                    }

                    thisRigid.velocity = new Vector2(direction * horizonSpeed, 0);
                    break;

                case 1:
                    if ((transform.position.y + scaleY * 0.5f) >= (pictureAxis[i].picCenter + pictureAxis[i].picHalfWidth)
                        ||(transform.position.y - scaleY * 0.5f) <= (pictureAxis[i].picCenter - pictureAxis[i].picHalfWidth))
                        direction *= -1;

                    thisRigid.velocity = new Vector2(0, direction * horizonSpeed);
                    break;
            }
        }



        // if ((transform.position.x + transform.localScale.x * 0.5) <= (pictureX + pictureHalfWidth))
    }
    
    void RealMovingFunction()
    {       
        // 사진 하나만 있을 때 
        // currentPicrure의 좌표 판정해서 발판 충돌


        // 사진 n개 이상 겹칠 때 
        // isinsideArea 판정 후, true에 해당하는 사진에 한해서만 엣지 콜라이더 좌표 설정 

        // 각각의 사진에 대해서, transform.position 비교 후 

        bool[] isInsideArea = new bool[PIC_CAPACITY];
        Transform curPicTrans = currentPicture.transform;

        float scaleX = transform.localScale.x * curPicTrans.localScale.x;
        float scaleY = transform.localScale.y * curPicTrans.localScale.y;

        for(int i=0; i<PIC_CAPACITY; i++) 
        {
            if (overLappedPicture[i] != null)
            {

            }
        }
    }
    void Crossing_Regulation() // 두 사진의 경계 사이에서 발판이 지나가는 경우에 대한 예외처리. 사진을 못 움직이게 한다.
    {

        for(int i=0; i<PIC_CAPACITY; i++)
        {
            if (overLappedPicture[i] != null)
            {
                otherPicsRigid[i] = overLappedPicture[i].GetComponent<Rigidbody2D>();

                if (!curPicStatusCode.Is_Plate_In_OtherPic(transform.gameObject)[i]) // 사진이 겹쳤지만, 겹친 사진 영역에 다 들어가진 않은 경우
                {
                    curPicRigid.velocity = Vector2.zero;
                    otherPicsRigid[i].velocity = Vector2.zero;
                }
            }
          
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
  
        if(collision.transform.tag == "Platform") // 이거 왜 안됨 ★
        {
            direction = -direction;
        }
    }

}
