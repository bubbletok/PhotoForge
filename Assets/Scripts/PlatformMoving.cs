using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlatformMoving : MonoBehaviour
{
    [SerializeField] private GameObject parentPicture;

    [SerializeField] private int directionChoose = 0;
    [SerializeField] private float horizonSpeed = 5f;
    private int direction = 1;


    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (directionChoose == 0) // �����̵� ������ directionChoose 0�϶� �۵�.
            MovingHorizon();
        else if (directionChoose == 1) // �����̵� ������ directionChoose 1�϶� �۵� 
            MovingVertical();
    }

    void MovingHorizon()
    {
        Transform pictureTrans = parentPicture.transform;
        float pictureX = pictureTrans.position.x;
        float pictureHalfWidth = pictureTrans.localScale.x * 0.5f;

        float scale = transform.localScale.x * pictureTrans.localScale.x;

        // if ((transform.position.x + transform.localScale.x * 0.5) <= (pictureX + pictureHalfWidth))
        if ((transform.position.x + scale * 0.5f >= pictureX + pictureHalfWidth)) // ���� ���, ������ ���� �ε�ģ ��� 
            direction = -1;
        else if (transform.position.x - scale * 0.5f <= pictureX - pictureHalfWidth)
            direction = 1;

        rb.velocity = new Vector2(direction*horizonSpeed,0);       

    }

    void MovingVertical()
    {

    }
}
