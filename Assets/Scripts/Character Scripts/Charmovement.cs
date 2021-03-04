using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charmovement : MonoBehaviour
{
    public bool isGrounded;
    public float VectorForce;
    public float JumpForce;

    Rigidbody2D rb2d;

    [SerializeField]
    Transform groundCheck;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Creates Raycast between ground and grouncheck object
        if (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        if ((Input.GetKey("right")) /*&& isGrounded*/)
        {
            rb2d.velocity = new Vector2(VectorForce, rb2d.velocity.y);
        }
        if ((Input.GetKey("left")) /*&& isGrounded*/)
        {
            rb2d.velocity = new Vector2(-VectorForce, rb2d.velocity.y);
        }
        if ((Input.GetKey("up") && isGrounded))
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, JumpForce);
        }
    }
}
