using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMoving : MonoBehaviour
{
    private readonly int PIC_CAPACITY = 5; // picturestatus code�� piccapicty�� ���ƾ� ��. �������� ���� �ʿ� 

    [SerializeField] public GameObject currentPicture; // ���� ���� ������ ������Ʈ. �θ� ������ �������� set �Ǿ�� ��. 
    public PictureStatus curPicStatusCode; // ���� ���� ������ �ڵ� 
    public GameObject[] overLappedPicture; // ���� ���� ������ ��ģ ���� 


    // �÷��� �̵� �� ����ϴ� ���� 
    [SerializeField] public int directionChoose = 0;
    [SerializeField] private float platformSpeed = 5f;
    private int direction = 1;

    private Rigidbody2D thisRigid;
    public Rigidbody2D curPicRigid;

    private bool isOverlap;
    public bool[] isCrossing;

    public float distWithPic;

    void Start()
    {
        //currentPicture = GetComponentInParent<PictureStatus>().gameObject;
        curPicStatusCode = currentPicture.GetComponent<PictureStatus>();
        overLappedPicture = new GameObject[PIC_CAPACITY];

        thisRigid = GetComponent<Rigidbody2D>();
        curPicRigid = currentPicture.GetComponent<Rigidbody2D>();
        isOverlap = false;
        isCrossing = new bool[PIC_CAPACITY];

        switch (directionChoose)
        {
            case 0:
                distWithPic = transform.position.y - currentPicture.transform.position.y; // �� �̷����ϸ� �� ���� �Ѵ� ó�� ���� 
                break;

            case 1:
                distWithPic = transform.position.x - currentPicture.transform.position.x; // �� �̷����ϸ� �� ���� �Ѵ� ó�� ���� 
                break;
        }
    }


    void Update()
    {
        for (int i = 0; i < PIC_CAPACITY; i++)
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

        if (isOverlap)
        {
            MovingFunction_Overlap(directionChoose);
        }
        else
        {
            MovingFunction_Alone(directionChoose);
        }
        Crossing_Regulation();

        ErrorPlate_Return();
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

        bool isInsideAtleast = false;
        for (int i = 0; i < PIC_CAPACITY; i++)
        {
            // 1. isInsideArea �� ����
            if (overLappedPicture[i] != null) // ��ģ ������ �ִ� ���.
            {
                switch (distinguisher)
                {
                    case 0: // ���� �̵� case
                        isInsideArea[i] = ((transform.position.y + transform.localScale.y / 2 / 2) <= overLappedPicture[i].transform.position.y + overLappedPicture[i].transform.localScale.y / 2)
                                    && ((transform.position.y - transform.localScale.y / 2 / 2) >= overLappedPicture[i].transform.position.y - overLappedPicture[i].transform.localScale.y / 2)
                                    ? true : false; // ���� �̵��� ��� ��ģ ������ ���� ������ ���ԵǴ��� �Ǵ�.
                        if (isInsideArea[i])
                            isInsideAtleast = true;
                        break;
                    case 1: // ���� �̵� case
                        isInsideArea[i] = ((transform.position.x + transform.localScale.x * 2 / 2) <= overLappedPicture[i].transform.position.x + overLappedPicture[i].transform.localScale.x * 1.81f / 2)
                                    && ((transform.position.x - transform.localScale.x * 2 / 2) >= overLappedPicture[i].transform.position.x - overLappedPicture[i].transform.localScale.x * 1.81f / 2)
                                    ? true : false; // ���� �̵��� ��� ��ģ ������ ���� ������ ���ԵǴ��� �Ǵ�. 
                        if (isInsideArea[i])
                            isInsideAtleast = true;
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

        Vector2 curPicvel = currentPicture.GetComponent<Rigidbody2D>().velocity;

        if (isInsideAtleast && distinguisher == 0) // �Ѱ��� ��ģ ������ �ִ� ��� 
        {
            realPicAxis.realPicHalfWidth *= 1.3f;
        }
        else if (!isInsideAtleast && distinguisher == 0) // ���� �Ѱ��� ���������� �ִ� ��� 
        {
            realPicAxis.realPicHalfWidth *= 1.81f;
        }

        switch (distinguisher) // ������ �ε����� �� ���� �ٲ��ִ� ���� 
        {
            case 0:
                print(realPicAxis.realPicCenter + " " + realPicAxis.realPicHalfWidth);
                if ((transform.position.x + transform.localScale.x * 2 * 0.5f) >= (realPicAxis.realPicCenter + realPicAxis.realPicHalfWidth))
                {
                    direction = -1;
                }
                else if ((transform.position.x - transform.localScale.x * 2 * 0.5f) <= (realPicAxis.realPicCenter - realPicAxis.realPicHalfWidth))
                {
                    direction = 1;
                }
                thisRigid.velocity = new Vector2(curPicvel.x + direction * platformSpeed, curPicvel.y);
                break; // ������ ������ ���� �ƴ϶�, ������ �����ϴ� ��ģ ���� '��ü'�� ����� ���ؾ� �Ѵ�. 

            case 1:
                if ((transform.position.y + transform.localScale.y * 0.5f * 0.5f) >= (realPicAxis.realPicCenter + realPicAxis.realPicHalfWidth))
                {
                    direction = -1;
                }
                else if ((transform.position.y - transform.localScale.y * 0.5f * 0.5f) <= (realPicAxis.realPicCenter - realPicAxis.realPicHalfWidth))
                {
                    direction = 1;
                }
                thisRigid.velocity = new Vector2(curPicvel.x, curPicvel.y + direction * platformSpeed);
                break;
        }

        switch (distinguisher)
        {
            case 0:
                if(transform.position.x < realPicAxis.realPicCenter - realPicAxis.realPicHalfWidth)
                { 
                    direction = 1;
                }
                else if (transform.position.x > realPicAxis.realPicCenter + realPicAxis.realPicHalfWidth)
                {
                    direction = -1;
                }
                break;
            case 1:
                if (transform.position.y < realPicAxis.realPicCenter - realPicAxis.realPicHalfWidth)
                {
                    direction = 1;
                }
                else if (transform.position.y > realPicAxis.realPicCenter + realPicAxis.realPicHalfWidth)
                {
                    direction = -1;
                }
                break;
        }

    }

    void MovingFunction_Alone(int distinguisher)
    {
        Transform curPicTrans = currentPicture.transform;

        Vector2 curPicvel = currentPicture.GetComponent<Rigidbody2D>().velocity;

        switch (distinguisher)
        {
            case 0:
                if ((transform.position.x + transform.localScale.x * 2 * 0.5f) >= (curPicTrans.position.x + curPicTrans.localScale.x * 1.81f * 0.5f))
                {
                    direction = -1;
                }
                else if ((transform.position.x - transform.localScale.x * 2 * 0.5f) <= (curPicTrans.position.x - curPicTrans.localScale.x * 1.81f * 0.5f))
                {
                    direction = 1;
                }

                thisRigid.velocity = new Vector2(curPicvel.x + direction * platformSpeed, curPicvel.y);
                break;

            case 1:
                if ((transform.position.y + transform.localScale.y * 0.5f * 0.5f) >= (curPicTrans.position.y + curPicTrans.localScale.y * 0.5f))
                    direction = -1;
                else if ((transform.position.y - transform.localScale.y * 0.5f * 0.5f) <= (curPicTrans.position.y - curPicTrans.localScale.y * 0.5f))
                    direction = 1;

                thisRigid.velocity = new Vector2(curPicvel.x, curPicvel.y + direction * platformSpeed);
                break;
        }
    }
    private bool Is_Plate_In_CurPic()
    {
        float lCornerPointX, rCornerPointX, tCornerPointY, bCornerPointY;
        float picLeftX, picRightX, picTopY, picBottomY;

        //Transform platformTrans = mPlatform.transform;

        lCornerPointX = transform.position.x - transform.localScale.x * 2 / 2;
        rCornerPointX = transform.position.x + transform.localScale.x * 2 / 2;
        tCornerPointY = transform.position.y + transform.localScale.y / 2 / 2;
        bCornerPointY = transform.position.y - transform.localScale.y / 2 / 2;

        picLeftX = currentPicture.transform.position.x - currentPicture.transform.localScale.x * 1.83f / 2; // ���� 1.81
        picRightX = currentPicture.transform.position.x + currentPicture.transform.localScale.x * 1.83f / 2;
        picTopY = currentPicture.transform.position.y + currentPicture.transform.localScale.y * 1.03f / 2; // ���� 1 
        picBottomY = currentPicture.transform.position.y - currentPicture.transform.localScale.y * 1.03f / 2;

        if (lCornerPointX >= picLeftX && rCornerPointX <= picRightX
                   && tCornerPointY <= picTopY && bCornerPointY >= picBottomY)
            return true;
        else
            return false;

    }

    void Crossing_Regulation() // �� ������ ��� ���̿��� ������ �������� ��쿡 ���� ����ó��. ������ �� �����̰� �Ѵ�.
    {
        for (int i = 0; i < PIC_CAPACITY; i++)
        {
            if (overLappedPicture[i] != null)
            {
                // ������ ��� �������� ������ ������ ���� ��� 

                if (!Is_Plate_In_CurPic() && isCrossing[i])
                {
                    if (currentPicture.GetComponent<PictureMovement>() != null)
                        currentPicture.GetComponent<PictureMovement>().cantMove = true;

                    if (overLappedPicture[i].GetComponent<PictureMovement>() != null)
                        overLappedPicture[i].GetComponent<PictureMovement>().cantMove = true;
                }
            }
        }
    }

    void ErrorPlate_Return()
    {
        bool[] isPlateinOther = curPicStatusCode.Is_Plate_In_OtherPic(transform.gameObject);
        /*
                bool inOtherPic = false;
                for(int i=0; i<PIC_CAPACITY; i++)
                {
                    if (isPlateinOther[i])
                    {
                        inOtherPic = true;
                        break;
                    }
                    else
                        continue;
                }*/


        switch (directionChoose)
        {
            case 0:
                transform.position = new Vector2(transform.position.x, currentPicture.transform.position.y + distWithPic); // ����y < ���� y��� distwithpic�� �˾Ƽ� ������ �� ��.
                break;

            case 1:
                transform.position = new Vector2(currentPicture.transform.position.x + distWithPic, transform.position.y);
                break;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Platform")
        {
            direction = -direction;
        }

        if (collision.transform.tag == "MovingPlatform")
        {
            switch (directionChoose)
            {
                case 0:
                    if (collision.gameObject.GetComponent<PlatformMoving>().directionChoose == 0)
                        direction = -direction;
                    break;

                case 1:
                    direction = -direction;
                    break;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Picture"))
        {
            for (int i = 0; i < PIC_CAPACITY; i++)
            {
                if (collision.gameObject == overLappedPicture[i])
                {
                    isCrossing[i] = true;
                    break;
                }
            }
        }

        if (collision.transform.tag == "Spike")
        {
            direction = -direction;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Picture"))
        {
            for (int i = 0; i < PIC_CAPACITY; i++)
            {
                if (collision.gameObject == currentPicture && isCrossing[i])
                {
                    isCrossing[i] = false;
                    break;
                }
            }
        }
    }


}