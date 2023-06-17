using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PictureMovement : MonoBehaviour
{
    [SerializeField] LayerMask limitArea;
    [SerializeField] float pictureMoveSpeed = 5f;
    Vector3 prevMousePos;
    Vector3 diffPicPos;
    Vector3 diifPlayerPos;
    Rigidbody2D rb;
    GameObject player;
    BoxCollider2D coll;

    Vector2 size;
    Vector2 dir;
    float dis;
    bool cantMove;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
    }

    private void OnMouseDown()
    {
        prevMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        diffPicPos = transform.position - prevMousePos;
        if (player != null)
            diifPlayerPos = player.transform.position - transform.position;
        else
            diifPlayerPos = new Vector3(0, 0, 0);
    }

    private void OnMouseDrag()
    {
        if (cantMove) return;

        //마우스 클릭 방향으로 움직이기
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //일정거리 이상 움직이면 좌표 수정
        if ((mousePos - prevMousePos).magnitude > 2f)
            prevMousePos = mousePos;

        //마우스 이동 방향으로 움직였을 때 제한 구역에 침범한다면
        //이동 안하기
        size = new Vector2(coll.bounds.size.x, coll.bounds.size.y);
        dir = (mousePos - prevMousePos).normalized;
        print(dir);
        dis = 0.5f;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(coll.bounds.center, size, 0f, dir, dis, limitArea);

        //디버그용
        Vector2 origin = transform.position;
        Vector2 topLeft = new Vector2(origin.x - size.x / 2, origin.y + size.y / 2) + dir * dis;
        Vector2 topRight = origin + size / 2 + dir * dis;
        Vector2 bottomLeft = origin - size / 2 + dir * dis;
        Vector2 bottomRight = new Vector2(origin.x + size.x / 2, origin.y - size.y / 2) + dir * dis;

        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);
        //

        //침범 했는지 && 침범한 구역이 자기 자식의 구역이 아닌지 판별
        //따라서 사진의 LimitArea 오브젝트는 항상 0번째 인덱스에 있어야함
        //-> 하이어라키에서 변경 금지
        foreach (RaycastHit2D hit in hits)
        {
            if (hit && hit.collider.gameObject != transform.GetChild(0).gameObject)
            {
                transform.position += (Vector3)dir * -0.5f;
                cantMove = true;
                print(hit.collider.name);
                return;
            }
        }

        transform.position = diffPicPos + mousePos;

        //플레이어가 존재하면 같이 움직이기
        if (player != null)
        {
            if (diifPlayerPos == new Vector3(0, 0, 0))
            {
                diifPlayerPos = player.transform.position - transform.position;
            }
            player.GetComponent<PlayerMovement>().setState(true);
            player.transform.position = transform.position + diifPlayerPos;
        }
    }

/*    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector2 origin = transform.position;
        Vector2 castEnd = origin + (dir * dis);

        Gizmos.DrawWireCube(origin, size);
        Gizmos.DrawLine(origin, castEnd);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, size, 0f, dir, dis, limitArea);
        foreach (RaycastHit2D hit in hits)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hit.point, 0.1f);
        }
    }*/

    public void setPlayer(GameObject playerObject)
    {
        player = playerObject;
    }

    private void OnMouseUp()
    {
        rb.velocity = new Vector2(0, 0);
        cantMove = false;
    }
}
