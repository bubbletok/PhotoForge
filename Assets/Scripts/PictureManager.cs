using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureManager : MonoBehaviour
{
    Vector3 prevMousePos;
    Vector3 diffPos;
    void Update()
    {
    }
    //마우스 첫 클릭 시 마우스 위치 저장
    //마우스 드래그시 abs(첫 위치-마우스 드래그한 만큼) 이동
    //즉 중심이 마우스 위치로 이동이 아닌 위치가 더해져야함
    private void OnMouseDown()
    {
        prevMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        prevMousePos.z = 0;
        print(transform.position + " " + prevMousePos);
        diffPos = transform.position - prevMousePos;
        print(diffPos);
    }
    private void OnMouseDrag()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = mousePos + diffPos;
    }

}
