using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public float Gravity = 0.4f;
    public float velocityForce = 1000f;
    public float maxVelocity = 1f;
    float distance;
    Vector3 forceDirection;
    Rigidbody2D rb;
    public List<Rigidbody2D> rbs;
    private bool _enterGravity;

    public float RotationSpeed;
    private Quaternion _lookRotation;
    private Vector3 _direction;
    private void Start()
    {
        rb = GetComponent <Rigidbody2D>();
        rb.AddForce(transform.up * 10f);
        rb.velocity = new Vector2(maxVelocity, maxVelocity);
     }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        _enterGravity = true;
        if (collision.CompareTag("Player") && !rbs.Contains(collision.GetComponent<Rigidbody2D>()))
        {
            rbs.Add(collision.GetComponent<Rigidbody2D>());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        _enterGravity = false;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        _enterGravity = true;
    }
    private void Update()
    {
        
        //rb.AddForce(rb.velocity * Time.deltaTime * velocityForce);

        if (_enterGravity == true)
        {
            foreach (var rb in rbs)
            {
                distance = Vector3.Distance(transform.position, rb.transform.position);
                float GravityGradient = Gravity / distance * transform.localScale.x;
                forceDirection = (gameObject.transform.position - rb.transform.position).normalized;
                rb.AddForce(GravityGradient * forceDirection);
            }
            //Debug.Log(distance);
            //Debug.Log(GravityGradient);
        }
        
        

    }
    private void FixedUpdate()
    {
        Vector3 diff = GetComponent<DistanceJoint2D>().connectedBody.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(diff);
        Quaternion current = transform.localRotation;
        transform.Rotate(0,0,0.01f);
        //if (GetComponent<DistanceJoint2D>().connectedBody != null)
        //{

        //    Vector3 diff = GetComponent<DistanceJoint2D>().connectedBody.transform.position - transform.position;

        //    float rotationZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        //    transform.rotation = Quaternion.Euler(0, 0, rotationZ);

        //}
        //var connect = GetComponent<DistanceJoint2D>().connectedBody;
        //maxVelocity = maxVelocity + connect.velocity.magnitude;
        rb.velocity *= maxVelocity;
        //rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, maxVelocity);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
        if (rb.velocity.magnitude > 10)
            Debug.Log(rb.velocity.magnitude);
    }


}
