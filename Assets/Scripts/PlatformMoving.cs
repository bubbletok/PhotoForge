using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlatformMoving : MonoBehaviour
{
    [SerializeField] private GameObject currentPicture; // ���� ���� ������ ������Ʈ. ���� parentPicture���� 
    private PictureStatus currentPicStatus; // ���� ���� ������ �ڵ� 
    private GameObject overLappedPicture; // ���� ���� ������ ��ģ ���� 
    private PictureStatus overLappedPicStatus;


    // �÷��� �̵� �� ����ϴ� ���� 
    [SerializeField] private int directionChoose = 0;
    [SerializeField] private float horizonSpeed = 5f;
    private int direction = 1;

    private Rigidbody2D thisRigid;

   
    // Start is called before the first frame update
    void Start()
    {
        thisRigid = GetComponent<Rigidbody2D>();
        currentPicture = GetComponentInParent<PictureStatus>().gameObject;
        currentPicStatus = currentPicture.GetComponent<PictureStatus>();
    }

    // Update is called once per frame
    void Update()
    {
/*        if (currentPicStatus.otherPic != null) // ���� ���� ���� ��ġ ���� ��ģ ������ �ִ� ��� 
            overLappedPicture = currentPicStatus.otherPic; // �� ��ģ ���� �ҷ����� */

        MovingFunction(directionChoose);
    }

    float pictureCenter, pictureHalfWidth; // ���� �߽� x ��ǥ, ������ ���� ���ݱ��� 

    void MovingFunction(int distinguisher)
    {
        Transform pictureTrans = currentPicture.transform;
  
        float scaleX = transform.localScale.x * pictureTrans.localScale.x;
        float scaleY = transform.localScale.y * pictureTrans.localScale.y;
        bool isInsideArea = false; // ��ģ ������ ������ ���ԵǴ��� �Ǻ��ϴ� bool

        if (currentPicStatus.otherPic != null) // ��ģ ������ �ִ� ���.
        {
            overLappedPicture = currentPicStatus.otherPic; 

            switch (distinguisher)
            {
                case 0: // ���� �̵� case
                    isInsideArea = ((transform.position.y + scaleY / 2) <= overLappedPicture.transform.position.y + overLappedPicture.transform.localScale.y / 2)
                                && ((transform.position.y - scaleY / 2) >= overLappedPicture.transform.position.y - overLappedPicture.transform.localScale.y / 2)
                                ? true : false; // ���� �̵��� ��� ��ģ ������ ���� ������ ���ԵǴ��� �Ǵ�.
                    break;
                case 1: // ���� �̵� case
                    isInsideArea = ((transform.position.x + scaleX / 2) <= overLappedPicture.transform.position.x + overLappedPicture.transform.localScale.x / 2)
                                && ((transform.position.x - scaleX / 2) >= overLappedPicture.transform.position.x - overLappedPicture.transform.localScale.x / 2)
                                ? true : false; // ���� �̵��� ��� ��ģ ������ ���� ������ ���ԵǴ��� �Ǵ�. 
                    break;
            }
        }

        if (!currentPicStatus.isOverlapped || !isInsideArea ) // ��ġ�� �ʴ� ��� �Ǵ� ������ �Ǻ����� �� ������ ��ģ ������ ���� �ȿ� ���� �ʴ��� �Ǵ�. ������ ���ƾ ���� �ȿ� ������ �̰� �����
        {
            switch (distinguisher)
            {
                case 0:
                    pictureCenter = pictureTrans.position.x; // ��ģ ���������� �߽� ��ǥ
                    pictureHalfWidth = pictureTrans.localScale.x * 0.5f; // ���� �ִ� ������ ���α��� ����
                    break;
                case 1:
                    pictureCenter = pictureTrans.position.y; 
                    pictureHalfWidth = pictureTrans.localScale.y * 0.5f; // ���α��� ���� 
                    break;
            }
        }
        else if(currentPicStatus.isOverlapped && isInsideArea) // ������ ��ġ�� ���� ������ ��ģ ������ ���� �ȿ� ���� ���� �� �����ؾ� ��.
        {
            switch (distinguisher)
            {
                case 0:
                    pictureCenter = (pictureTrans.position.x + overLappedPicture.transform.position.x) / 2; 
                    pictureHalfWidth = (currentPicStatus.Calculate_FullLengthX() - currentPicStatus.Calculate_OverlapAreaX()) / 2;
                    break;
                case 1:
                    pictureCenter = (pictureTrans.position.y + overLappedPicture.transform.position.y) / 2;
                    pictureHalfWidth = (currentPicStatus.Calculate_FullLengthY() - currentPicStatus.Calculate_OverlapAreaY()) / 2;
                    break;

            }
        }


        switch (distinguisher) // ������ �ε����� �� ���� �ٲ��ִ� ���� 
        {
            case 0:
                if ((transform.position.x + scaleX * 0.5f >= pictureCenter + pictureHalfWidth)) // ���� ������ ������ ������ ���� �ε�ģ ��� 
                {
                    direction = -1;
                }
                else if (transform.position.x - scaleX * 0.5f <= pictureCenter - pictureHalfWidth) // ���� ���� ������ ���� ���� �ε�ģ ���
                {
                    direction = 1;
                }
                thisRigid.velocity = new Vector2(direction * horizonSpeed, 0);
                break;
            case 1:
                if ((transform.position.y + scaleY * 0.5f >= pictureCenter + pictureHalfWidth)) 
                {
                    direction = -1;
                }
                else if (transform.position.y - scaleY * 0.5f <= pictureCenter - pictureHalfWidth) 
                {
                    direction = 1;
                }
                thisRigid.velocity = new Vector2(0, direction * horizonSpeed);
                break;
        }

        // if ((transform.position.x + transform.localScale.x * 0.5) <= (pictureX + pictureHalfWidth))
    }


    bool isInParentPic, isInOtherPic;
    void IsCrossing() // �� ������ ��� ���̿��� ������ �������� ��쿡 ���� ����ó��. ������ �� �����̰� �Ѵ�.
    {
        isInParentPic = currentPicStatus.isOverlapped;
        isInOtherPic = overLappedPicStatus.isOverlapped; // �̰� �ƴ�. 
        if(currentPicStatus.isOverlapped)
        {

        }
    }

    void Is_Plate_In_OverlappedPicture() // ������ �ٸ� �������� �Ѿ��, ������ ������ ��� �θ� ���Ӱ��� �����ϴ� �Լ� 
    {
        // ������ �ٸ� ���� ���� �ȿ� �ִ� ���� �����ؾ� ��.
        float[] lBottom = new float[2];  // ������ �װ��� ������.
        float[] rBottom = new float[2];
        float[] lTop = new float[2];
        float[] rTop = new float[2];

        Transform overlapPictureCenter = overLappedPicture.transform;
        float overlapPictureSizeX = overLappedPicture.transform.localScale.x;
        float overlapPictureSizeY = overLappedPicture.transform.localScale.y;

        lBottom[0] = transform.position.x - transform.localScale.x / 2;
        lBottom[1] = transform.position.y - transform.localScale.y / 2;

        rBottom[0] = transform.position.x + transform.localScale.x / 2;
        rBottom[1] = transform.position.y - transform.localScale.y / 2;

        lTop[0] = transform.position.x - transform.localScale.x / 2;
        lTop[1] = transform.position.y + transform.localScale.y / 2;

        rTop[0] = transform.position.x + transform.localScale.x / 2;
        rTop[1] = transform.position.y + transform.localScale.y / 2;

        float overlapPictureLeftX = overlapPictureCenter.position.x - overlapPictureSizeX / 2;

        float overlapPictureRightX = overlapPictureCenter.position.x + overlapPictureSizeX / 2;

        float overlapPictureTopY = overlapPictureCenter.position.y + overlapPictureSizeY / 2;

        float overlapPictureBottomY = overlapPictureCenter.position.y - overlapPictureSizeY / 2;


        // x�� ��� 



        // y�� ��� 



    }

    void Change_Hierarchy() // ������ �ٸ� �������� �Ѿ��, ������ ������ ��� �θ� ���Ӱ��� �����ϴ� �Լ� 
    {

    }
}
