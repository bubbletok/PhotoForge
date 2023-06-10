using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.TextCore.Text;
using UnityEngine;

public class PlatformMoving : MonoBehaviour
{
    [SerializeField] private GameObject parentPicture; // ���� ���� ������ ������Ʈ
    private PictureStatus parentPicStatus; // ���� ���� ������ �ڵ� 
    private GameObject overLappedPicture; // ���� ���� ������ ��ģ ���� 


    // �÷��� �̵� �� ����ϴ� ���� 
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

    float pictureX, pictureHalfWidth; // ���� �߽� x ��ǥ, ������ ���� ���ݱ��� 
    void MovingHorizon()
    {
        Transform pictureTrans = parentPicture.transform;
  
        float scale = transform.localScale.x * pictureTrans.localScale.x;

        if (!parentPicStatus.isOverlapped) // ���ǹ� �̻� ����. ��ġ�� �ʴ� ���  
        {
            pictureX = pictureTrans.position.x;
            pictureHalfWidth = pictureTrans.localScale.x * 0.5f;
        }
        else // ������ ��ġ�� ��� 
        {
            overLappedPicture = parentPicStatus.otherPic;
             
            // �߽� X��ǥ, �׸��� ������ ���� ���ݱ��� ���� �ʿ�
            pictureX = (pictureTrans.position.x + overLappedPicture.transform.position.x) / 2;
            pictureHalfWidth =(parentPicStatus.Calculate_FullLengthX() - parentPicStatus.Calculate_OverlapAreaX()) / 2;
        }

        // if ((transform.position.x + transform.localScale.x * 0.5) <= (pictureX + pictureHalfWidth))
        if ((transform.position.x + scale * 0.5f >= pictureX + pictureHalfWidth)) // ���� ������ ������ ������ ���� �ε�ģ ��� 
            direction = -1;
        else if (transform.position.x - scale * 0.5f <= pictureX - pictureHalfWidth) // ���� ���� ������ ���� ���� �ε�ģ ��� 
            direction = 1;

        rb.velocity = new Vector2(direction*horizonSpeed,0);       
    }

    void MovingVertical()
    {

    }



}
