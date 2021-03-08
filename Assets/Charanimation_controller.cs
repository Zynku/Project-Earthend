using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charanimation_controller : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb2d;
    public bool isGrounded;
    public float isMoving;
    public bool isAttacking;
    public bool isJumping;
    [SerializeField] private string currentState;


    //Animation States
    const string P_IDLE = "Archer Idle";
    const string P_RUN = "Run";
    const string P_JUMP = "Jump";
    const string P_FALL = "Fall";


    void Start()
    {
        animator = GetComponent<Animator>();
        isGrounded = GetComponent<Charjump>().isGrounded;
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ChangeAnimationState(string newState)
    {
        //Stops animations from interrupting themselves
        if (currentState == newState) return;

        //Play new state
        animator.Play(newState);

        //Reassign the current state
        currentState = newState;
    }

    private void FixedUpdate()
    {
        //Movement States
        isMoving = Mathf.Abs(Input.GetAxisRaw("Horizontal"));
        isJumping = Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0;
        isAttacking = Input.GetKeyDown(KeyCode.D);

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //No input, play Idle animation
        if (isMoving == 0)
        {
            if ((rb2d.velocity.x) != 0 && !isJumping)
            {
                ChangeAnimationState(P_IDLE);
            }
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //Flip animations left or right based on input
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //Play Run animation if moving left or right
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0)
        {
            if (isGrounded)
            {
                ChangeAnimationState(P_RUN);
            }
            if (!isGrounded)
            {
                ChangeAnimationState(P_JUMP);
            }
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //Play Jump if onGround and if Jump is held
        if (isJumping)
        {
            if (!isGrounded)
            { 
                ChangeAnimationState(P_JUMP); 
            }

            if (isGrounded)
            { 
                ChangeAnimationState(P_JUMP);
            }
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}
