using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class TransparentPlatform : MonoBehaviour
{
    // Start is called before the first frame update
    BoxCollider2D coll;
    bool collWithTransparent;
    private void Start()
    {
        collWithTransparent = false;
        coll = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        collWithTransparent = false;
        Collider2D[] colls = Physics2D.OverlapBoxAll(transform.position, coll.bounds.size, 1f);
        foreach (Collider2D coll2d in colls)
        {
            if (coll2d && coll2d.gameObject.GetComponent<TransparentPlatform>() != null && coll2d.gameObject != gameObject)
            {
                print(coll2d.gameObject.name);
                collWithTransparent = true;
                break;
            }
        }
        if (collWithTransparent)
        {
            UnityEngine.Color myColor = GetComponent<SpriteRenderer>().color;
            myColor.a = 1f;
            GetComponent<BoxCollider2D>().isTrigger = false;
        }
        else
        {
            UnityEngine.Color myColor = GetComponent<SpriteRenderer>().color;
            myColor.a = .5f;
            GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }

/*    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<TransparentPlatform>() != null)
        {
            Color myColor = GetComponent<SpriteRenderer>().color;
            myColor.a = 1f;
            GetComponent<BoxCollider2D>().enabled = true;
            Color otherColor = collision.gameObject.GetComponent<SpriteRenderer>().color;
            otherColor.a = 1f;
            collision.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<TransparentPlatform>() != null)
        {
            Color myColor = GetComponent<SpriteRenderer>().color;
            myColor.a = .5f;
            GetComponent<BoxCollider2D>().enabled = false;
            Color otherColor = collision.gameObject.GetComponent<SpriteRenderer>().color;
            otherColor.a = .5f;
            collision.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }*/
}
