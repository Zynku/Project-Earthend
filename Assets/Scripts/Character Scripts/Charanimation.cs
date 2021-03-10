using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charanimation : MonoBehaviour
{
    public bool isGrounded;
    public float xvel => rb2d.velocity.x;
    public float yvel => rb2d.velocity.y;

    Animator animator;
    Rigidbody2D rb2d;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //Checks for direction and plays flipped or unflipped anims
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        //Checks for groundcheck from charmovement script, returns true if true
        if (GetComponent<Charmovement_2>().isGrounded)
        {
            isGrounded = true;
            animator.SetBool("Grounded", true);
            animator.SetBool("Jumping", false);
        }
        else
        {
            isGrounded = false;
            animator.SetBool("Grounded", false);
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //Checks y velocities and plays anims
        //Rising
        if (rb2d.velocity.y > 0f && !isGrounded)
        {
            animator.SetBool("Jumping", true);
            animator.SetBool("Falling", false);
        }
        //Falling, not on ground
        if (rb2d.velocity.y < 0f && !isGrounded)
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", true);
            animator.SetBool("Double Jump", false);
        }
        //Falling, on ground
        if (Mathf.Ceil(rb2d.velocity.y) < 0 && isGrounded)
        {
            animator.SetBool("Falling", false);
            animator.SetBool("Double Jump", false);
        }
        //AirJumped, not on ground
        if (GetComponent<Charmovement_2>().airJumped && !isGrounded)
        {
            animator.SetBool("Double Jump", true);
        }
        //On Ground
        if (isGrounded)
        {
            animator.SetBool("Falling", false);
            animator.SetBool("Double Jump", false);
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //Go righttttt, play run anim
        if ((Input.GetKey("right")) && isGrounded)
        {
            animator.SetBool("Run", true);
        }
        if ((Input.GetKeyUp("right")))
            animator.SetBool("Run", false);
        //Go left, play run anim
        if ((Input.GetKey("left")) && isGrounded)
        {
            animator.SetBool("Run", true);
        }
        if ((Input.GetKeyUp("left")))
        {
            animator.SetBool("Run", false);
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //Melee Keys
        //Check Animation Transitions for Animation States
        if ((Input.GetKey("d")) && isGrounded)
        {
            animator.SetBool("Melee 1", true);
        }
        else animator.SetBool("Melee 1", false);
        //---------------------------------------------------------------------------------------------------------------------------------------------
    }
}