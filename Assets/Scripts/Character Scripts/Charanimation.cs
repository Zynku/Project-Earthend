using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charanimation : MonoBehaviour
{
    [SerializeField] internal Charinputcontrol inputScript;

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
        if (inputScript.right && !dead)
        {
            transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        }

        if (inputScript.left && !dead)
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
        if (isGrounded && inputScript.upDown && !dead)
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
        if (!isGrounded && inputScript.upDown)
        {
            animator.Play("Jump Transition");
        }
        //On Ground
        if (isGrounded)
        {
            animator.SetBool("Falling", false);
            animator.SetBool("Jumping", false);
        }

        //On Ground, Running, Sliding
        if (rb2d.velocity.x > 0.5f || rb2d.velocity.x < -0.5f)
        {
            if (isGrounded && inputScript.down)
            {
                animator.SetBool("Sliding", true);
            }

            if ((isGrounded && inputScript.downUp))
            {
                animator.SetBool("Sliding", false);
            }
        }
        else
        {
            animator.SetBool("Sliding", false);
        }
                
        //Not on ground, for any reason
        if (!isGrounded)
        {
            animator.SetBool("Sliding", false);
            animator.SetBool("Crouch", false);
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
            //If you hold both like some sort of maniac
            if (inputScript.left && inputScript.right)
            {
                animator.SetBool("Run", false);
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //Crouch States
        if (Mathf.Abs(rb2d.velocity.x) < 0.1f)
        {
            if (inputScript.down && isGrounded)
            {
                animator.SetBool("Crouch", true);
            }

            if (inputScript.downUp && isGrounded)
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
        animator.SetFloat("yVel", rb2d.velocity.y);
        animator.SetFloat("xVel", rb2d.velocity.x);
        animator.SetFloat("yVelAbs", Mathf.Abs(rb2d.velocity.y));
        animator.SetFloat("xVelAbs", Mathf.Abs(rb2d.velocity.x));

        if (inputScript.upAxis != 0 || inputScript.downAxis !=  0)
        {
            animator.SetFloat("verticalPressed", 1);
        }
        else
        {
            animator.SetFloat("verticalPressed", 0);
        }

        if (inputScript.leftAxis != 0 || inputScript.rightAxis != 0)
        {
            animator.SetFloat("horizontalPressed", 1);
        }
        else
        {
            animator.SetFloat("horizontalPressed", 0);
        }
    }
}




        

