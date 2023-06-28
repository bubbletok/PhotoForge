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
using UnityEngine.UIElements;

public class PlatformMoving : MonoBehaviour
{
    private readonly int PIC_CAPACITY = 5; // picturestatus code의 piccapicty와 같아야 함. 수동으로 수정 필요 

    [SerializeField] public GameObject currentPicture; // 현재 속한 사진의 오브젝트. 부모 관계의 사진으로 set 되어야 함. 
    public PictureStatus curPicStatusCode; // 현재 속한 사진의 코드 
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

    bool isOverlap = false;
    void Update()
    {
        for(int i= 0; i < PIC_CAPACITY; i++) 
            overLappedPicture[i] = curPicStatusCode.otherPics[i];

       
        foreach (GameObject overlapPic in overLappedPicture)
        {
            if (overlapPic == null)
            {
                isOverlap = false;
                continue;
            }
            else // 겹친 사진이 있는 경우
            {
                isOverlap = true;
                break;
            }
        }

        if(isOverlap)
        {
            MovingFunction_Overlap(directionChoose);
        }
        else
        {
            MovingFunction_Alone(directionChoose);
        }

        //Crossing_Regulation();

    }

    void MovingFunction_Overlap(int distinguisher)
    {
        bool[] isInsideArea = new bool[PIC_CAPACITY]; // 겹친 사진의 영역에 포함되는지 판별하는 bool
        Transform curPicTrans = currentPicture.transform;

        (float realPicCenter, float realPicHalfWidth) realPicAxis = (0, 0);

        switch (distinguisher)
        {
            case 0:
                realPicAxis = (curPicTrans.position.x, curPicTrans.localScale.x); // halfwidth는 나중에 /2 해줘야 함.
                break;
            case 1:
                realPicAxis = (curPicTrans.position.y, curPicTrans.localScale.y);
                break;
        }
        float PicCount = 1; // 평균을 구하는 용도이기 때문에 초기값은 0이 아닌 1로 설정되어야 함 

        float scaleX = transform.localScale.x * curPicTrans.localScale.x;
        float scaleY = transform.localScale.y * curPicTrans.localScale.y;

        for(int i= 0; i < PIC_CAPACITY;i++) 
        {
            // 1. isInsideArea 값 도출
            if (overLappedPicture[i] != null) // 겹친 사진이 있는 경우.
            {
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

            // 2. isInsideArea 조건을 만족하는 사진에 대해 좌표값 도출 
            if (overLappedPicture[i] != null && isInsideArea[i]) // 사진이 겹치는 경우와 발판이 겹친 사진의 영역 안에 들어가는 조건 다 만족해야 함.
            {
                switch (distinguisher)
                {
                    case 0:
                        realPicAxis.realPicCenter += overLappedPicture[i].transform.position.x;
                        realPicAxis.realPicHalfWidth += overLappedPicture[i].transform.localScale.x - curPicStatusCode.Calculate_OverlapAreaX()[i];
                        ++PicCount;
                        break;
                    case 1:
                        realPicAxis.realPicCenter += overLappedPicture[i].transform.position.y;
                        realPicAxis.realPicHalfWidth += overLappedPicture[i].transform.localScale.y - curPicStatusCode.Calculate_OverlapAreaY()[i];
                        ++PicCount;
                        break;
                }
            }
        }

        realPicAxis.realPicCenter /= PicCount; // 좌표 평균 구하기
        realPicAxis.realPicHalfWidth /= 2; // 실제 발판 이동범위 구하기 

        switch (distinguisher) // 사진과 부딪쳤을 때 방향 바꿔주는 구문 
        {
            case 0:
                if ((transform.position.x + scaleX * 0.5f) >= (realPicAxis.realPicCenter + realPicAxis.realPicHalfWidth))
                {
                    direction = -1;
                }
                else if ((transform.position.x - scaleX * 0.5f) <= (realPicAxis.realPicCenter - realPicAxis.realPicHalfWidth))
                {
                    direction = 1;
                }
                thisRigid.velocity = new Vector2(direction * horizonSpeed, 0);
                break; // 각각에 접근할 것이 아니라, 조건을 만족하는 겹친 사진 '전체'의 평균을 구해야 한다. 

            case 1:
                if ((transform.position.y + scaleY * 0.5f) >= (realPicAxis.realPicCenter + realPicAxis.realPicHalfWidth))
                {
                    direction = -1;
                }
                else if ((transform.position.y - scaleY * 0.5f) <= (realPicAxis.realPicCenter - realPicAxis.realPicHalfWidth))
                {
                    direction = 1;
                }
                thisRigid.velocity = new Vector2(0, direction * horizonSpeed);
                break;
        }
  
    }

    void MovingFunction_Alone(int distinguisher)
    {
        Transform curPicTrans = currentPicture.transform;

        float scaleX = transform.localScale.x * curPicTrans.localScale.x;
        float scaleY = transform.localScale.y * curPicTrans.localScale.y;

        switch (distinguisher)
        {
            case 0:
                if ((transform.position.x + scaleX * 0.5f) >= (curPicTrans.position.x + curPicTrans.localScale.x * 0.5f))
                    direction = -1;
                else if ((transform.position.x - scaleX * 0.5f) <= (curPicTrans.position.x - curPicTrans.localScale.x * 0.5f))
                    direction = 1;

                thisRigid.velocity = new Vector2(direction * horizonSpeed, 0);
                break;

            case 1:
                if ((transform.position.y + scaleY * 0.5f) >= (curPicTrans.position.y + curPicTrans.localScale.y * 0.5f))
                    direction = -1;     
                else if ((transform.position.y - scaleY * 0.5f) <= (curPicTrans.position.y - curPicTrans.localScale.y * 0.5f))
                    direction = 1;

                thisRigid.velocity = new Vector2(0, direction * horizonSpeed);
                break;
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
