using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class TransparentPlatform : MonoBehaviour
{
    [SerializeField] GameObject[] otherTransPlatforms;
    [SerializeField] GameObject newCollObject;
    [SerializeField] GameObject[] newColl;
    public readonly int PLATE_CAPACITY = 5;

    BoxCollider2D myPlatformColl;
    Collider2D[] otherPlatformColls;
    [SerializeField] bool[] isOverlappedByX, isOverlappedByY;

    bool atLeastOne;

    private void Start()
    {
        otherTransPlatforms = new GameObject[PLATE_CAPACITY];
        newColl = new GameObject[PLATE_CAPACITY];
        isOverlappedByX = new bool[PLATE_CAPACITY];
        isOverlappedByY = new bool[PLATE_CAPACITY];
        myPlatformColl = GetComponent<BoxCollider2D>();

        for(int i=0; i<PLATE_CAPACITY; i++)
        {
            newColl[i] = Instantiate(newCollObject, transform.position, Quaternion.identity);
            newColl[i].GetComponent<MadeByPlatform>().madeByPlatform = gameObject;

            if (newColl[i]!=null)
                newColl[i].SetActive(false);
        }

    }
    private void Update()
    {
        Calculate_OverlapAreaX_Transparent();
        Calculate_OverlapAreaY_Transparent();
        for (int i = 0; i < PLATE_CAPACITY; i++)
        {
            if (otherTransPlatforms[i] == null)
            {
                newColl[i].transform.localScale = Vector2.zero;
                newColl[i].SetActive(false);
            }
        }

        //print(Calculate_FullLengthX_Transparent()[0]);
        //print(Calculate_OverlapAreaX_Transparent()[0] + " " + Calculate_OverlapAreaY_TransParent()[0]);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<TransparentPlatform>() != null)
        {
            for (int i = 0; i < PLATE_CAPACITY; i++)
            {
                if (otherTransPlatforms[i] == null)
                {
                    otherTransPlatforms[i] = collision.gameObject;
                    break;
                }
            }
        }  
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<TransparentPlatform>() != null)
        {
            for (int i = 0; i < PLATE_CAPACITY; i++)
            {
                if ((otherTransPlatforms[i] != null) && (collision.gameObject == otherTransPlatforms[i].gameObject))
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
                float newCollLeftX = 0f;
                float newCollRIghtX = 0f;
                otherPlatesLocalScale[i] = otherTransPlatforms[i].transform.localScale; // 크기 저장.

                if (otherTransPlatforms[i].GetComponent<Rigidbody2D>().velocity == Vector2.zero)
                    otherPlatesLocalScale[i] = new Vector2(otherTransPlatforms[i].transform.localScale.x * 7, otherTransPlatforms[i].transform.localScale.y * 7);

                if (gameObject.GetComponent<Rigidbody2D>().velocity == Vector2.zero)
                    myPlateLocalScale = new Vector2(transform.localScale.x * 7, transform.localScale.y * 7);

                float thisDiffX = GetComponent<BoxCollider2D>().size.x;
                float otherDiffX = otherTransPlatforms[i].GetComponent<BoxCollider2D>().size.x;

                float otherPlateRightX = otherTransPlatforms[i].transform.position.x + otherPlatesLocalScale[i].x * otherDiffX / 2;
                float otherPlateLeftX = otherTransPlatforms[i].transform.position.x - otherPlatesLocalScale[i].x * otherDiffX / 2;
                float thisPlateRightX = transform.position.x + myPlateLocalScale.x * thisDiffX / 2;
                float thisPlateLeftX = transform.position.x - myPlateLocalScale.x * thisDiffX / 2;
                //print(this.name + " " + transform.localScale.x + " " + transform.position.x + " " + thisPlateLeftX + " " + thisPlateRightX);
                if (otherPlateLeftX <= thisPlateRightX && otherPlateLeftX >= thisPlateLeftX && otherPlateRightX > thisPlateRightX)
                {
                    //print(gameObject.name + "  " + "Case1");
                    //realLengthX[i] = (otherTransPlatforms[i].transform.position.x + otherPlatesLocalScale[i].x / 2) - (transform.position.x - myPlateLocalScale.x / 2);
                    overlapSizeX[i] = Mathf.Abs(thisPlateRightX - otherPlateLeftX);
                    newCollLeftX = otherPlateLeftX;
                    newCollRIghtX = thisPlateRightX;
                    isOverlappedByX[i] = true;

                }
                else if (otherPlateRightX >= thisPlateLeftX && otherPlateRightX <= thisPlateRightX && otherPlateLeftX < thisPlateLeftX)
                {
                    //print(gameObject.name + "  " + "Case2");

                    //realLengthX[i] = (transform.position.x + myPlateLocalScale.x / 2) - (otherTransPlatforms[i].transform.position.x - otherPlatesLocalScale[i].x / 2);
                    overlapSizeX[i] = Mathf.Abs(otherPlateRightX - thisPlateLeftX);
                    newCollLeftX = thisPlateLeftX;
                    newCollRIghtX = otherPlateRightX;
                    isOverlappedByX[i] = true;
                }
                else if (otherPlateLeftX >= thisPlateLeftX && otherPlateRightX <= thisPlateRightX)
                {
                    //print(gameObject.name + "  " + "Case3");

                    //realLengthX[i] = otherPlatesLocalScale[i].x;
                    overlapSizeX[i] = otherPlatesLocalScale[i].x;
                    newCollLeftX = otherPlateLeftX;
                    newCollRIghtX = otherPlateRightX;
                    isOverlappedByX[i] = true;
                }
                else if (otherPlateLeftX <= thisPlateLeftX && otherPlateRightX >= thisPlateRightX)
                {
                    //print(gameObject.name + "  " + "Case4");

                    //realLengthX[i] = myPlateLocalScale.x;
                    overlapSizeX[i] = myPlateLocalScale.x;
                    newCollLeftX = thisPlateLeftX;
                    newCollRIghtX = thisPlateRightX;
                    isOverlappedByX[i] = true;
                }
                else
                {
                    isOverlappedByX[i] = false;
                }
                //print(isOverlappedByX[i]);
                //print(this.name + " " + i);
                if (isOverlappedByX[i] && newColl[i] != null)
                {
                    //print("X: " + newCollLeftX + " " + newCollRIghtX);
                    newColl[i].transform.position = new Vector2((newCollLeftX + newCollRIghtX) / 2, newColl[i].transform.position.y);
                    newColl[i].transform.localScale = new Vector2((newCollRIghtX - newCollLeftX), newColl[i].transform.localScale.y);
                    newColl[i].SetActive(true);
                }
                else
                {
                    newColl[i].SetActive(false);
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
                float newCollDownY = 0f;
                float newCollUpY = 0f;
                otherPlatesLocalScale[i] = otherTransPlatforms[i].transform.localScale;

                if (otherTransPlatforms[i].GetComponent<Rigidbody2D>().velocity == Vector2.zero)
                    otherPlatesLocalScale[i] = new Vector2(otherTransPlatforms[i].transform.localScale.x * 7, otherTransPlatforms[i].transform.localScale.y * 7);

                if (gameObject.GetComponent<Rigidbody2D>().velocity == Vector2.zero)
                    myPlateLocalScale = new Vector2(transform.localScale.x * 7, transform.localScale.y * 7);

                float thisDiffY = GetComponent<BoxCollider2D>().size.y;
                float otherDiffY = otherTransPlatforms[i].GetComponent<BoxCollider2D>().size.y;

                float otherPlayerUpY = otherTransPlatforms[i].transform.position.y + otherPlatesLocalScale[i].y * otherDiffY / 2;
                float otherPlayerDownY = otherTransPlatforms[i].transform.position.y - otherPlatesLocalScale[i].y * otherDiffY / 2;
                float thisPlateUpY = transform.position.y + myPlateLocalScale.y * thisDiffY / 2;
                float thisPlateDownY = transform.position.y - myPlateLocalScale.y * thisDiffY / 2;
                //print(this.name + " " + transform.localScale.y + " " + transform.position.y + " " + thisPlateDownY + " " + thisPlateUpY);
                //print(this.name);
                //print(otherPlayerDownY + " " + thisPlateUpY + " " + thisPlateDownY + " " + otherPlayerUpY);
                if ((otherPlayerDownY <= thisPlateUpY && thisPlateUpY <= otherPlayerUpY)
                    && (thisPlateDownY <= otherPlayerDownY && otherPlayerDownY <= thisPlateUpY))//otherPlayerDownY <= thisPlateUpY && otherPlayerDownY >= thisPlateDownY && otherPlayerUpY > thisPlateUpY)
                {
                    //print(gameObject.name + "  " + "Case1");
                    //realLengthX[i] = (otherTransPlatforms[i].transform.position.x + otherPlatesLocalScale[i].x / 2) - (transform.position.x - myPlateLocalScale.x / 2);
                    overlapSizeY[i] = Mathf.Abs(thisPlateUpY - otherPlayerDownY);
                    newCollDownY = otherPlayerDownY;
                    newCollUpY = thisPlateUpY;
                    isOverlappedByY[i] = true;
                }
                else if (otherPlayerUpY >= thisPlateDownY && otherPlayerUpY <= thisPlateUpY && otherPlayerDownY < thisPlateDownY)
                {
                    //print(gameObject.name + "  " + "Case2");

                    //realLengthX[i] = (transform.position.x + myPlateLocalScale.x / 2) - (otherTransPlatforms[i].transform.position.x - otherPlatesLocalScale[i].x / 2);
                    overlapSizeY[i] = Mathf.Abs(otherPlayerUpY - thisPlateDownY);
                    newCollDownY = thisPlateDownY;
                    newCollUpY = otherPlayerUpY;
                    isOverlappedByY[i] = true;
                }
                else if (otherPlayerDownY >= thisPlateDownY && otherPlayerUpY <= thisPlateUpY)
                {
                    //print(gameObject.name + "  " + "Case3");

                    //realLengthX[i] = otherPlatesLocalScale[i].x;
                    overlapSizeY[i] = otherPlatesLocalScale[i].y;
                    newCollDownY = otherPlayerDownY;
                    newCollUpY = otherPlayerUpY;
                    isOverlappedByY[i] = true;
                }
                else if (otherPlayerDownY <= thisPlateDownY && otherPlayerUpY >= thisPlateUpY)
                {
                    //print(gameObject.name + "  " + "Case4");

                    //realLengthX[i] = myPlateLocalScale.x;
                    overlapSizeY[i] = myPlateLocalScale.y;
                    newCollDownY = thisPlateDownY;
                    newCollUpY = thisPlateUpY;
                    isOverlappedByY[i] = true;
                }
                else
                {
                    isOverlappedByY[i] = false;
                }
                if (isOverlappedByY[i] && newColl[i] != null)
                {
                    //print("Y: " + newCollDownY + " " + newCollUpY);
                    newColl[i].transform.position = new Vector2(newColl[i].transform.position.x, (newCollDownY + newCollUpY) / 2);
                    newColl[i].transform.localScale = new Vector2(newColl[i].transform.localScale.x, (newCollUpY - newCollDownY));
                    newColl[i].SetActive(true);
                }
                else
                {
                    newColl[i].SetActive(false);
                }
            }
        }


        return overlapSizeY;
    }


}