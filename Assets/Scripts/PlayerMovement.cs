using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float jumpForce;

    [SerializeField] LayerMask jumpableGround;

    bool onMovingPicture;
    PlayerInput input;
    BoxCollider2D coll;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        speed = 8f;
        jumpForce = 10f;
        onMovingPicture = false;
        input = GetComponent<PlayerInput>();
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (onMovingPicture) return;
        float dirX = input.horizontal;
        rb.velocity = new Vector2(dirX * speed, rb.velocity.y);

        if (input.jumped == 1 && isGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    bool isGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .01f, jumpableGround);
    }

    public void setState(bool state)
    {
        onMovingPicture = state;
    }
}
