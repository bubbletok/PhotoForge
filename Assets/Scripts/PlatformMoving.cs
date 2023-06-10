using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.TextCore.Text;
using UnityEngine;

public class PlatformMoving : MonoBehaviour
{
    [SerializeField] private GameObject parentPicture; // 현재 속한 사진의 오브젝트
    private PictureStatus parentPicStatus; // 현재 속한 사진의 코드 
    private GameObject overLappedPicture; // 현재 속한 사진과 겹친 사진 


    // 플랫폼 이동 시 사용하는 변수 
    [SerializeField] private int directionChoose = 0;
    [SerializeField] private float horizonSpeed = 5f;
    private int direction = 1;

    private Rigidbody2D rb;

   
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        parentPicStatus = parentPicture.GetComponent<PictureStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (directionChoose)
        {
            case 0:
                MovingHorizon();
                break;
            case 1:
                MovingVertical();
                break;
        }
    }

    float pictureX, pictureHalfWidth; // 사진 중심 x 좌표, 사진의 가로 절반길이 
    void MovingHorizon()
    {
        Transform pictureTrans = parentPicture.transform;
  
        float scale = transform.localScale.x * pictureTrans.localScale.x;

        if (!parentPicStatus.isOverlapped) // 조건문 이상 없음. 겹치지 않는 경우  
        {
            pictureX = pictureTrans.position.x;
            pictureHalfWidth = pictureTrans.localScale.x * 0.5f;
        }
        else // 사진이 겹치는 경우 
        {
            overLappedPicture = parentPicStatus.otherPic;
             
            // 중심 X좌표, 그리고 사진의 가로 절반길이 수정 필요
            pictureX = (pictureTrans.position.x + overLappedPicture.transform.position.x) / 2;
            pictureHalfWidth =(parentPicStatus.Calculate_FullLengthX() - parentPicStatus.Calculate_OverlapAreaX()) / 2;
        }

        // if ((transform.position.x + transform.localScale.x * 0.5) <= (pictureX + pictureHalfWidth))
        if ((transform.position.x + scale * 0.5f >= pictureX + pictureHalfWidth)) // 발판 오른쪽 끝단이 오른쪽 경계랑 부딪친 경우 
            direction = -1;
        else if (transform.position.x - scale * 0.5f <= pictureX - pictureHalfWidth) // 발판 왼쪽 끝단이 왼쪽 경계랑 부딪친 경우 
            direction = 1;

        rb.velocity = new Vector2(direction*horizonSpeed,0);       
    }

    void MovingVertical()
    {

    }



}
