using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureStatus : MonoBehaviour
{
    [SerializeField] public readonly int PIC_CAPACITY = 5; // ��ģ ������ �����ϴ� �뷮. ���� �� ������ �ִ� 3�� ���� �����ϰ� ����.
                                                          // �� PlatformMoving code�� picCapacity�� ũ�� ������� ��. �ϴ� �������� ������ �� 
    public GameObject[] otherPics;
    private Transform[] otherPicTrans;
    public PictureStatus otherPicCode;

    public List<GameObject> platformList = new List<GameObject>();
    public PlatformMoving platformCode;


    void Start()
    {
        otherPics = new GameObject[PIC_CAPACITY];
        otherPicTrans = new Transform[PIC_CAPACITY];
                                                                                               

        for(int i= 0; i < PIC_CAPACITY; i++) // �ִ�뷮��ŭ for loop
        {
            // ������ �ʱ�ȭ.
            otherPics[i] = null;
            otherPicTrans[i] = null;
        }

    }

    public bool[] Is_Plate_In_OtherPic(GameObject mPlatform) // ������ ������ ���� ���� �ȿ� ������ ���� true ��ȯ�ϴ� �Լ� 
    {
        Transform[] overlapPicCenter = new Transform[PIC_CAPACITY];

        float[] overlapPicLeftX = new float[PIC_CAPACITY], overlapPicRightX = new float[PIC_CAPACITY],
                overlapPicTopY = new float[PIC_CAPACITY], overlapPicBottomY = new float[PIC_CAPACITY];

        float lCornerPointX, rCornerPointX, tCornerPointY, bCornerPointY;

        // ������ �ٸ� ���� ���� �ȿ� �ִ� ���� �����ؾ� ��.
        bool[]returnBoolArray = new bool[PIC_CAPACITY];

        Transform plateTrans = mPlatform.transform;

        // ������ �� ������ ��ǥ ����.
        lCornerPointX = plateTrans.position.x - plateTrans.localScale.x * 2 / 2;
        rCornerPointX = plateTrans.position.x + plateTrans.localScale.x * 2/ 2;
        tCornerPointY = plateTrans.position.y + plateTrans.localScale.y / 2 / 2;
        bCornerPointY = plateTrans.position.y - plateTrans.localScale.y / 2 / 2;

        for (int i = 0; i < PIC_CAPACITY; ++i) // �� ���Ǹ��� 5 size�� bool �迭 ������ ��.
        {
            if (otherPics[i] != null)
            {
                overlapPicCenter[i] = otherPics[i].transform;

                // ��ģ ������ �� ��ǥ ����.
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

   

    private void AddListNoRepeat(List<GameObject> platformList, GameObject platformToPut) // �ߺ� ���� ����Ʈ �߰���Ű�� �Լ�.
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
                if (otherPics[i] == null) // ����ִ� �����̳ʿ� ����, ontriggerenter���� �νĵ� ������ �߰���Ų��.
                {
                    otherPics[i] = collision.gameObject;
                    break;
                }
            }
        } // �̰� ���� flag���� �ε��ĵ� �����. �����ؾ� �� 


    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Picture"))
        {
            for (int i = 0; i < PIC_CAPACITY; i++) // ��ü �����ִ� ������ ���ؼ�,
            {
                if ((otherPics[i] != null) && (collision.gameObject == otherPics[i].gameObject)) // ���ƴٰ� �������� ���� �̸��� i��° �����̳ʿ� ����� ���� �̸��� ������ �Ǻ�.
                {
                    otherPics[i] = null; 
                    break;
                }
            }
        }

        if(collision.CompareTag("MovingPlatform")) // movingplatform�� ������ ������, isoverlap �޼ҵ��� �� ���ǿ� ���� �迭�� ���� ���̴�. 
        {
            bool isRemoved = false;
            foreach (GameObject mPlatform in platformList) // ����Ʈ�� ����� �� ���� �ҷ�����.
            {
                bool[] isPlateinOtherPic = Is_Plate_In_OtherPic(mPlatform); // �� ���Ǹ��� 5���� �迭 �ҷ�����.
                for (int i = 0; i < PIC_CAPACITY; i++)
                {
                    if ((collision.gameObject == mPlatform) && isPlateinOtherPic[i]) // i��° ������ ������ �� ���. + �� ������ ���� ������ ����� ������ ���
                    {
                        otherPicCode = otherPics[i].GetComponent<PictureStatus>();
                        platformCode = mPlatform.GetComponent<PlatformMoving>();

                        // �÷����� curPic ��������� ��. + curPicStatusCode��.
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

                        // �θ���� ���� 
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

    public float[] Calculate_FullLengthX() // ���� ������ ���α��̿� �ٸ� ������ ���α��̸� ��� ���� �Լ� 
    {
        float[] fullLength = new float[PIC_CAPACITY];
        for(int i=0; i<PIC_CAPACITY; ++i)
        {
            if (otherPics[i] != null) // ��ģ ���¿��� �����Ų��. else null reference error �߻� ���ɼ� ���� 
                fullLength[i] = transform.localScale.x + otherPics[i].transform.localScale.x;
            else // �������� ���� ��� 
                continue; // pass
        }

        return fullLength;
    }
    public float[] Calculate_OverlapAreaX() // ������ ��ġ�� ��� ��ġ�� X ���� ��� 
    {
        Transform[] otherPicTrans = new Transform[PIC_CAPACITY];

        float[] fullLengthX = Calculate_FullLengthX();
        float[] realLengthX = new float[PIC_CAPACITY];   
        float[] overlapSizeX = new float[PIC_CAPACITY];
        for (int i=0; i<PIC_CAPACITY; i++)
        {
            if (otherPics[i] != null) // �����ִ� ��쿡�� �����Ͽ��� null reference ���� ���� 
            {
                otherPicTrans[i] = otherPics[i].transform; // ��ġ ����.

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


    public float[] Calculate_FullLengthY() // ���� ������ ���α��̿� �ٸ� ������ ���α��̸� ��� ���� �Լ� 
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
    public float[] Calculate_OverlapAreaY() // ������ ��ġ�� ��� ��ġ�� X ���� ��� 
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
