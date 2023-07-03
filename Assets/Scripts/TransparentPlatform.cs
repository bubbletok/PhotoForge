using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class TransparentPlatform : MonoBehaviour
{
    GameObject[] otherTransPlatforms;
    public readonly int PLATE_CAPACITY = 5;

    BoxCollider2D myPlatformColl;
    Collider2D[] otherPlatformColls;


    private void Start()
    {
        otherTransPlatforms = new GameObject[PLATE_CAPACITY];
        myPlatformColl = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        Calculate_OverlapAreaX_Transparent();
        //print(Calculate_FullLengthX_Transparent()[0]);
        //print(Calculate_OverlapAreaX_Transparent()[0] + " " + Calculate_OverlapAreaY_TransParent()[0]);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<TransparentPlatform>() != null)
        {
            for (int i = 0; i < PLATE_CAPACITY; i++)
            {
                if (otherTransPlatforms[i] == null) // ����ִ� �����̳ʿ� ����, ontriggerenter���� �νĵ� ������ �߰���Ų��.
                { 
                    otherTransPlatforms[i] = collision.gameObject;
                    break;
                }
            }
        } // �̰� ���� flag���� �ε��ĵ� �����. �����ؾ� �� 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<TransparentPlatform>() != null)
        {
            for (int i = 0; i < PLATE_CAPACITY; i++) // ��ü �����ִ� ������ ���ؼ�,
            {
                if ((otherTransPlatforms[i] != null) && (collision.gameObject == otherTransPlatforms[i].gameObject)) // ���ƴٰ� �������� ���� �̸��� i��° �����̳ʿ� ����� ���� �̸��� ������ �Ǻ�.
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
                    fullLength[i] = transform.localScale.x + otherTransPlatforms[i].transform.localScale.x * 7;

                else if(gameObject.GetComponent<Rigidbody2D>().velocity == Vector2.zero)
                    fullLength[i] = transform.localScale.x * 7 + otherTransPlatforms[i].transform.localScale.x;

                else
                    fullLength[i] = transform.localScale.x + otherTransPlatforms[i].transform.localScale.x;
            }
            else // �������� ���� ��� 
                continue; // pass
        }

        return fullLength;
    }

    public float[] Calculate_FullLengthY_TransParent() // ���� ������ ���α��̿� �ٸ� ������ ���α��̸� ��� ���� �Լ� 
    {
        float[] fullLength = new float[PLATE_CAPACITY];
        for (int i = 0; i < PLATE_CAPACITY; ++i)
        {
            if (otherTransPlatforms[i] != null)
            {
                if (otherTransPlatforms[i].GetComponent<Rigidbody2D>().velocity == Vector2.zero)
                    fullLength[i] = transform.localScale.y + otherTransPlatforms[i].transform.localScale.y * 7;

                else if (gameObject.GetComponent<Rigidbody2D>().velocity == Vector2.zero)
                    fullLength[i] = transform.localScale.y * 7 + otherTransPlatforms[i].transform.localScale.y;

                else
                    fullLength[i] = transform.localScale.y + otherTransPlatforms[i].transform.localScale.y;
            }
            else
                continue;
        }
        return fullLength;
    }

    public float[] Calculate_OverlapAreaX_Transparent() // ������ ��ġ�� ��� ��ġ�� X ���� ��� 
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
                otherPlatesLocalScale[i] = otherTransPlatforms[i].transform.localScale; // ũ�� ����.

                if (otherTransPlatforms[i].GetComponent<Rigidbody2D>().velocity == Vector2.zero)
                    otherPlatesLocalScale[i] = new Vector2(otherTransPlatforms[i].transform.localScale.x * 7, otherTransPlatforms[i].transform.localScale.y * 7);

                if (gameObject.GetComponent<Rigidbody2D>().velocity == Vector2.zero)
                    myPlateLocalScale = new Vector2(transform.localScale.x * 7, transform.localScale.y * 7);


                float otherPlateRightX = otherTransPlatforms[i].transform.position.x + otherPlatesLocalScale[i].x / 2;
                float otherPlateLeftX = otherTransPlatforms[i].transform.position.x - otherPlatesLocalScale[i].x / 2;
                float thisPlateRightX = transform.position.x + myPlateLocalScale.x / 2;
                float thisPlateLeftX = transform.position.x - myPlateLocalScale.x / 2;

                if(otherPlateLeftX <= thisPlateRightX && otherPlateLeftX >= thisPlateLeftX && otherPlateRightX > thisPlateRightX) 
                {
                    print(gameObject.name + "  " + "Case1");
                    //realLengthX[i] = (otherTransPlatforms[i].transform.position.x + otherPlatesLocalScale[i].x / 2) - (transform.position.x - myPlateLocalScale.x / 2);
                    overlapSizeX[i] = Mathf.Abs(thisPlateRightX - otherPlateLeftX);
                }
                else if(otherPlateRightX >= thisPlateLeftX && otherPlateRightX <= thisPlateRightX && otherPlateLeftX < thisPlateLeftX)
                {
                    print(gameObject.name + "  " + "Case2");

                    //realLengthX[i] = (transform.position.x + myPlateLocalScale.x / 2) - (otherTransPlatforms[i].transform.position.x - otherPlatesLocalScale[i].x / 2);
                    overlapSizeX[i] = Mathf.Abs(otherPlateRightX - thisPlateLeftX);
                }
                else if(otherPlateLeftX >= thisPlateLeftX && otherPlateRightX <= thisPlateRightX)
                {
                    print(gameObject.name + "  " + "Case3");

                    //realLengthX[i] = otherPlatesLocalScale[i].x;
                    overlapSizeX[i] = otherPlatesLocalScale[i].x;
                }
                else if(otherPlateLeftX <= thisPlateLeftX && otherPlateRightX >= thisPlateRightX)
                {
                    print(gameObject.name + "  " + "Case4");

                    //realLengthX[i] = myPlateLocalScale.x;
                    overlapSizeX[i] = myPlateLocalScale.x;
                }
            }
        }

        return overlapSizeX;
    }


    public float[] Calculate_OverlapAreaY_TransParent() // ������ ��ġ�� ��� ��ġ�� Y���� ���  �̰� �̵��� ���� �ʿ� 
    {
        Transform[] otherPlatesTrans = new Transform[PLATE_CAPACITY];
        Vector2 myPlateLocalScale = transform.localScale;

        float[] fullLengthY = Calculate_FullLengthY_TransParent();
        float[] realLengthY = new float[PLATE_CAPACITY];
        float[] overlapSizeY = new float[PLATE_CAPACITY];

        for (int i = 0; i < PLATE_CAPACITY; i++)
        {
            if (otherTransPlatforms[i] != null)
            {
                otherPlatesTrans[i] = otherTransPlatforms[i].transform;

                if (otherTransPlatforms[i].GetComponent<Rigidbody2D>().velocity == Vector2.zero)
                    otherPlatesTrans[i].localScale = new Vector2(otherPlatesTrans[i].localScale.x * 7, otherPlatesTrans[i].localScale.y * 7);

                else if (gameObject.GetComponent<Rigidbody2D>().velocity == Vector2.zero)
                    myPlateLocalScale = new Vector2(transform.localScale.x * 7, transform.localScale.y * 7);


                if (transform.position.y < otherPlatesTrans[i].position.y)
                {
                    realLengthY[i] = (otherPlatesTrans[i].position.y + otherPlatesTrans[i].localScale.y / 2) -
                                    (transform.position.y - myPlateLocalScale.y / 2);
                }
                else
                {
                    realLengthY[i] = (transform.position.y + myPlateLocalScale.y / 2) -
                                    (otherPlatesTrans[i].position.y - otherPlatesTrans[i].localScale.y / 2);
                }
            }
            overlapSizeY[i] = fullLengthY[i] - realLengthY[i];
        }


        return overlapSizeY;
    }


}
