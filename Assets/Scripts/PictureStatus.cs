using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TreeEditor;
using UnityEngine;

public class PictureStatus : MonoBehaviour
{
    public bool isOverlapped = false;

    private PictureStatus otherPicStatus;
    public GameObject otherPic;
    private Transform otherPicTrans;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Picture")
        {
            otherPic = other.gameObject; // ��ģ ���� ������Ʈ�� �����´�.
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.transform.tag == "Picture")
        {
            isOverlapped = true; 
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.transform.tag == "Picture")
        {
            isOverlapped = false;
            // ���� ������ �Ű��� ���, ������ �θ���� ���� �ʿ� 
        }

    }

    public float Calculate_FullLengthX() // ���� ������ ���α��̿� �ٸ� ������ ���α��̸� ��� ���� �Լ� 
    {
        return transform.localScale.x + otherPic.transform.localScale.x;
    }
    public float Calculate_OverlapAreaX() // ������ ��ġ�� ��� ��ġ�� X ���� ��� 
    {
        Transform otherPicTrans = otherPic.transform;
        float fullLengthX = Calculate_FullLengthX();
        float realLengthX;

        if (transform.position.x < otherPicTrans.position.x)
        {
            realLengthX = (otherPicTrans.position.x + otherPicTrans.localScale.x / 2) -
                            (transform.position.x - transform.localScale.x / 2);
        }
        else
        {
            realLengthX = (transform.position.x + transform.localScale.x / 2) -
                            (otherPicTrans.position.x - otherPicTrans.localScale.x / 2);
         }
        return fullLengthX - realLengthX;
    }


    public float Calculate_FullLengthY() // ���� ������ ���α��̿� �ٸ� ������ ���α��̸� ��� ���� �Լ� 
    {
        return transform.localScale.y + otherPic.transform.localScale.y;
    }
    public float Calculate_OverlapAreaY() // ������ ��ġ�� ��� ��ġ�� X ���� ��� 
    {
        Transform otherPicTrans = otherPic.transform;
        float fullLengthY = Calculate_FullLengthY();
        float realLengthY;

        if (transform.position.y < otherPicTrans.position.y)
        {
            realLengthY = (otherPicTrans.position.y + otherPicTrans.localScale.y / 2) -
                            (transform.position.y - transform.localScale.y / 2);
        }
        else
        {
            realLengthY = (transform.position.y + transform.localScale.y / 2) -
                            (otherPicTrans.position.y - otherPicTrans.localScale.y / 2);
        }
        return fullLengthY - realLengthY;
    }

    void Update()
    {
        if (isOverlapped)
        {
            print(Calculate_OverlapAreaX() + " " + Calculate_OverlapAreaY());
        }

    }
}
