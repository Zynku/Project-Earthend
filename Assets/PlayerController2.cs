using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    public bool isGrounded;
    public float VectorForce;
    public float JumpForce;
    public float fallMultiplier;
    public float lowJumpMultiplier;
    private float AnimScale = 3.847f;

    Animator animator;
    Rigidbody2D rb2d;
    
    [SerializeField]
    Transform groundCheck;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();

    }
    private void Update()
    {
        //Applies fall multipliers to increase gravity in air and applies jump multipliers for longer button presses.
        if (rb2d.velocity.y < 0)
        {
            rb2d.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb2d.velocity.y >0 && !Input.GetButton ("Jump"))
        {
            rb2d.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
    private void FixedUpdate()
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
        //Applies right and left forces with correct animation, and flips to face correct direction
        if (Input.GetKey("right"))
        {
            rb2d.velocity = new Vector2(VectorForce, rb2d.velocity.y);
            if (isGrounded)
                animator.Play("Run Animation");
            else animator.Play("Jump Animation player-jump-1");
            transform.localScale = new Vector3(AnimScale, AnimScale, AnimScale);
        }
        else if (Input.GetKey("left"))
        {
            rb2d.velocity = new Vector2(-VectorForce, rb2d.velocity.y);
            if (isGrounded)
                animator.Play("Run Animation");
            else animator.Play("Jump Animation player-jump-1");
            transform.localScale = new Vector3(-AnimScale, AnimScale, AnimScale);
        }
        //Gives jumpforce if up input is given. If in the air, jump anim plays.
        if ((Input.GetKey("up")) && isGrounded)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, JumpForce);
        }
        //If in there air, plays jump animation
        if (!isGrounded)
        {
            animator.Play("Jump Animation player-jump-1");
        }
        //If there is no input, idle animation plays
        if (!Input.anyKey && isGrounded)
        {
            animator.Play("Idle Animation player-idle-1");
        }
        //If down is pressed, crouch animation plays
        if ((isGrounded && Input.GetKey("down")))
        {
            animator.Play("Crouch Animation 2");
            rb2d.velocity = new Vector2(0.0f, rb2d.velocity.y * Time.deltaTime); 
        }     
    }
}
