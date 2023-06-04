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
    //���콺 ù Ŭ�� �� ���콺 ��ġ ����
    //���콺 �巡�׽� abs(ù ��ġ-���콺 �巡���� ��ŭ) �̵�
    //�� �߽��� ���콺 ��ġ�� �̵��� �ƴ� ��ġ�� ����������
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
