using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PictureMovement : MonoBehaviour
{
    [SerializeField] LayerMask limitArea;
    [SerializeField] float pictureMovementSpeed;
    [SerializeField] GameObject[] limitAreas;
    [SerializeField] GameObject flash;
    public GameObject alertOutline;
    public bool cantMove;

    Vector3 prevPicPos;
    Vector3 mousePos;
    Vector3 diffPicPos;
    Vector3 diifPlayerPos;
    Rigidbody2D rb;
    GameObject player;
    BoxCollider2D coll;

    Vector2 size;
    Vector2 dir;
    float dis;


    private void Awake()
    {
        pictureMovementSpeed = 8f;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();

        limitAreas = GameObject.FindGameObjectsWithTag("LimitArea");
    }

    private void LateUpdate()
    {
        checkLimitArea(-21.3f, 21.3f, -12.2f, 12.2f);
        if (cantMove)
        {
            rb.velocity = Vector2.zero;
        }
        else
        {
            alertOutline.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != "LimitArea") return;
        GameObject limitArea = collision.gameObject;
        float x1 = transform.position.x - transform.localScale.x * 1.81f / 2;
        float x2 = transform.position.x + transform.localScale.x * 1.81f / 2;
        float y1 = transform.position.y - transform.localScale.y / 2;
        float y2 = transform.position.y + transform.localScale.y / 2;

        float minX = limitArea.transform.position.x - limitArea.transform.localScale.x / 2;
        float maxX = limitArea.transform.position.x + limitArea.transform.localScale.x / 2;
        float minY = limitArea.transform.position.y - limitArea.transform.localScale.y / 2;
        float maxY = limitArea.transform.position.y + limitArea.transform.localScale.y / 2;
    }

    bool isOverlap(float min, float pos, float max)
    {
        return min <= pos && pos <= max;
    }

    void checkLimitArea(float minX, float maxX, float minY, float maxY)
    {
        checkLimitAreaX(minX, maxX); 
        checkLimitAreaY(minY, maxY);
    }

    void checkLimitAreaX(float minX, float maxX)
    {
        float x = transform.position.x;

        if (x - transform.localScale.x * 1.81f / 2 <= minX)
        {
            Vector3 padding = new Vector3(minX - x + (transform.localScale.x * 1.81f / 2) + .01f, 0, 0f);
            transform.position += padding;
            if(player != null)
                player.transform.position += padding;
            cantMove = true;
            rb.velocity = Vector3.zero;
        }
        else if (x + transform.localScale.x * 1.81f / 2 >= maxX)
        {
            Vector3 padding = new Vector3(x + (transform.localScale.x * 1.81f / 2) - maxX + .01f, 0, 0f);
            transform.position -= padding;
            if(player != null)
                player.transform.position -= padding;
            cantMove = true;
            rb.velocity = Vector3.zero;
        }
    }

    void checkLimitAreaY(float minY, float maxY)
    {
        float y = transform.position.y;
        if (y - transform.localScale.y / 2 <= minY)
        {
            Vector3 padding = new Vector3(0, minY - y + (transform.localScale.y / 2) + .01f, 0f);
            transform.position += padding;
            if(player != null) 
                player.transform.position += padding;
            cantMove = true;
            rb.velocity = Vector3.zero;
        }
        else if (y + transform.localScale.y / 2 >= maxY)
        {
            Vector3 padding = new Vector3(0, y + (transform.localScale.y / 2) - maxY + .01f, 0f);
            transform.position -= padding;
            if(player != null)
                player.transform.position -= padding;
            cantMove = true;
            rb.velocity = Vector3.zero;
        }
    }

    private void OnMouseDown()
    {
        cantMove = false;
        prevPicPos = transform.position;
        diffPicPos = transform.position - prevPicPos;
        if (player != null)
            diifPlayerPos = player.transform.position - transform.position;
        else
            diifPlayerPos = new Vector3(0, 0, 0);
    }

    public void getAwayFromLimitArea()
    {
        foreach (GameObject limitArea in limitAreas)
        {
            if (!limitArea.activeSelf) continue;
            if (limitArea == transform.GetChild(0).gameObject) continue;
            float x1 = transform.position.x - transform.localScale.x * 1.81f / 2;
            float x2 = transform.position.x + transform.localScale.x * 1.81f / 2;
            float y1 = transform.position.y - transform.localScale.y / 2;
            float y2 = transform.position.y + transform.localScale.y / 2;

            float minX = limitArea.transform.position.x - limitArea.transform.localScale.x / 2;
            float maxX = limitArea.transform.position.x + limitArea.transform.localScale.x / 2;
            float minY = limitArea.transform.position.y - limitArea.transform.localScale.y / 2;
            float maxY = limitArea.transform.position.y + limitArea.transform.localScale.y / 2;
            if (isOverlap(minX, x1, maxX) && isOverlap(minX, x2, maxX) && isOverlap(minY, y1, maxY) && isOverlap(minY, y2, maxY))
            {
                //flashWithCollisiion();
                transform.position += (transform.position - limitArea.transform.position) * 0.2f;
                if (isOverlap(minX, x1, maxX))
                {
                    //checkLimitAreaX(maxX, minX);
                    transform.position += new Vector3(maxX - x1 + 0.01f, 0f, 0f);
                }
                else if (isOverlap(minX, x2, maxX))
                {
                    //checkLimitAreaX(minX, maxX);
                    transform.position -= new Vector3(x2 - minX + 0.01f, 0f, 0f);
                }

                if (isOverlap(minY, y1, maxY))
                {
                    //checkLimitAreaY(maxY, minY);
                    transform.position += new Vector3(0f, maxY - y1 + 0.01f, 0f);
                }
                else if (isOverlap(minY, y2, maxY))
                {
                    //checkLimitAreaY(minY, maxY);
                    transform.position -= new Vector3(0f, y2 - minY + 0.01f, 0f);
                }
            }
        }
    }

    private void OnMouseDrag()
    {
        if (cantMove) return;
        moveWithVelocity();
        getAwayFromLimitArea();
    }

    void moveWithVelocity()
    {
        if (cantMove) return;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        prevPicPos = transform.position;

        size = new Vector2(coll.bounds.size.x, coll.bounds.size.y);
        dir = (mousePos - prevPicPos).normalized;
        pictureMovementSpeed = Vector3.Distance(mousePos, prevPicPos) * 2f;
        dis = 0.5f;

        RaycastHit2D[] hits = Physics2D.BoxCastAll(coll.bounds.center, size, 0f, dir, dis, limitArea);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit && hit.collider.gameObject != transform.GetChild(0).gameObject)
            {
                transform.position += (Vector3)dir * -1 * 2f;
                StartCoroutine(flashWithCollisiion());
                getAwayFromLimitArea();
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
            player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            player.transform.position = transform.position + diifPlayerPos;
        }
    }

    IEnumerator flashWithCollisiion()
    {
        SpriteRenderer sprite = flash.GetComponent<SpriteRenderer>();
        float elapsedTime = 0;
        float toTheFlashTime = 0.1f;
        float outTheFlashTime = 0.1f;
        //print(sprite.name + sprite.color.a + "FUCK");
        while (sprite.color.a < 1f)
        {
            elapsedTime += Time.deltaTime / toTheFlashTime;
            Color alpha = sprite.color;
            alpha.a = Mathf.Lerp(0, 1, elapsedTime);
            sprite.color = alpha;
            yield return null;
        }

        elapsedTime = 0;
        //flashSound.Play();
        while (sprite.color.a > 0f)
        {
            elapsedTime += Time.deltaTime / outTheFlashTime;
            Color alpha = sprite.color;
            alpha.a = Mathf.Lerp(1, 0, elapsedTime);
            sprite.color = alpha;
            yield return null;
        }
        yield return null;
    }

    void moveWithPosition()
    {
        //���콺 Ŭ�� �������� �����̱�
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //�����Ÿ� �̻� �����̸� ��ǥ ����
        if ((mousePos - prevPicPos).magnitude > 2f)
            prevPicPos = mousePos;

        //���콺 �̵� �������� �������� �� ���� ������ ħ���Ѵٸ�
        //�̵� ���ϱ�
        size = new Vector2(coll.bounds.size.x, coll.bounds.size.y);
        dir = (mousePos - prevPicPos).normalized;
        dis = 0.5f;

        /*        //����׿�
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

        //ħ�� �ߴ��� && ħ���� ������ �ڱ� �ڽ��� ������ �ƴ��� �Ǻ�
        //���� ������ LimitArea ������Ʈ�� �׻� 0��° �ε����� �־����
        //-> ���̾��Ű���� ���� ����
        StartCoroutine(waitToCollisionCheck());
        

        //�÷��̾ �����ϸ� ���� �����̱�
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
        //moveWithVelocity ����ϴ� ���
        rb.velocity = Vector3.zero;
    }
}
