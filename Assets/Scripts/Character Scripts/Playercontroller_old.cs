using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playercontroller_old : MonoBehaviour
{
    public bool isGrounded;
    public float VectorForce;

    Animator animator;
    Rigidbody2D rb2d;

    [SerializeField]
    Transform groundCheck;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Creates Raycast between groundcheck object and ground
        if (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        //Go righttttt, play run anim
        if ((Input.GetKey("right")) && isGrounded)
        {
            rb2d.velocity = new Vector2(VectorForce, rb2d.velocity.y);
            animator.SetBool("Run", true);
        }
        if ((Input.GetKeyUp("right")))
            animator.SetBool("Run", false);
        //Go left, play run anim flipped
        if ((Input.GetKey("left")) && isGrounded)
        {
            rb2d.velocity = new Vector2(-VectorForce, rb2d.velocity.y);
            animator.SetBool("Run Mirror", true);
            animator.SetBool("Run", true);
        }
        if ((Input.GetKeyUp("left")))
        {
            animator.SetBool("Run Mirror", false);
            animator.SetBool("Run", false);
        }
    }

}
