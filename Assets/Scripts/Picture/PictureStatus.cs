using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureStatus : MonoBehaviour
{
    [SerializeField] public readonly int PIC_CAPACITY = 5; // 겹친 사진을 저장하는 용량. 현재 한 사진당 최대 3개 저장 가능하게 설정.
                                                          // ★ PlatformMoving code의 picCapacity와 크기 맞춰줘야 함. 일단 수동으로 수정할 것 
    public GameObject[] otherPics;
    private Transform[] otherPicTrans;
    public PictureStatus otherPicCode;

    public List<GameObject> platformList = new List<GameObject>();
    public PlatformMoving platformCode;


    void Start()
    {
        otherPics = new GameObject[PIC_CAPACITY];
        otherPicTrans = new Transform[PIC_CAPACITY];
                                                                                               

        for(int i= 0; i < PIC_CAPACITY; i++) // 최대용량만큼 for loop
        {
            // 변수들 초기화.
            otherPics[i] = null;
            otherPicTrans[i] = null;
        }

    }

    public bool[] Is_Plate_In_OtherPic(GameObject mPlatform) // 발판이 겹쳐진 사진 영역 안에 완전히 들어가면 true 반환하는 함수 
    {
        Transform[] overlapPicCenter = new Transform[PIC_CAPACITY];

        float[] overlapPicLeftX = new float[PIC_CAPACITY], overlapPicRightX = new float[PIC_CAPACITY],
                overlapPicTopY = new float[PIC_CAPACITY], overlapPicBottomY = new float[PIC_CAPACITY];

        float lCornerPointX, rCornerPointX, tCornerPointY, bCornerPointY;

        // 발판이 다른 사진 영역 안에 있는 것을 판정해야 함.
        bool[]returnBoolArray = new bool[PIC_CAPACITY];

        Transform plateTrans = mPlatform.transform;

        // 발판의 네 꼭짓점 좌표 저장.
        lCornerPointX = plateTrans.position.x - plateTrans.localScale.x * 2 / 2;
        rCornerPointX = plateTrans.position.x + plateTrans.localScale.x * 2/ 2;
        tCornerPointY = plateTrans.position.y + plateTrans.localScale.y / 2 / 2;
        bCornerPointY = plateTrans.position.y - plateTrans.localScale.y / 2 / 2;

        for (int i = 0; i < PIC_CAPACITY; ++i) // 각 발판마다 5 size의 bool 배열 가져야 함.
        {
            if (otherPics[i] != null)
            {
                overlapPicCenter[i] = otherPics[i].transform;

                // 겹친 사진의 네 좌표 저장.
                overlapPicLeftX[i] = overlapPicCenter[i].position.x - otherPics[i].transform.localScale.x * 1.81f / 2;
                overlapPicRightX[i] = overlapPicCenter[i].position.x + otherPics[i].transform.localScale.x * 1.81f/ 2;
                overlapPicTopY[i] = overlapPicCenter[i].position.y + otherPics[i].transform.localScale.y / 2;
                overlapPicBottomY[i] = overlapPicCenter[i].position.y - otherPics[i].transform.localScale.y / 2;


                if (lCornerPointX >= overlapPicLeftX[i] && rCornerPointX <= overlapPicRightX[i]
                    && tCornerPointY <= overlapPicTopY[i] && bCornerPointY >= overlapPicBottomY[i])
                    returnBoolArray[i] = true;
                else
                    returnBoolArray[i] = false;
            }
            else
                continue;
        }
        return returnBoolArray;
    }

   

    private void AddListNoRepeat(List<GameObject> platformList, GameObject platformToPut) // 중복 없이 리스트 추가시키는 함수.
    {
        bool isRepeated = false;
         foreach(GameObject mPlatform in platformList)
        {
            if (mPlatform == platformToPut)
            {
                isRepeated = true;
                break;
            }
            else
                continue;
        }

        if (!isRepeated)
            platformList.Add(platformToPut);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Picture"))
        {
            for (int i = 0; i < PIC_CAPACITY; i++)
            {
                if (otherPics[i] == null) // 비어있는 컨테이너에 대해, ontriggerenter에서 인식된 사진을 추가시킨다.
                {
                    otherPics[i] = collision.gameObject;
                    break;
                }
            }
        } // 이거 지금 flag랑도 부딪쳐도 실행됨. 수정해야 함 


    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Picture"))
        {
            for (int i = 0; i < PIC_CAPACITY; i++) // 전체 겹쳐있는 사진에 대해서,
            {
                if ((otherPics[i] != null) && (collision.gameObject == otherPics[i].gameObject)) // 겹쳤다가 빠져나온 사진 이름이 i번째 컨테이너에 저장된 사진 이름과 같은지 판별.
                {
                    otherPics[i] = null; 
                    break;
                }
            }
        }

        if(collision.CompareTag("MovingPlatform")) // movingplatform이 완전히 나갔고, isoverlap 메소드의 각 발판에 대한 배열이 있을 것이다. 
        {
            bool isRemoved = false;
            foreach (GameObject mPlatform in platformList) // 리스트에 저장된 각 발판 불러오기.
            {
                bool[] isPlateinOtherPic = Is_Plate_In_OtherPic(mPlatform); // 각 발판마다 5개의 배열 불러오기.
                for (int i = 0; i < PIC_CAPACITY; i++)
                {
                    if ((collision.gameObject == mPlatform) && isPlateinOtherPic[i]) // i번째 사진에 발판이 들어간 경우. + 그 발판이 현재 사진에 저장된 발판인 경우
                    {
                        otherPicCode = otherPics[i].GetComponent<PictureStatus>();
                        platformCode = mPlatform.GetComponent<PlatformMoving>();

                        // 플랫폼의 curPic 변경해줘야 함. + curPicStatusCode도.
                        switch(platformCode.directionChoose)
                        {
                            case 0:
                            /*  bool isUp = mPlatform.transform.position.y >= otherPics[i].transform.position.y ? true : false;
                                if (isUp)
                                    platformCode.distWithPic = mPlatform.transform.position.y - otherPics[i].transform.position.y;
                                else
                                    platformCode.distWithPic = otherPics[i].transform.position.y - mPlatform.transform.position.y;
                                platformCode.distWithPic = Mathf.Abs(platformCode.distWithPic);*/
                                platformCode.distWithPic = mPlatform.transform.position.y - otherPics[i].transform.position.y;
                                break;

                            case 1:
                        /*      bool isRight = mPlatform.transform.position.x >= otherPics[i].transform.position.x ? true : false;
                                if (isRight)
                                    platformCode.distWithPic = mPlatform.transform.position.x - otherPics[i].transform.position.x;
                                else
                                    platformCode.distWithPic = otherPics[i].transform.position.x - mPlatform.transform.position.x;
                                platformCode.distWithPic = Mathf.Abs(platformCode.distWithPic);*/
                                platformCode.distWithPic = mPlatform.transform.position.x - otherPics[i].transform.position.x;
                                break;

                        }

                        platformCode.currentPicture = otherPics[i];
                        platformCode.curPicStatusCode = otherPicCode;
                        platformCode.curPicRigid = otherPics[i].GetComponent<Rigidbody2D>();

                        // 부모관계 변경 
                        AddListNoRepeat(otherPicCode.platformList, mPlatform); 

                        platformList.Remove(mPlatform); 
                        isRemoved = true;
                        break;
                    }
                }

                if (isRemoved)
                    break;
            }
        }
    }

    public float[] Calculate_FullLengthX() // 현재 사진의 가로길이와 다른 사진의 가로길이를 모두 더한 함수 
    {
        float[] fullLength = new float[PIC_CAPACITY];
        for(int i=0; i<PIC_CAPACITY; ++i)
        {
            if (otherPics[i] != null) // 겹친 상태에만 실행시킨다. else null reference error 발생 가능성 높음 
                fullLength[i] = transform.localScale.x + otherPics[i].transform.localScale.x;
            else // 겹쳐있지 않은 경우 
                continue; // pass
        }

        return fullLength;
    }
    public float[] Calculate_OverlapAreaX() // 사진이 겹치는 경우 겹치는 X 범위 계산 
    {
        Transform[] otherPicTrans = new Transform[PIC_CAPACITY];

        float[] fullLengthX = Calculate_FullLengthX();
        float[] realLengthX = new float[PIC_CAPACITY];   
        float[] overlapSizeX = new float[PIC_CAPACITY];
        for (int i=0; i<PIC_CAPACITY; i++)
        {
            if (otherPics[i] != null) // 겹쳐있는 경우에만 실행하여야 null reference 방지 가능 
            {
                otherPicTrans[i] = otherPics[i].transform; // 위치 저장.

                if (transform.position.x < otherPicTrans[i].position.x)
                {
                    realLengthX[i] = (otherPicTrans[i].position.x + otherPicTrans[i].localScale.x / 2) -
                                    (transform.position.x - transform.localScale.x / 2);
                }
                else
                {
                    realLengthX[i] = (transform.position.x + transform.localScale.x / 2) -
                                    (otherPicTrans[i].position.x - otherPicTrans[i].localScale.x / 2);
                }
            }
            overlapSizeX[i] = fullLengthX[i] - realLengthX[i];
        }


        return overlapSizeX;
    }


    public float[] Calculate_FullLengthY() // 현재 사진의 가로길이와 다른 사진의 가로길이를 모두 더한 함수 
    {
        float[] fullLength = new float[PIC_CAPACITY];
        for (int i = 0; i < PIC_CAPACITY; ++i)
        {
            if (otherPics[i] != null)
                fullLength[i] = transform.localScale.y + otherPics[i].transform.localScale.y;
            else
                continue;
        }

        return fullLength;
    }
    public float[] Calculate_OverlapAreaY() // 사진이 겹치는 경우 겹치는 X 범위 계산 
    {
        Transform[] otherPicTrans = new Transform[PIC_CAPACITY];

        float[] fullLengthY = Calculate_FullLengthY();
        float[] realLengthY = new float[PIC_CAPACITY];
        float[] overlapSizeY = new float[PIC_CAPACITY];
        for (int i = 0; i < PIC_CAPACITY; i++)
        {
            if (otherPics[i] != null)
            {
                otherPicTrans[i] = otherPics[i].transform;

                if (transform.position.y < otherPicTrans[i].position.y)
                {
                    realLengthY[i] = (otherPicTrans[i].position.y + otherPicTrans[i].localScale.y / 2) -
                                    (transform.position.y - transform.localScale.y / 2);
                }
                else
                {
                    realLengthY[i] = (transform.position.y + transform.localScale.y / 2) -
                                    (otherPicTrans[i].position.y - otherPicTrans[i].localScale.y / 2);
                }
            }
            overlapSizeY[i] = fullLengthY[i] - realLengthY[i];
        }


        return overlapSizeY;
    }

}
