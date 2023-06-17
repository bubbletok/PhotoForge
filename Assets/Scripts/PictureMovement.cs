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

        //���콺 Ŭ�� �������� �����̱�
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //�����Ÿ� �̻� �����̸� ��ǥ ����
        if ((mousePos - prevMousePos).magnitude > 2f)
            prevMousePos = mousePos;

        //���콺 �̵� �������� �������� �� ���� ������ ħ���Ѵٸ�
        //�̵� ���ϱ�
        size = new Vector2(coll.bounds.size.x, coll.bounds.size.y);
        dir = (mousePos - prevMousePos).normalized;
        print(dir);
        dis = 0.5f;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(coll.bounds.center, size, 0f, dir, dis, limitArea);

        //����׿�
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

        //ħ�� �ߴ��� && ħ���� ������ �ڱ� �ڽ��� ������ �ƴ��� �Ǻ�
        //���� ������ LimitArea ������Ʈ�� �׻� 0��° �ε����� �־����
        //-> ���̾��Ű���� ���� ����
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

        //�÷��̾ �����ϸ� ���� �����̱�
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
