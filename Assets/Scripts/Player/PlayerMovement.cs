using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    PlayerInput input;
    BoxCollider2D coll;
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sprite;

    [SerializeField] float speed;
    [SerializeField] public float jumpForce;
    [SerializeField] LayerMask jumpableGround;

    float dirX;
    bool onMovingPicture;
    enum MovementState { idle, running, jumping, falling };
    // Start is called before the first frame update
    void Start()
    {
        //speed = 8f;
        //jumpForce = 6.5f;
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
        RaycastHit2D isGround = isGrounded();
        if (input.jumped == 1 && isGround)
        {
            if (isGround.transform.tag == "MovingPlatform" && isGround.transform.GetComponent<PlatformMoving>().directionChoose == 1)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(Vector2.up * jumpForce * 80);
            }
            else
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        UpdateAnimaion();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Platform")
        {
            //print("Enter Platform");
            if (collision.transform.position.y + collision.transform.localScale.y / 2 >= transform.position.y - transform.localScale.y * 1.957911 / 2)
                jumpForce = -1f;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Platform")
        {
            //print("Exit Platform");
            jumpForce = 5.5f;
        }
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

        if (input.jumped == 1)
        {
            if (rb.velocity.y > .001f)
                state = MovementState.jumping;
            else if (rb.velocity.y <= -.001f)
                state = MovementState.falling;
        }
        anim.SetInteger("state", (int)state);
    }
    RaycastHit2D isGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .2f, jumpableGround);
    }
    public void setOnPicture(bool state)
    {
        onMovingPicture = state;
    }
}
