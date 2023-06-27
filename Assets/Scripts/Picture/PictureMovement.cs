using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PictureMovement : MonoBehaviour
{
    [SerializeField] LayerMask limitArea;
    [SerializeField] float pictureMovementSpeed;
    //[SerializeField] float pictureMoveSpeed = 5f;
    Vector3 originPicPos;
    Vector3 prevMousePos;
    Vector3 mousePos;
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
        pictureMovementSpeed = 8f;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        originPicPos = transform.position;
    }

    private void LateUpdate()
    {
/*        if (!cantMove)
        {
            RaycastHit2D[] hits = Physics2D.BoxCastAll(coll.bounds.center, size, 0f, dir, dis, limitArea);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit && hit.collider.gameObject != transform.GetChild(0).gameObject)
                {
                    transform.position = originPicPos;
                    if(player!=null)
                        player.transform.position = transform.position;
                    return;
                }
            }
        }*/
    }

    private void OnMouseDown()
    {
        cantMove = false;
        //prevMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        prevMousePos = transform.position;
        diffPicPos = transform.position - prevMousePos;
        if (player != null)
            diifPlayerPos = player.transform.position - transform.position;
        else
            diifPlayerPos = new Vector3(0, 0, 0);
    }

    private void OnMouseDrag()
    {
        if (cantMove) return;

        //위치 변환
        //moveWithPosition();

        //속도 변환
        moveWithVelocity();

        /*        float x1, y1, x2, y2;
                x1 = transform.position.x - transform.localScale.x / 2;
                x2 = transform.position.x + transform.localScale.x / 2;
                y1 = transform.position.y + transform.localScale.y / 2;
                y2 = transform.position.y - transform.localScale.y / 2;*/
        float x = transform.position.x, y = transform.position.y;
        float newX = transform.position.x, newY = transform.position.y;
        float minX = -20.8333333333333333f, maxX = 20.8333333333333333f;
        float minY = -11.5f, maxY = 11.5f;
        if (x < minX)
        {
            newX = minX + transform.localScale.x / 2;
            cantMove = true;
        }
        else if (x > maxX)
        {
            newX = maxX - transform.localScale.x / 2;
            cantMove = true;
        }
        else
        {
            cantMove = false;
        }
        if (y < minY)
        {
            newY = minY + transform.localScale.y / 2;
            cantMove = true;
        }
        else if (y > maxY)
        {
            newY = maxY - transform.localScale.y / 2;
            cantMove = true;
        }
        else
        {
            cantMove = false;
        }
        transform.position = new Vector3(newX, newY, 0f);
    }

    void moveWithVelocity()
    {
        if (cantMove) return;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        prevMousePos = transform.position;
        //일정거리 이상 움직이면 좌표 수정
        /*        if ((mousePos - prevMousePos).magnitude > 2f)
                {
                    prevMousePos = transform.position;
                    return;
                }*/

        size = new Vector2(coll.bounds.size.x, coll.bounds.size.y);
        dir = (mousePos - prevMousePos).normalized;
        pictureMovementSpeed = Vector3.Distance(mousePos, prevMousePos) * 2f;
        dis = 0.5f;

        RaycastHit2D[] hits = Physics2D.BoxCastAll(coll.bounds.center, size, 0f, dir, dis, limitArea);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit && hit.collider.gameObject != transform.GetChild(0).gameObject)
            {
                transform.position += (Vector3)dir * -1f;
                rb.velocity = Vector3.zero;
                cantMove = true;
                //print(hit.collider.name);
                return;
            }
        }


        //rb.AddForce(dir * pictureMovementSpeed);
        rb.velocity = dir * pictureMovementSpeed;

        if (player != null)
        {
            if (diifPlayerPos == new Vector3(0, 0, 0))
            {
                diifPlayerPos = player.transform.position - transform.position;
            }
            player.GetComponent<PlayerMovement>().setOnPicture(true);
            player.transform.position = transform.position + diifPlayerPos;
        }
    }

    void moveWithPosition()
    {
        //마우스 클릭 방향으로 움직이기
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //일정거리 이상 움직이면 좌표 수정
        if ((mousePos - prevMousePos).magnitude > 2f)
            prevMousePos = mousePos;

        //마우스 이동 방향으로 움직였을 때 제한 구역에 침범한다면
        //이동 안하기
        size = new Vector2(coll.bounds.size.x, coll.bounds.size.y);
        dir = (mousePos - prevMousePos).normalized;
        dis = 0.5f;

        /*        //디버그용
                Vector2 origin = transform.position;
                Vector2 topLeft = new Vector2(origin.x - size.x / 2, origin.y + size.y / 2) + dir * dis;
                Vector2 topRight = origin + size / 2 + dir * dis;
                Vector2 bottomLeft = origin - size / 2 + dir * dis;
                Vector2 bottomRight = new Vector2(origin.x + size.x / 2, origin.y - size.y / 2) + dir * dis;

                Debug.DrawLine(topLeft, topRight, Color.red);
                Debug.DrawLine(topRight, bottomRight, Color.red);
                Debug.DrawLine(bottomRight, bottomLeft, Color.red);
                Debug.DrawLine(bottomLeft, topLeft, Color.red);
                //*/

        //침범 했는지 && 침범한 구역이 자기 자식의 구역이 아닌지 판별
        //따라서 사진의 LimitArea 오브젝트는 항상 0번째 인덱스에 있어야함
        //-> 하이어라키에서 변경 금지
        StartCoroutine(waitToCollisionCheck());
        

        //플레이어가 존재하면 같이 움직이기
        if (player != null)
        {
            if (diifPlayerPos == new Vector3(0, 0, 0))
            {
                diifPlayerPos = player.transform.position - transform.position;
            }
            player.GetComponent<PlayerMovement>().setOnPicture(true);
            player.transform.position = transform.position + diifPlayerPos;
        }
    }
    
    IEnumerator waitToCollisionCheck()
    {
        yield return new WaitForSeconds(0.1f);
        RaycastHit2D[] hits = Physics2D.BoxCastAll(coll.bounds.center, size, 0f, dir, dis, limitArea);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit && hit.collider.gameObject != transform.GetChild(0).gameObject)
            {
                transform.position += (Vector3)dir * -0.8f;
                cantMove = true;
                //print(hit.collider.name);
                break;
            }
        }
        if(!cantMove)
            transform.position = diffPicPos + mousePos;
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
        if (player != null)
        {
            player.GetComponent<PlayerMovement>().setOnPicture(false);
        }
        cantMove = false;
        //moveWithVelocity 사용하는 경우
        rb.velocity = Vector3.zero;
    }
}
