﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charjump : MonoBehaviour
{
    private Rigidbody2D rb2d;
    [SerializeField] private float jumpForce = 4f;
    //[SerializeField] private float walljumpForce = 4f;
    [SerializeField] private float airLinearDrag = 2.5f;
    [SerializeField] private float groundLinearDrag = 4.67f;
    [SerializeField] private float fallJumpMultiplier = 0.6f;
    [SerializeField] private float lowJumpFallMultiplier = 1.29f;
    private float HorizontalDirection;
    private bool canjump => Input.GetKeyDown(KeyCode.UpArrow) && isGrounded;
    public bool isGrounded;
    public float checkdistances = 0.2f;
    public float wallcheckdistances = 0.3f;
    public float airJumps = 2f;
    public bool airJumped = false;
    public bool againstWallR = false;
    public bool againstWallL = false;
    public bool wallgrabbed = false;


    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

        if (canjump) Jump();
        {
            HorizontalDirection = GetInput().x;
        }

        //Creates 3 Linecasts extending checkdistances at the middle, left and right of transform, with layermask for layer "Ground"
        if (Physics2D.Raycast(transform.position, Vector2.down, checkdistances, 1 << LayerMask.NameToLayer("Ground")) || 
            Physics2D.Raycast(new Vector2(transform.position.x + 0.095f, transform.position.y), Vector2.down, checkdistances, 1 << LayerMask.NameToLayer("Ground")) ||
            Physics2D.Raycast(new Vector2(transform.position.x + -0.095f, transform.position.y), Vector2.down, checkdistances, 1 << LayerMask.NameToLayer("Ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void FixedUpdate()
    {
        ApplyGroundLinearDrag();
        ApplyAirLinearDrag();
        FallMultiplier();
        WallCheck();
        AirJump();

        if (isGrounded) {ApplyGroundLinearDrag();}
        else{ApplyAirLinearDrag();}

        //if (againstWallR == true && Input.GetKey(KeyCode.RightArrow) && !isGrounded) { WallGrab(); }
    }

    private Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void Jump()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0f);
        rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void AirJump()
    {
        if (airJumps > 0 && !isGrounded && (Input.GetKeyDown(KeyCode.UpArrow)))
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
            Jump();
            airJumps -= 1f;
            airJumped = true;
        }

        if (rb2d.velocity.y < 0)
        {
            airJumped = false;
        }

        if (isGrounded)
        {
            airJumps = 2;
            airJumped = false;
        }
    }

    /*private void WallGrab()
    {
        wallgrabbed = true;
        rb2d.velocity = new Vector2(0, 0);
        rb2d.gravityScale = 0;
        //WallJump();
       
    }*/

    /*private void WallJump()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            rb2d.AddForce(new Vector2(-1, 1) * walljumpForce, ForceMode2D.Impulse);
            rb2d.gravityScale = 1;
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        { rb2d.gravityScale = 1; }
    }*/

    private void FallMultiplier()
    {
        if (rb2d.velocity.y < 0)
        {
            rb2d.gravityScale = fallJumpMultiplier;
        }
        else if (rb2d.velocity.y > 0 && !Input.GetKeyDown(KeyCode.UpArrow))
        {
            rb2d.gravityScale = lowJumpFallMultiplier;
        }
        else
        {
            rb2d.gravityScale = 1f;
        }
    }

    private void ApplyAirLinearDrag()
    {
            rb2d.drag = airLinearDrag;
    }

    private void ApplyGroundLinearDrag()
    {
        if (Mathf.Abs(HorizontalDirection) < 0.4 /*|| changingDirection*/)
        {
            rb2d.drag = groundLinearDrag;
        }
        else
        {
            rb2d.drag = 0f;
        }
    }

    private void WallCheck()
    {
        if (Physics2D.Raycast(new Vector2(transform.position.x , transform.position.y + 0.2f), Vector2.right, wallcheckdistances, 1 << LayerMask.NameToLayer("Ground")))
        {
            againstWallR = true;
        }
        else { againstWallR = false; }

        if (Physics2D.Raycast(new Vector2(transform.position.x , transform.position.y + 0.2f), Vector2.left, wallcheckdistances, 1 << LayerMask.NameToLayer("Ground")))
        {
            againstWallL = true;
        }
        else { againstWallL = false; }
    }

    private void OnDrawGizmos()
    {
        //Middle groundcheck
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector2.down * checkdistances);
        //Right groundcheck
        Gizmos.color = Color.green;
        Gizmos.DrawRay(new Vector2 (transform.position.x + 0.095f, transform.position.y), Vector2.down * checkdistances);
        //Left groundcheck
        Gizmos.color = Color.green;
        Gizmos.DrawRay(new Vector2(transform.position.x + -0.095f, transform.position.y), Vector2.down * checkdistances);
        //Right wallcheck
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(new Vector2(transform.position.x , transform.position.y + 0.2f), Vector2.right * wallcheckdistances);
        //Left wallcheck
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(new Vector2(transform.position.x , transform.position.y + 0.2f), Vector2.left * wallcheckdistances);
    }
}
