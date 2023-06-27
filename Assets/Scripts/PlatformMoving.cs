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
    private readonly int PIC_CAPACITY = 5; // picturestatus code�� piccapicty�� ���ƾ� ��. �������� ���� �ʿ� 

    [SerializeField] public GameObject currentPicture; // ���� ���� ������ ������Ʈ. �θ� ������ �������� set �Ǿ�� ��. 
    private PictureStatus curPicStatusCode; // ���� ���� ������ �ڵ� 
    public GameObject[] overLappedPicture; // ���� ���� ������ ��ģ ���� 

    // �÷��� �̵� �� ����ϴ� ���� 
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
        bool[] isInsideArea = new bool[PIC_CAPACITY]; // ��ģ ������ ������ ���ԵǴ��� �Ǻ��ϴ� bool
        var pictureAxis = new(float picCenter, float picHalfWidth)[PIC_CAPACITY];

        Transform curPicTrans = currentPicture.transform;
  
        float scaleX = transform.localScale.x * curPicTrans.localScale.x;
        float scaleY = transform.localScale.y * curPicTrans.localScale.y;

        for(int i= 0;i < PIC_CAPACITY;i++) 
        {
            if (overLappedPicture[i] != null) // ��ģ ������ �ִ� ���.
            {
                //overLappedPicture = currentPicStatus.otherPic; 
                switch (distinguisher)
                {
                    case 0: // ���� �̵� case
                        isInsideArea[i]= ((transform.position.y + scaleY / 2) <= overLappedPicture[i].transform.position.y + overLappedPicture[i].transform.localScale.y / 2)
                                    && ((transform.position.y - scaleY / 2) >= overLappedPicture[i].transform.position.y - overLappedPicture[i].transform.localScale.y / 2)
                                    ? true : false; // ���� �̵��� ��� ��ģ ������ ���� ������ ���ԵǴ��� �Ǵ�.
                        break;
                    case 1: // ���� �̵� case
                        isInsideArea[i] = ((transform.position.x + scaleX / 2) <= overLappedPicture[i].transform.position.x + overLappedPicture[i].transform.localScale.x / 2)
                                    && ((transform.position.x - scaleX / 2) >= overLappedPicture[i].transform.position.x - overLappedPicture[i].transform.localScale.x / 2)
                                    ? true : false; // ���� �̵��� ��� ��ģ ������ ���� ������ ���ԵǴ��� �Ǵ�. 
                        break;
                }
            }

            if (overLappedPicture[i] == null || !isInsideArea[i]) // ��ġ�� �ʴ� ��� �Ǵ� ������ �Ǻ����� �� ������ ��ģ ������ ���� �ȿ� ���� �ʴ��� �Ǵ�. ������ ���ƾ ���� �ȿ� ������ �̰� �����
            {
                switch (distinguisher)
                {
                    case 0:
                        pictureAxis[i] = (curPicTrans.position.x, curPicTrans.localScale.x * 0.5f); // ��ģ ���������� �߽� ��ǥ , ���� �ִ� ������ ���α��� ����
                        break;
                    case 1:
                        pictureAxis[i] = (curPicTrans.position.y, curPicTrans.localScale.y * 0.5f); 
                        break;
                }
            }
            else if (overLappedPicture[i] != null && isInsideArea[i]) // ������ ��ġ�� ���� ������ ��ģ ������ ���� �ȿ� ���� ���� �� �����ؾ� ��.
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
            switch (distinguisher) // ������ �ε����� �� ���� �ٲ��ִ� ���� 
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
        // ���� �ϳ��� ���� �� 
        // currentPicrure�� ��ǥ �����ؼ� ���� �浹


        // ���� n�� �̻� ��ĥ �� 
        // isinsideArea ���� ��, true�� �ش��ϴ� ������ ���ؼ��� ���� �ݶ��̴� ��ǥ ���� 

        // ������ ������ ���ؼ�, transform.position �� �� 

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
    void Crossing_Regulation() // �� ������ ��� ���̿��� ������ �������� ��쿡 ���� ����ó��. ������ �� �����̰� �Ѵ�.
    {

        for(int i=0; i<PIC_CAPACITY; i++)
        {
            if (overLappedPicture[i] != null)
            {
                otherPicsRigid[i] = overLappedPicture[i].GetComponent<Rigidbody2D>();

                if (!curPicStatusCode.Is_Plate_In_OtherPic(transform.gameObject)[i]) // ������ ��������, ��ģ ���� ������ �� ���� ���� ���
                {
                    curPicRigid.velocity = Vector2.zero;
                    otherPicsRigid[i].velocity = Vector2.zero;
                }
            }
          
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
  
        if(collision.transform.tag == "Platform") // �̰� �� �ȵ� ��
        {
            direction = -direction;
        }
    }

}
