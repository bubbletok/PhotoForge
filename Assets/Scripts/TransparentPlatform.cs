using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class TransparentPlatform : MonoBehaviour
{
    [SerializeField] GameObject[] otherTransPlatforms;
    [SerializeField] GameObject[] newColl;
    public readonly int PLATE_CAPACITY = 5;

    Vector2 newCollTransform;

    BoxCollider2D myPlatformColl;
    Collider2D[] otherPlatformColls;


    private void Start()
    {
        otherTransPlatforms = new GameObject[PLATE_CAPACITY];
        newColl = new GameObject[PLATE_CAPACITY];
        myPlatformColl = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        Calculate_OverlapAreaX_Transparent();
        Calculate_OverlapAreaY_Transparent();
        //print(Calculate_FullLengthX_Transparent()[0]);
        //print(Calculate_OverlapAreaX_Transparent()[0] + " " + Calculate_OverlapAreaY_TransParent()[0]);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<TransparentPlatform>() != null)
        {
            for (int i = 0; i < PLATE_CAPACITY; i++)
            {
                if (otherTransPlatforms[i] == null) // 비어있는 컨테이너에 대해, ontriggerenter에서 인식된 사진을 추가시킨다.
                {
                    otherTransPlatforms[i] = collision.gameObject;
                    break;
                }
            }
        } // 이거 지금 flag랑도 부딪쳐도 실행됨. 수정해야 함 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<TransparentPlatform>() != null)
        {
            for (int i = 0; i < PLATE_CAPACITY; i++) // 전체 겹쳐있는 사진에 대해서,
            {
                if ((otherTransPlatforms[i] != null) && (collision.gameObject == otherTransPlatforms[i].gameObject)) // 겹쳤다가 빠져나온 사진 이름이 i번째 컨테이너에 저장된 사진 이름과 같은지 판별.
                {
                    otherTransPlatforms[i] = null;
                    break;
                }
            }
        }
    }

    public float[] Calculate_FullLengthX_Transparent()
    {
        float[] fullLength = new float[PLATE_CAPACITY];
        for (int i = 0; i < PLATE_CAPACITY; ++i)
        {
            if (otherTransPlatforms[i] != null)
            {
                if (otherTransPlatforms[i].GetComponent<Rigidbody2D>().velocity == Vector2.zero)
                    fullLength[i] = transform.localScale.x * 2 + otherTransPlatforms[i].transform.localScale.x * 2 * 7;

                else if (gameObject.GetComponent<Rigidbody2D>().velocity == Vector2.zero)
                    fullLength[i] = transform.localScale.x * 2 * 7 + otherTransPlatforms[i].transform.localScale.x * 2;

                else
                    fullLength[i] = transform.localScale.x * 2 + otherTransPlatforms[i].transform.localScale.x * 2;
            }
            else // 겹쳐있지 않은 경우 
                continue; // pass
        }

        return fullLength;
    }

    public float[] Calculate_FullLengthY_TransParent() // 현재 사진의 가로길이와 다른 사진의 가로길이를 모두 더한 함수 
    {
        float[] fullLength = new float[PLATE_CAPACITY];
        for (int i = 0; i < PLATE_CAPACITY; ++i)
        {
            if (otherTransPlatforms[i] != null)
            {
                if (otherTransPlatforms[i].GetComponent<Rigidbody2D>().velocity == Vector2.zero)
                    fullLength[i] = transform.localScale.y / 2 + otherTransPlatforms[i].transform.localScale.y / 2 * 7;

                else if (gameObject.GetComponent<Rigidbody2D>().velocity == Vector2.zero)
                    fullLength[i] = transform.localScale.y / 2 * 7 + otherTransPlatforms[i].transform.localScale.y / 2;

                else
                    fullLength[i] = transform.localScale.y / 2 + otherTransPlatforms[i].transform.localScale.y / 2;
            }
            else
                continue;
        }
        return fullLength;
    }

    public float[] Calculate_OverlapAreaX_Transparent() // 사진이 겹치는 경우 겹치는 X 범위 계산 
    {

        Vector2[] otherPlatesLocalScale = new Vector2[PLATE_CAPACITY];
        Vector2 myPlateLocalScale = transform.localScale;

        float[] fullLengthX = Calculate_FullLengthX_Transparent();
        float[] realLengthX = new float[PLATE_CAPACITY];
        float[] overlapSizeX = new float[PLATE_CAPACITY];


        for (int i = 0; i < PLATE_CAPACITY; i++)
        {
            if (otherTransPlatforms[i] != null)
            {
                otherPlatesLocalScale[i] = otherTransPlatforms[i].transform.localScale; // 크기 저장.

                if (otherTransPlatforms[i].GetComponent<Rigidbody2D>().velocity == Vector2.zero)
                    otherPlatesLocalScale[i] = new Vector2(otherTransPlatforms[i].transform.localScale.x * 7, otherTransPlatforms[i].transform.localScale.y * 7);

                if (gameObject.GetComponent<Rigidbody2D>().velocity == Vector2.zero)
                    myPlateLocalScale = new Vector2(transform.localScale.x * 7, transform.localScale.y * 7);


                float otherPlateRightX = otherTransPlatforms[i].transform.position.x + otherPlatesLocalScale[i].x * 2 / 2;
                float otherPlateLeftX = otherTransPlatforms[i].transform.position.x - otherPlatesLocalScale[i].x * 2 / 2;
                float thisPlateRightX = transform.position.x + myPlateLocalScale.x * 2 / 2;
                float thisPlateLeftX = transform.position.x - myPlateLocalScale.x * 2 / 2;
                //print(this.name + " " + transform.localScale.x + " " + transform.position.x + " " + thisPlateLeftX + " " + thisPlateRightX);
                if (otherPlateLeftX <= thisPlateRightX && otherPlateLeftX >= thisPlateLeftX && otherPlateRightX > thisPlateRightX)
                {
                    //print(gameObject.name + "  " + "Case1");
                    //realLengthX[i] = (otherTransPlatforms[i].transform.position.x + otherPlatesLocalScale[i].x / 2) - (transform.position.x - myPlateLocalScale.x / 2);
                    overlapSizeX[i] = Mathf.Abs(thisPlateRightX - otherPlateLeftX);

                    //?

                }
                else if (otherPlateRightX >= thisPlateLeftX && otherPlateRightX <= thisPlateRightX && otherPlateLeftX < thisPlateLeftX)
                {
                    //print(gameObject.name + "  " + "Case2");

                    //realLengthX[i] = (transform.position.x + myPlateLocalScale.x / 2) - (otherTransPlatforms[i].transform.position.x - otherPlatesLocalScale[i].x / 2);
                    overlapSizeX[i] = Mathf.Abs(otherPlateRightX - thisPlateLeftX);
                }
                else if (otherPlateLeftX >= thisPlateLeftX && otherPlateRightX <= thisPlateRightX)
                {
                    //print(gameObject.name + "  " + "Case3");

                    //realLengthX[i] = otherPlatesLocalScale[i].x;
                    overlapSizeX[i] = otherPlatesLocalScale[i].x;
                }
                else if (otherPlateLeftX <= thisPlateLeftX && otherPlateRightX >= thisPlateRightX)
                {
                    //print(gameObject.name + "  " + "Case4");

                    //realLengthX[i] = myPlateLocalScale.x;
                    overlapSizeX[i] = myPlateLocalScale.x;
                }
            }
        }

        return overlapSizeX;
    }


    public float[] Calculate_OverlapAreaY_Transparent() // 사진이 겹치는 경우 겹치는 Y범위 계산  이거 이따가 수정 필요 
    {
        Vector2[] otherPlatesLocalScale = new Vector2[PLATE_CAPACITY];
        Vector2 myPlateLocalScale = transform.localScale;

        float[] fullLengthY = Calculate_FullLengthY_TransParent();
        float[] realLengthY = new float[PLATE_CAPACITY];
        float[] overlapSizeY = new float[PLATE_CAPACITY];

        for (int i = 0; i < PLATE_CAPACITY; i++)
        {
            if (otherTransPlatforms[i] != null)
            {
                if (otherTransPlatforms[i].GetComponent<Rigidbody2D>().velocity == Vector2.zero)
                    otherPlatesLocalScale[i] = new Vector2(otherTransPlatforms[i].transform.localScale.x * 7, otherTransPlatforms[i].transform.localScale.y * 7);

                if (gameObject.GetComponent<Rigidbody2D>().velocity == Vector2.zero)
                    myPlateLocalScale = new Vector2(transform.localScale.x * 7, transform.localScale.y * 7);


                float otherPlayerUpY = otherTransPlatforms[i].transform.position.y + otherPlatesLocalScale[i].y / 2 / 2;
                float otherPlayerDownY = otherTransPlatforms[i].transform.position.y - otherPlatesLocalScale[i].y / 2 / 2;
                float thisPlateUpY = transform.position.y + myPlateLocalScale.y / 2 / 2;
                float thisPlateDownY = transform.position.y - myPlateLocalScale.y / 2 / 2;
                //print(this.name + " " + transform.localScale.y + " " + transform.position.y + " " + thisPlateDownY + " " + thisPlateUpY);
                print(this.name);
                print(otherPlayerDownY + " " + thisPlateUpY + " " + thisPlateDownY + " " + otherPlayerUpY);
                if (otherPlayerDownY <= thisPlateUpY && otherPlayerDownY >= thisPlateDownY && otherPlayerUpY > thisPlateUpY)
                {
                    //print(gameObject.name + "  " + "Case1");
                    //realLengthX[i] = (otherTransPlatforms[i].transform.position.x + otherPlatesLocalScale[i].x / 2) - (transform.position.x - myPlateLocalScale.x / 2);
                    overlapSizeY[i] = Mathf.Abs(thisPlateUpY - otherPlayerDownY);
                }
                else if (otherPlayerUpY >= thisPlateDownY && otherPlayerUpY <= thisPlateUpY && otherPlayerDownY < thisPlateDownY)
                {
                    //print(gameObject.name + "  " + "Case2");

                    //realLengthX[i] = (transform.position.x + myPlateLocalScale.x / 2) - (otherTransPlatforms[i].transform.position.x - otherPlatesLocalScale[i].x / 2);
                    overlapSizeY[i] = Mathf.Abs(otherPlayerUpY - thisPlateDownY);
                }
                else if (otherPlayerDownY >= thisPlateDownY && otherPlayerUpY <= thisPlateUpY)
                {
                    //print(gameObject.name + "  " + "Case3");

                    //realLengthX[i] = otherPlatesLocalScale[i].x;
                    overlapSizeY[i] = otherPlatesLocalScale[i].y;
                }
                else if (otherPlayerDownY <= thisPlateDownY && otherPlayerUpY >= thisPlateUpY)
                {
                    //print(gameObject.name + "  " + "Case4");

                    //realLengthX[i] = myPlateLocalScale.x;
                    overlapSizeY[i] = myPlateLocalScale.y;
                }
            }
        }


        return overlapSizeY;
    }


}