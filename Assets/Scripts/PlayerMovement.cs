using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 8f;
    [SerializeField] float jumpForce = 10f;

    [SerializeField] LayerMask jumpableGround;
    PlayerInput input;
    BoxCollider2D coll;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInput>();
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        float dirX = input.horizontal;
        rb.velocity = new Vector2(dirX * speed,rb.velocity.y);
    
        if (input.jumped && isGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    bool isGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);    }
}
