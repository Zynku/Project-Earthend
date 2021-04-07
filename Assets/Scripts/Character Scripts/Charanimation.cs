using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charanimation : MonoBehaviour
{
    public bool isGrounded;
    public float xvel;
    public float yvel;
    public int airJumpsHas;
    public bool dead;



    Animator animator;
    Rigidbody2D rb2d;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        dead = GetComponent<Char_control>().dead;


    }

    // Update is called once per frame
    void Update()
    {
        yvel = rb2d.velocity.y;
        xvel = rb2d.velocity.x;




        //---------------------------------------------------------------------------------------------------------------------------------------------
        //Checks for direction and plays flipped or unflipped anims
        if (Input.GetAxisRaw("Horizontal") > 0 && !dead)
        {
            transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        }

        if (Input.GetAxisRaw("Horizontal") < 0 && !dead)
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
        if (isGrounded && Input.GetKeyDown(KeyCode.UpArrow) && !dead)
        {
            animator.Play("Jump Transition");
        }
        //Rising
        if (rb2d.velocity.y > 0f && !isGrounded && !dead)
        {
            animator.SetBool("Jumping", true);
            animator.SetBool("Falling", false);
        }
        //Falling, not on ground
        if (rb2d.velocity.y < 0f && !isGrounded)
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", true);

        }
        //Falling, on ground
        if (Mathf.Ceil(rb2d.velocity.y) < 0 && isGrounded)
        {
            animator.SetBool("Falling", false);
        }
        //AirJumped, not on ground
        if (!isGrounded && Input.GetKeyDown(KeyCode.UpArrow))
        {
            animator.Play("Jump Transition");
        }
        //On Ground
        if (isGrounded)
        {
            animator.SetBool("Falling", false);
        }
        //Not on ground for any reason
        if (!isGrounded)
        {
            animator.SetBool("Run", false);
        }

        //On Ground, Sliding
        if (Mathf.Abs(rb2d.velocity.x) > 0.5f)
        {
            if (isGrounded && Input.GetKey(KeyCode.DownArrow))
            {
                animator.SetBool("Sliding", true);
            }

            if ((isGrounded && Input.GetKeyUp(KeyCode.DownArrow)))
            {
                animator.SetBool("Sliding", false);
            } 
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------
        //Go righttttt or left, play run anim
        if (!animator.GetBool("Sliding"))
        {
            if (Input.GetKey("right"))
            {
                animator.SetBool("Run", true);
            }
            if (Input.GetKeyUp("right"))
            {
                animator.SetBool("Run", false);
            }
            //Go left, play run anim
            if (Input.GetKey("left"))
            {
                animator.SetBool("Run", true);
            }
            if (Input.GetKeyUp("left"))
            {
                animator.SetBool("Run", false);
            }
            //If you hold both like some sort of madman
            if (Input.GetKey("left") && (Input.GetKey("right")))
            {
                animator.SetBool("Run", false);
            }
            //If not holding anything
            if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            {
                animator.SetBool("Run", false);
                animator.SetBool("Crouch", false);
                animator.SetBool("Sliding", false);
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //Crouch States
        if (Mathf.Abs(rb2d.velocity.x) < 0.1f)
        {
            if (Input.GetKey(KeyCode.DownArrow) && isGrounded)
            {
                animator.SetBool("Crouch", true);
            }

            if (Input.GetKeyUp(KeyCode.DownArrow) && isGrounded)
            {
                animator.SetBool("Crouch", false);
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //Deadass lmao
        if (GetComponent<Charhealth>().currentHealth <= 0)
        {
            animator.Play("Low Poly Death HD3");
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------
        animator.SetFloat("yVel", Mathf.Clamp(rb2d.velocity.y, -1, 1));
        animator.SetFloat("xVel", Mathf.Clamp(rb2d.velocity.x, -1, 1));
        animator.SetFloat("yVelAbs", Mathf.Abs(Mathf.Clamp(rb2d.velocity.y, -1, 1)));
        animator.SetFloat("xVelAbs", Mathf.Abs(Mathf.Clamp(rb2d.velocity.x, -1, 1)));
        animator.SetFloat("verticalPressed", Input.GetAxis("Vertical"));
        animator.SetFloat("horizontalPressed", Mathf.Abs(Input.GetAxis("Horizontal")));
        airJumpsHas = GetComponent<Char_control>().airJumpshas;
    }
}




        

