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
    Charcontrol charcontrol;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        charcontrol = GetComponent<Charcontrol>();
    }


    private void FixedUpdate()
    {
        switch (charcontrol.currentState)
        {
            case Charcontrol.State.Idle:
                animator.SetBool("Run", false);
                animator.SetBool("Crouch", false);
                animator.SetBool("Jumping", false);
                animator.SetBool("Walking", false);
                break;

            case Charcontrol.State.Walking:
                animator.SetBool("Run", false);
                animator.SetBool("Walking", true);
                break;

            case Charcontrol.State.Running:
                animator.SetBool("Run", true);
                animator.SetBool("Walking", false);
                animator.SetBool("Crouch", false);
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

            case Charcontrol.State.CrouchWalking:
                break;

            case Charcontrol.State.Attacking:
                break;

            case Charcontrol.State.Air_Attacking:
                break;

            case Charcontrol.State.Dodging:
                    animator.Play("L_P Combat Roll into Idle");
                    //animator.SetTrigger("Dodging");
                    //animator.SetTrigger("Rolling");
                    animator.SetBool("Run", false);
                break;

            case Charcontrol.State.Ledgegrabbing:
                break;

            case Charcontrol.State.Stunned:
                break;

            case Charcontrol.State.Dead:
                break;

        }
        onAnimate();
        isGrounded = charcontrol.isGrounded;
        currentState = charcontrol.currentState.ToString();
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




        

