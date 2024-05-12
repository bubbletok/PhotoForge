using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    PlayerInput input;
    BoxCollider2D coll;
    Rigidbody2D rb;
    public Animator anim;
    SpriteRenderer sprite;

    [SerializeField] float speed;
    [SerializeField] public float jumpForce;
    [SerializeField] LayerMask jumpableGround;

    float dirX;
    float diffDisY;
    bool onMovingPicture;
    enum MovementState { idle, running, jumping, falling };
    void Start()
    {
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
        RaycastHit2D[] isGrounds = isGrounded();
        foreach (RaycastHit2D isGround in isGrounds)
        {
            if (isGround && isGround.transform.GetComponent<BoxCollider2D>().isTrigger == false)
            {
                if (isGround.collider.name == "TransparentNewColl(Clone)")
                {
                    transform.position += new Vector3(0, 0.01f, 0);
                }
                else
                {
                    transform.position += new Vector3(0, 0.0001f, 0);
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                }
            }
            if (input.jumped == 1 && isGround && isGround.transform.GetComponent<BoxCollider2D>().isTrigger == false)
            {
                if (isGround.transform.tag == "MovingPlatform" && isGround.transform.GetComponent<PlatformMoving>().directionChoose == 1)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    rb.AddForce(Vector2.up * jumpForce * 80);
                }
                else if((isGround.transform.GetComponent<MadeByPlatform>() != null && isGround.transform.GetComponent<MadeByPlatform>().madeByPlatform.GetComponent<PlatformMoving>() != null
                    && isGround.transform.GetComponent<MadeByPlatform>().madeByPlatform.GetComponent<PlatformMoving>().directionChoose == 1))
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    rb.AddForce(Vector2.up * jumpForce * 20);
                }
                else
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
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
        if (collision.collider.GetComponent<PlatformMoving>() != null)
        {
            if (collision.collider.GetComponent<PlatformMoving>().directionChoose == 1)
            {
                diffDisY = transform.position.y - coll.transform.position.y;
            }
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
        {
            if (rb.velocity.y > .1f)
                state = MovementState.jumping;
            else if (rb.velocity.y <= -.1f)
                state = MovementState.falling;
            else
            {
                state = MovementState.idle;
            }
        }

        anim.SetInteger("state", (int)state);
    }
    RaycastHit2D[] isGrounded()
    {
        return Physics2D.BoxCastAll(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
    public void setOnPicture(bool state)
    {
        onMovingPicture = state;
    }
}