using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charanimation : MonoBehaviour
{
    public bool isGrounded;

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
        //Checks for groundcheck from charjump script, returns true if true
        if (GetComponent<Charjump>().isGrounded)
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
        if (rb2d.velocity.y > 0f && !isGrounded)
        {
            animator.SetBool("Jumping", true);
        }

        if (rb2d.velocity.y < 0f && !isGrounded)
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", true);
        }

        if (rb2d.velocity.y < -0.1f && isGrounded)
        {
            animator.SetBool("Landed", true);
            animator.SetBool("Falling", false);
        }
        else { animator.SetBool("Landed", false); }

        if (GetComponent<Charjump>().AirJump(true))
        {
            animator.Play("Jumping");

        }
        //---------------------------------------------------------------------------------------------------------------------------------------------



        //Go righttttt, play run anim
        if ((Input.GetKey("right")) && isGrounded)
        {
            animator.SetBool("Run", true);
            transform.localScale = new Vector3(1, 1, 1);
        }
        if ((Input.GetKeyUp("right")))
            animator.SetBool("Run", false);


        //Go left, play run anim flipped
        if ((Input.GetKey("left")) && isGrounded)
        {
            animator.SetBool("Run", true);
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if ((Input.GetKeyUp("left")))
        {
            animator.SetBool("Run", false);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------
        //Melee Keys
        if ((Input.GetKey("d")) && isGrounded)
        {
            animator.SetBool("Melee 1", true);
        }
        else animator.SetBool("Melee 1", false);
        //---------------------------------------------------------------------------------------------------------------------------------------------
    }
}