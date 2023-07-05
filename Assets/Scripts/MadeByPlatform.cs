using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MadeByPlatform : MonoBehaviour
{
    public GameObject madeByPlatform;
    BoxCollider2D coll;

    float diffDisY = 0;

    private void Start()
    {
        coll = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        Collider2D[] colls = Physics2D.OverlapBoxAll(transform.position, coll.size * coll.transform.localScale + new Vector2(0,0.2f), 1f);
        foreach (Collider2D hit in colls)
        {
/*            if(hit && hit.tag == "Player")
                print(hit.name);*/
            if (hit.tag == "Player")
            {
                if(diffDisY == 0)
                {
                    diffDisY = hit.transform.position.y - transform.position.y;
                }
                if (hit.gameObject.GetComponent<PlayerInput>().jumped != 1)
                {
                    print("∞Ì¡§µ ");
                    hit.transform.position = new Vector2(hit.transform.position.x, diffDisY + transform.position.y);
                    hit.GetComponent<Rigidbody2D>().velocity = new Vector2(hit.GetComponent<Rigidbody2D>().velocity.x, 0);
                    if(hit.gameObject.GetComponent<PlayerInput>().horizontal == 0)
                        hit.GetComponent<PlayerMovement>().anim.SetInteger("state", 0);
                }
                break;
            }
        }
    }
/*    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            print("Enter");
            diffDisY = collision.transform.position.y - transform.position.y;
        }
    }*/


/*    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (collision.gameObject.GetComponent<PlayerInput>().jumped != 1)
            {
                collision.transform.position = new Vector2(collision.transform.position.x, diffDisY + transform.position.y);
            }
        }
    }*/
}
