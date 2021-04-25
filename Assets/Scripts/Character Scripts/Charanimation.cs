using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charanimation : MonoBehaviour
{
    public string currentState;
    public float xvel;
    public float yvel;
    public bool isGrounded;
    private bool jumped;

    Animator animator;
    Rigidbody2D rb2d;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }


    // Update is called once per frame
    // Old animation code.
    /*void Update()
    {
        //Charcontrol.State currentState = Charcontrol.currentState;
        //if(currentState == Charcontrol.State.Attacking) { }

        
        grounded = Charcontrol.isGrounded;
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
        if (grounded)
        {
            animator.SetBool("Grounded", true);
            animator.SetBool("Jumping", false);
        }
        else
        {
            animator.SetBool("Grounded", false);
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------
        //Checks y velocities and plays anims
        if (grounded && Input.GetKeyDown(KeyCode.UpArrow) && !dead)
        {
            animator.Play("Jump Transition");
        }
        //Rising
        if (rb2d.velocity.y > 0f && !grounded && !dead)
        {
            animator.SetBool("Jumping", true);
            animator.SetBool("Falling", false);
        }
        //Falling, not on ground
        if (rb2d.velocity.y < fallThreshold && !grounded)
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", true);
            animator.Play("Low Poly Girl Whole Fall HD3");

        }
        //Falling, on ground
        if (Mathf.Ceil(rb2d.velocity.y) < 0 && grounded)
        {
            animator.SetBool("Falling", false);
        }
        //AirJumped, not on ground
        if (!grounded && Input.GetKeyDown(KeyCode.UpArrow))
        {
            animator.Play("Jump Transition");
        }
        //On Ground
        if (grounded)
        {
            animator.SetBool("Falling", false);
        }
        //Not on ground for any reason
        if (!grounded)
        {
            animator.SetBool("Run", false);
        }

        //On Ground, Sliding
        if (Mathf.Abs(rb2d.velocity.x) > 0.5f)
        {
            if (grounded && Input.GetKey(KeyCode.DownArrow))
            {
                animator.SetBool("Sliding", true);
            }

            if ((grounded && Input.GetKeyUp(KeyCode.DownArrow)))
            {
                animator.SetBool("Sliding", false);
            }
        }
        //Not moving
        if (Mathf.Abs(rb2d.velocity.x) == 0f)
        {
            animator.SetBool("Sliding", false);
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
            if (Input.GetKey(KeyCode.DownArrow) && grounded)
            {
                animator.SetBool("Crouch", true);
            }

            if (Input.GetKeyUp(KeyCode.DownArrow) && grounded)
            {
                animator.SetBool("Crouch", false);
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

            yvel = rb2d.velocity.y;
            xvel = rb2d.velocity.x;

            
        }
    }*/

    private void FixedUpdate()
    {
        switch (Charcontrol.Instance.currentState)
        {
            case Charcontrol.State.Idle:
                animator.SetBool("Run", false);
                animator.SetBool("Crouch", false);
                animator.SetBool("Sliding", false);
                animator.SetBool("Jumping", false);
                break;

            case Charcontrol.State.Walking:
                animator.SetBool("Run", false);
                break;

            case Charcontrol.State.Running:
                animator.SetBool("Run", true);
                animator.SetBool("Crouch", false);
                animator.SetBool("Sliding", false);
                break;

            case Charcontrol.State.Jumping:
                if (jumped == false)
                {
                    animator.Play("Jump Transition");
                    animator.SetBool("Jumping", true);
                    jumped = true;
                }
                break;

            case Charcontrol.State.AirJumping:
                animator.Play("Jump Transition");
                animator.SetBool("Jumping", true);
                break;

            case Charcontrol.State.Falling:
                break;

            case Charcontrol.State.Landing:
                break;

            case Charcontrol.State.Crouching:
                animator.SetBool("Crouch", true);
                animator.SetBool("Run", false);
                animator.SetBool("Sliding", false);
                break;

            case Charcontrol.State.CrouchWalking:
                break;

            case Charcontrol.State.Attacking:
                break;

            case Charcontrol.State.Air_Attacking:
                break;

            case Charcontrol.State.Sliding:
                if (!Charcontrol.Instance.slid)
                {
                    animator.SetBool("Sliding", true);
                    animator.SetBool("Crouch", false);
                    animator.SetBool("Run", false);
                }
                break;

            case Charcontrol.State.Ledgegrabbing:
                break;

            case Charcontrol.State.Stunned:
                break;

            case Charcontrol.State.Dead:
                break;

        }
        onAnimate();
        isGrounded = Charcontrol.isGrounded;
        currentState = Charcontrol.Instance.currentState.ToString();
    }

    public void onAnimate()
    {
        if (isGrounded)
        {
            animator.SetBool("Grounded", true);
            animator.SetBool("Jumping", false);
            jumped = false;
        }
        if (!isGrounded)
        {
            animator.SetBool("Grounded", false);
        }

        animator.SetFloat("yVel", Mathf.Clamp(rb2d.velocity.y, -1, 1));
        animator.SetFloat("xVel", Mathf.Clamp(rb2d.velocity.x, -1, 1));
        animator.SetFloat("yVelAbs", Mathf.Abs(Mathf.Clamp(rb2d.velocity.y, -1, 1)));
        animator.SetFloat("xVelAbs", Mathf.Abs(Mathf.Clamp(rb2d.velocity.x, -1, 1)));
        animator.SetFloat("verticalPressed", Input.GetAxis("Vertical"));
        animator.SetFloat("horizontalPressed", Mathf.Abs(Input.GetAxis("Horizontal")));
    }
}




        

