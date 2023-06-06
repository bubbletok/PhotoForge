using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureMovement : MonoBehaviour
{
    [SerializeField] float pictureMoveSpeed = 5f;
    Vector3 prevPos;
    Vector3 diffPos;
    Rigidbody2D rb;
    GameObject player;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnMouseDrag()
    {
        //���콺 Ŭ�� �������� �����̱�
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 heading = mousePos - transform.position;
        Vector3 dir = heading / heading.magnitude;
        rb.velocity = dir * pictureMoveSpeed;


        //�÷��̾ �����ϸ� ���� �����̱�
        if (player != null)
        {
            diffPos = transform.position - player.transform.position;
            player.transform.position = transform.position + diffPos;
        }
    }

    public void setPlayer(GameObject playerObject)
    {
        player = playerObject;
    }

    private void OnMouseUp()
    {
        rb.velocity = new Vector2(0, 0);
    }
}
