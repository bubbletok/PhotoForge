using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlatformMoving : MonoBehaviour
{
    [SerializeField] private GameObject currentPicture; // 현재 속한 사진의 오브젝트. 원래 parentPicture였음 
    private PictureStatus currentPicStatus; // 현재 속한 사진의 코드 
    private GameObject overLappedPicture; // 현재 속한 사진과 겹친 사진 
    private PictureStatus overLappedPicStatus;


    // 플랫폼 이동 시 사용하는 변수 
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
/*        if (currentPicStatus.otherPic != null) // 현재 속한 사진 위치 기준 겹친 사진이 있는 경우 
            overLappedPicture = currentPicStatus.otherPic; // 그 겹친 사진 불러오기 */

        MovingFunction(directionChoose);
    }

    float pictureCenter, pictureHalfWidth; // 사진 중심 x 좌표, 사진의 가로 절반길이 

    void MovingFunction(int distinguisher)
    {
        Transform pictureTrans = currentPicture.transform;
  
        float scaleX = transform.localScale.x * pictureTrans.localScale.x;
        float scaleY = transform.localScale.y * pictureTrans.localScale.y;
        bool isInsideArea = false; // 겹친 사진의 영역에 포함되는지 판별하는 bool

        if (currentPicStatus.otherPic != null) // 겹친 사진이 있는 경우.
        {
            overLappedPicture = currentPicStatus.otherPic; 

            switch (distinguisher)
            {
                case 0: // 가로 이동 case
                    isInsideArea = ((transform.position.y + scaleY / 2) <= overLappedPicture.transform.position.y + overLappedPicture.transform.localScale.y / 2)
                                && ((transform.position.y - scaleY / 2) >= overLappedPicture.transform.position.y - overLappedPicture.transform.localScale.y / 2)
                                ? true : false; // 가로 이동의 경우 겹친 사진의 세로 영역에 포함되는지 판단.
                    break;
                case 1: // 세로 이동 case
                    isInsideArea = ((transform.position.x + scaleX / 2) <= overLappedPicture.transform.position.x + overLappedPicture.transform.localScale.x / 2)
                                && ((transform.position.x - scaleX / 2) >= overLappedPicture.transform.position.x - overLappedPicture.transform.localScale.x / 2)
                                ? true : false; // 세로 이동의 경우 겹친 사진의 가로 영역에 포함되는지 판단. 
                    break;
            }
        }

        if (!currentPicStatus.isOverlapped || !isInsideArea ) // 겹치지 않는 경우 또는 위에서 판별했을 때 발판이 겹친 사진의 영역 안에 들어가지 않는지 판단. 사진이 겹쳤어도 영역 안에 없으면 이것 실행됨
        {
            switch (distinguisher)
            {
                case 0:
                    pictureCenter = pictureTrans.position.x; // 겹친 사진끼리의 중심 좌표
                    pictureHalfWidth = pictureTrans.localScale.x * 0.5f; // 원래 있던 사진의 가로길이 절반
                    break;
                case 1:
                    pictureCenter = pictureTrans.position.y; 
                    pictureHalfWidth = pictureTrans.localScale.y * 0.5f; // 세로길이 절반 
                    break;
            }
        }
        else if(currentPicStatus.isOverlapped && isInsideArea) // 사진이 겹치는 경우와 발판이 겹친 사진의 영역 안에 들어가는 조건 다 만족해야 함.
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


        switch (distinguisher) // 사진과 부딪쳤을 때 방향 바꿔주는 구문 
        {
            case 0:
                if ((transform.position.x + scaleX * 0.5f >= pictureCenter + pictureHalfWidth)) // 발판 오른쪽 끝단이 오른쪽 경계랑 부딪친 경우 
                {
                    direction = -1;
                }
                else if (transform.position.x - scaleX * 0.5f <= pictureCenter - pictureHalfWidth) // 발판 왼쪽 끝단이 왼쪽 경계랑 부딪친 경우
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
    void IsCrossing() // 두 사진의 경계 사이에서 발판이 지나가는 경우에 대한 예외처리. 사진을 못 움직이게 한다.
    {
        isInParentPic = currentPicStatus.isOverlapped;
        isInOtherPic = overLappedPicStatus.isOverlapped; // 이거 아님. 
        if(currentPicStatus.isOverlapped)
        {

        }
    }

    void Is_Plate_In_OverlappedPicture() // 발판이 다른 사진으로 넘어가고, 사진이 떼지는 경우 부모 종속관계 변경하는 함수 
    {
        // 발판이 다른 사진 영역 안에 있는 것을 판정해야 함.
        float[] lBottom = new float[2];  // 발판의 네가지 꼭짓점.
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


        // x의 경우 



        // y의 경우 



    }

    void Change_Hierarchy() // 발판이 다른 사진으로 넘어가고, 사진이 떼지는 경우 부모 종속관계 변경하는 함수 
    {

    }
}
