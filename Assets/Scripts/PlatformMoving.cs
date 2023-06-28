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
    private readonly int PIC_CAPACITY = 5; // picturestatus code�� piccapicty�� ���ƾ� ��. �������� ���� �ʿ� 

    [SerializeField] public GameObject currentPicture; // ���� ���� ������ ������Ʈ. �θ� ������ �������� set �Ǿ�� ��. 
    public PictureStatus curPicStatusCode; // ���� ���� ������ �ڵ� 
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
            else // ��ģ ������ �ִ� ���
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
        bool[] isInsideArea = new bool[PIC_CAPACITY]; // ��ģ ������ ������ ���ԵǴ��� �Ǻ��ϴ� bool
        Transform curPicTrans = currentPicture.transform;

        (float realPicCenter, float realPicHalfWidth) realPicAxis = (0, 0);

        switch (distinguisher)
        {
            case 0:
                realPicAxis = (curPicTrans.position.x, curPicTrans.localScale.x); // halfwidth�� ���߿� /2 ����� ��.
                break;
            case 1:
                realPicAxis = (curPicTrans.position.y, curPicTrans.localScale.y);
                break;
        }
        float PicCount = 1; // ����� ���ϴ� �뵵�̱� ������ �ʱⰪ�� 0�� �ƴ� 1�� �����Ǿ�� �� 

        float scaleX = transform.localScale.x * curPicTrans.localScale.x;
        float scaleY = transform.localScale.y * curPicTrans.localScale.y;

        for(int i= 0; i < PIC_CAPACITY;i++) 
        {
            // 1. isInsideArea �� ����
            if (overLappedPicture[i] != null) // ��ģ ������ �ִ� ���.
            {
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

            // 2. isInsideArea ������ �����ϴ� ������ ���� ��ǥ�� ���� 
            if (overLappedPicture[i] != null && isInsideArea[i]) // ������ ��ġ�� ���� ������ ��ģ ������ ���� �ȿ� ���� ���� �� �����ؾ� ��.
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

        realPicAxis.realPicCenter /= PicCount; // ��ǥ ��� ���ϱ�
        realPicAxis.realPicHalfWidth /= 2; // ���� ���� �̵����� ���ϱ� 

        switch (distinguisher) // ������ �ε����� �� ���� �ٲ��ִ� ���� 
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
                break; // ������ ������ ���� �ƴ϶�, ������ �����ϴ� ��ģ ���� '��ü'�� ����� ���ؾ� �Ѵ�. 

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
