using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerInput input;
    public float speed = 5f;
    public float jumpHight;
    public bool jumped;

    Rigidbody rb;
    private void Start()
    {
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        jumped = false;
    }

    private void Update()
    {
        
    }

    // Start is called before the first frame update
    private void FixedUpdate()
    {
        if (input.jumped == 1.0 && !jumped)
        {
            jumpHight = 200;
            jumped = true;
        }
        else
        {
            jumpHight = 0;
        }
        Vector3 movement = transform.right * input.horizontal;

        transform.Translate(movement * speed * Time.deltaTime);
        rb.AddForce(transform.up * jumpHight, ForceMode.Force);
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "ground")
        {
            jumped = false;
        }
    }
}
