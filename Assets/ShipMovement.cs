using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float Speed = 4f;
    public float maxSpeed = 10f;
    public Rigidbody2D rb;
    Vector2 moveDirection;
    void Update()
    {
        moveDirection.x = Input.GetAxisRaw("Horizontal");
        moveDirection.y = Input.GetAxisRaw("Vertical");
    }
    private void FixedUpdate()
    {
        if(moveDirection.x == -1)
        {
            transform.rotation *= Quaternion.Euler(0, 0, 5f);
            rb.freezeRotation = true;
        }
        if (moveDirection.x == 1)
        {
            transform.rotation *= Quaternion.Euler(0, 0, -5f);
            rb.freezeRotation = true;
        }

        if (moveDirection.y == 1)
        {
            rb.AddForce(rb.transform.up * Speed * Time.fixedDeltaTime);
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
            //Debug.Log(rb.velocity);
        }
        rb.freezeRotation = false;
    }
}
