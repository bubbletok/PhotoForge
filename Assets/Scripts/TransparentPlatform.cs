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
                collWithTransparent = true;
                break;
            }
        }
        if (collWithTransparent)
        {
            UnityEngine.Color myColor = GetComponent<SpriteRenderer>().color;
            myColor.a = 1f;
            GetComponent<SpriteRenderer>().color = myColor;
        }
        else
        {
            UnityEngine.Color myColor = GetComponent<SpriteRenderer>().color;
            myColor.a = .5f;
            GetComponent<SpriteRenderer>().color = myColor;
            
        }
    }

    void calcArea()
    {

    }

    bool isOverlap(float min, float pos, float max)
    {
        return min <= pos && pos <= max;
    }
}
