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
        //마우스 클릭 방향으로 움직이기
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 heading = mousePos - transform.position;
        Vector3 dir = heading / heading.magnitude;
        rb.velocity = dir * pictureMoveSpeed;


        //플레이어가 존재하면 같이 움직이기
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
