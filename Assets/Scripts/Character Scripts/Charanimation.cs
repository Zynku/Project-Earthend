using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charanimation : MonoBehaviour
{
    public bool isGrounded;
    public float xvel;
    public float yvel;



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
        yvel = rb2d.velocity.y;
        xvel = rb2d.velocity.x;

        

        //---------------------------------------------------------------------------------------------------------------------------------------------
        //Checks for direction and plays flipped or unflipped anims
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        }

        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            transform.localScale = new Vector3(-0.9f, 0.9f, 0.9f);
        }

        //Checks for groundcheck from charmovement script, returns true if true
        if (GetComponent<Char_control>().isGrounded)
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
        if (GetComponent<Char_control>().airJumped && !isGrounded)
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
        //Go righttttt or left, play run anim
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
        //Crouch States
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //animator.SetBool("Crouching", true);
            animator.SetTrigger("Crouching");
            animator.Play("Low Poly Girl Into Crouch HD3");
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //Melee Keys
        //If attack key is pressed and on ground, play first melee anim, sets trigger
        
        //---------------------------------------------------------------------------------------------------------------------------------------------

        animator.SetFloat("yVel", Mathf.Clamp(rb2d.velocity.y, -1, 1));
        animator.SetFloat("verticalPressed", Input.GetAxis("Vertical"));
        animator.SetFloat("horizontalPressed", Mathf.Abs(Input.GetAxis("Horizontal")));
    }
}