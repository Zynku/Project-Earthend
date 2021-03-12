using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Char_control : MonoBehaviour
{
    private Rigidbody2D rb2d;

    [Header("Movement Variables")]
    [SerializeField] private float movementAcceleration = 9.5f;
    [SerializeField] private float maxMoveSpeed = 1.6f;
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float airLinearDrag = 2.5f;
    [SerializeField] private float groundLinearDrag = 4.67f;
    [SerializeField] private float fallJumpMultiplier = 0.6f;
    [SerializeField] private float lowJumpFallMultiplier = 1.29f;
    //[SerializeField] private float walljumpForce = 4f;

    [Header("Jump Variables")]
    public bool isGrounded;
    private float HorizontalDirection;
    private bool canJump => Input.GetKeyDown(KeyCode.UpArrow) && isGrounded;
    public float checkdistances = 0.2f;
    public float wallcheckdistances = 0.3f;
    public float airJumps = 2f;
    private float airJumpshas;
    public bool airJumped = false;
    public bool againstWallR = false;
    public bool againstWallL = false;
    public bool wallgrabbed = false;

    [Header("Melee Variables")]
    public GameObject Melee1;
    public bool Attacking;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        airJumpshas = airJumps;
        Melee1.SetActive(false);
    }
    
    private void Update()
    {
        // Get Horizontal Input from Method
        HorizontalDirection = GetInput().x;

        if (canJump) Jump();
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

        if (Input.GetKeyDown(KeyCode.D))
        {
            Attacking = true;
        }
    }

    private void FixedUpdate()
    {
        MoveCharacter();
        ApplyGroundLinearDrag();
        ApplyAirLinearDrag();
        FallMultiplier();
        WallCheck();
        AirJump();

        if (isGrounded) { ApplyGroundLinearDrag(); }
        else { ApplyAirLinearDrag(); }

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
        if (airJumpshas > 0 && !isGrounded && (Input.GetKeyDown(KeyCode.UpArrow)))
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
            Jump();
            airJumpshas -= 1f;
            airJumped = true;
        }

        if (rb2d.velocity.y < 0)
        {
            airJumped = false;
        }

        if (isGrounded)
        {
            airJumpshas = airJumps;
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
        if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.2f), Vector2.right, wallcheckdistances, 1 << LayerMask.NameToLayer("Ground")))
        {
            againstWallR = true;
        }
        else { againstWallR = false; }

        if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.2f), Vector2.left, wallcheckdistances, 1 << LayerMask.NameToLayer("Ground")))
        {
            againstWallL = true;
        }
        else { againstWallL = false; }
    }

    private void MoveCharacter()
    {
        //Adds a force equal to horizontaldirection variable * acceleration
        //If velocity is more than maxmovespeed, set speed to maxmovespeed
        rb2d.AddForce(new Vector2(HorizontalDirection, 0f) * movementAcceleration);

        if (Mathf.Abs(rb2d.velocity.x) > maxMoveSpeed)
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxMoveSpeed, rb2d.velocity.y);
    }
    
    public void OnMelee1Start()
    {
        //Used by Animation Events in Melee Animation. On Event 1 activates hitbox, on Event 2 deactivates hitbox
        Melee1.SetActive(true);
    }

    public void OnMelee1End()
    {
        Attacking = false;
        Melee1.SetActive(false);
    }
}