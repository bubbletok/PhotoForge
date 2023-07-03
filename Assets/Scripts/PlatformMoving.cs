using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMoving : MonoBehaviour
{
    private readonly int PIC_CAPACITY = 5; // picturestatus code의 piccapicty와 같아야 함. 수동으로 수정 필요 

    [SerializeField] public GameObject currentPicture; // 현재 속한 사진의 오브젝트. 부모 관계의 사진으로 set 되어야 함. 
    public PictureStatus curPicStatusCode; // 현재 속한 사진의 코드 
    public GameObject[] overLappedPicture; // 현재 속한 사진과 겹친 사진 


    // 플랫폼 이동 시 사용하는 변수 
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
                distWithPic = transform.position.y - currentPicture.transform.position.y; // 걍 이렇게하면 양 음수 둘다 처리 가능 
                break;

            case 1:
                distWithPic = transform.position.x - currentPicture.transform.position.x; // 걍 이렇게하면 양 음수 둘다 처리 가능 
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
            else // 겹친 사진이 있는 경우
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

        bool isInsideAtleast = false;
        for (int i = 0; i < PIC_CAPACITY; i++)
        {
            // 1. isInsideArea 값 도출
            if (overLappedPicture[i] != null) // 겹친 사진이 있는 경우.
            {
                switch (distinguisher)
                {
                    case 0: // 가로 이동 case
                        isInsideArea[i] = ((transform.position.y + transform.localScale.y / 2 / 2) <= overLappedPicture[i].transform.position.y + overLappedPicture[i].transform.localScale.y / 2)
                                    && ((transform.position.y - transform.localScale.y / 2 / 2) >= overLappedPicture[i].transform.position.y - overLappedPicture[i].transform.localScale.y / 2)
                                    ? true : false; // 가로 이동의 경우 겹친 사진의 세로 영역에 포함되는지 판단.
                        if (isInsideArea[i])
                            isInsideAtleast = true;
                        break;
                    case 1: // 세로 이동 case
                        isInsideArea[i] = ((transform.position.x + transform.localScale.x * 2 / 2) <= overLappedPicture[i].transform.position.x + overLappedPicture[i].transform.localScale.x * 1.81f / 2)
                                    && ((transform.position.x - transform.localScale.x * 2 / 2) >= overLappedPicture[i].transform.position.x - overLappedPicture[i].transform.localScale.x * 1.81f / 2)
                                    ? true : false; // 세로 이동의 경우 겹친 사진의 가로 영역에 포함되는지 판단. 
                        if (isInsideArea[i])
                            isInsideAtleast = true;
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

        Vector2 curPicvel = currentPicture.GetComponent<Rigidbody2D>().velocity;

        if (isInsideAtleast && distinguisher == 0) // 한개라도 겹친 사진이 있는 경우 
        {
            realPicAxis.realPicHalfWidth *= 1.3f;
        }
        else if (!isInsideAtleast && distinguisher == 0) // 사진 한개만 독립적으로 있는 경우 
        {
            realPicAxis.realPicHalfWidth *= 1.81f;
        }

        switch (distinguisher) // 사진과 부딪쳤을 때 방향 바꿔주는 구문 
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
                break; // 각각에 접근할 것이 아니라, 조건을 만족하는 겹친 사진 '전체'의 평균을 구해야 한다. 

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

        picLeftX = currentPicture.transform.position.x - currentPicture.transform.localScale.x * 1.83f / 2; // 원래 1.81
        picRightX = currentPicture.transform.position.x + currentPicture.transform.localScale.x * 1.83f / 2;
        picTopY = currentPicture.transform.position.y + currentPicture.transform.localScale.y * 1.03f / 2; // 원래 1 
        picBottomY = currentPicture.transform.position.y - currentPicture.transform.localScale.y * 1.03f / 2;

        if (lCornerPointX >= picLeftX && rCornerPointX <= picRightX
                   && tCornerPointY <= picTopY && bCornerPointY >= picBottomY)
            return true;
        else
            return false;

    }

    void Crossing_Regulation() // 두 사진의 경계 사이에서 발판이 지나가는 경우에 대한 예외처리. 사진을 못 움직이게 한다.
    {
        for (int i = 0; i < PIC_CAPACITY; i++)
        {
            if (overLappedPicture[i] != null)
            {
                // 발판이 어느 사진에도 완전히 들어가있지 않은 경우 

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
                transform.position = new Vector2(transform.position.x, currentPicture.transform.position.y + distWithPic); // 발판y < 사진 y라면 distwithpic은 알아서 음수가 될 것.
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