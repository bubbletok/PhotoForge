using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    PlayerInput input;
    BoxCollider2D coll;
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sprite;

    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] LayerMask jumpableGround;

    float dirX;
    bool onMovingPicture;
    enum MovementState { idle, running, jumping, falling};
    // Start is called before the first frame update
    void Start()
    {
        speed = 8f;
        jumpForce = 6.5f;
        onMovingPicture = false;

        input = GetComponent<PlayerInput>();
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (onMovingPicture) return;
        dirX = input.horizontal;
        rb.velocity = new Vector2(dirX * speed, rb.velocity.y);
        if (input.jumped == 1 && isGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * input.jumped);
        }

        UpdateAnimaion();
    }

    void UpdateAnimaion()
    {
        MovementState state;
        if (dirX > 0f)
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        { 
        state = MovementState.running;
        sprite.flipX = true;
        }
        else
            state = MovementState.idle;

        if (rb.velocity.y > .001f)
            state = MovementState.jumping;
        else if (rb.velocity.y <= -.001f)
            state = MovementState.falling;

        anim.SetInteger("state", (int)state);
    }

    bool isGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .01f, jumpableGround);
    }

    public void setOnPicture(bool state)
    {
        onMovingPicture = state;
    }
}
