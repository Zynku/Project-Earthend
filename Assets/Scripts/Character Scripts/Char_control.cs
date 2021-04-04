using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Char_control : MonoBehaviour
{
    [SerializeField] internal Charinputcontrol inputScript;
    
    private Rigidbody2D rb2d;
    private BoxCollider2D boxCol;

    [Header("Movement Variables")]
    [SerializeField] private float movementAcceleration = 9.5f;
    [SerializeField] private float maxMoveSpeed = 1.6f;
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float airLinearDrag = 2.5f;
    [SerializeField] private float groundLinearDrag = 4.67f;
    [SerializeField] private float fallJumpMultiplier = 0.6f;
    [SerializeField] private float lowJumpFallMultiplier = 1.29f;
    public float facingDir = 0;
    public bool crouching = false;
    public bool sliding = true;

    //[SerializeField] private float walljumpForce = 4f;

    [Header("Jump Variables")]
    public bool isGrounded;
    private float HorizontalDirection;
    public float checkdistances = 0.2f;
    public float ycheckOffset;
    public float wallcheckdistances = 0.3f;
    public float ywallcheckoffset;
    public int airJumps = 2;
    public int airJumpshas;
    [HideInInspector] public bool airJumped = false;
    public bool againstWallR = false;
    public bool againstWallL = false;
    public bool againstWallFront = false;
    public bool wallgrabbed = false;

    [Header("Melee Variables")]
    public GameObject Melee1;
    [HideInInspector] public bool Attacking;
    public int attackdamageMax;
    public int attackdamageMin;

    [Header("Death Variables")]
    public bool dead;
    public float despawnTime;

    [Header("Crouch Hitbox Variables")]
    public Vector2 boxColSize;
    public Vector2 boxColOffset;
    public Vector2 boxColCrouchSize;
    public Vector2 boxColCrouchOffset;

    [Header("Misc Variables")]
    public float shakeintensity;
    public float shaketime;


    void Start()
    {
        boxCol = GetComponent<BoxCollider2D>();
        boxColSize = boxCol.size;
        boxColOffset = boxCol.offset;

        rb2d = GetComponent<Rigidbody2D>();
        airJumpshas = airJumps;
        Melee1.SetActive(false);


        dead = false;
    }
    
    private void Update()
    {

        if (Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y + ycheckOffset), checkdistances, 1 << LayerMask.NameToLayer("Ground"))) 
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (inputScript.lightAttackDown)
        {
            Attacking = true;
        }
        if ((inputScript.upDown) && isGrounded) { Jump(); }
        if (airJumpshas > 0 && !isGrounded && (inputScript.upDown)) { AirJump(); }

    }

    private void FixedUpdate()
    {
        ApplyGroundLinearDrag();
        ApplyAirLinearDrag();
        FallMultiplier();
        WallCheck();
        CrouchorSlide();

        //If not crouching and not dead...
        if (crouching == false && dead == false) 
        {
            //If you're pressing left or right move character
            if (inputScript.left || inputScript.right)
            MoveCharacter(); 
        } 

        if (isGrounded) { ApplyGroundLinearDrag(); }
        else { ApplyAirLinearDrag(); }

        if (GetComponent<Charhealth>().currentHealth <= 0)
        {
            dead = true;
            //onDeath();
        }

        //if (againstWallR == true && inputScript.rightDown && !isGrounded) { WallGrab(); }
    }

    private void MoveCharacter()
    {
        //Adds a force equal to horizontaldirection variable * acceleration if you're not holding down both like a madman
        if (!(inputScript.right && inputScript.left))
        {
            rb2d.AddForce(new Vector2(inputScript.horizontalDir, 0f) * movementAcceleration);
            facingDir = inputScript.horizontalDir;
        }
        

        //If velocity is more than maxmovespeed, set speed to maxmovespeed
        if (Mathf.Abs(rb2d.velocity.x) > maxMoveSpeed)
        {
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxMoveSpeed, rb2d.velocity.y);
        }     

        //If against a wall, apply reverse force to cancel out acceleration
        if (againstWallFront && !isGrounded)
        {
            rb2d.AddForce(new Vector2(-inputScript.horizontalDir, 0f) * movementAcceleration);
        }

        
    }

    private void Jump()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0f);
        rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void AirJump()
    {
        
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
        Jump();
        airJumpshas -= 1;
        airJumped = true;
        
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

    private void DestroyGameObject()
    {
        Destroy(gameObject);
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
        if (inputScript.rightDown)
        {
            rb2d.AddForce(new Vector2(-1, 1) * walljumpForce, ForceMode2D.Impulse);
            rb2d.gravityScale = 1;
        }
        else if (inputScript.rightUp)
        { rb2d.gravityScale = 1; }
    }*/

    private void FallMultiplier()
    {
        if (rb2d.velocity.y < 0)
        {
            rb2d.gravityScale = fallJumpMultiplier;
        }
        else if (rb2d.velocity.y > 0 && !inputScript.up)
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
        if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + ywallcheckoffset), Vector2.right *  facingDir, wallcheckdistances, 1 << LayerMask.NameToLayer("Ground")))
        {
            againstWallFront = true;
        }
        else { againstWallFront = false; }
    }

    public void CrouchorSlide()
    {
        //This is crouch
        if (Mathf.Abs(rb2d.velocity.x) < 0.1f)
        {
            if (inputScript.down && isGrounded)
            {
                crouching = true;
                rb2d.velocity = new Vector2(0, 0);
                boxCol.size = boxColCrouchSize;
                boxCol.offset = boxColCrouchOffset;
            }
            else
            {
                boxCol.size = boxColSize;
                boxCol.offset = boxColOffset;
                crouching = false;
                rb2d.velocity = rb2d.velocity;

            }
        }
        //This is slide
        if (Mathf.Abs(rb2d.velocity.x) > 0.5f)
        {
            if (inputScript.down && isGrounded)
            {
                sliding = true;
                rb2d.velocity = rb2d.velocity;
                //rb2d.AddForce(new Vector2(-HorizontalDirection, 0f) * -movementAcceleration);
            }
        }
    }
    
    public void OnMelee1Start()
    {
        //Used by Animation Events in Melee Animation. On Event 1 activates hitbox, on Event 2 deactivates hitbox
        Melee1.SetActive(true);
        cinemachine_controller.Instance.ShakeCamera(shakeintensity, shaketime);
    }

    public void OnMelee1End()
    {
        Attacking = false;
        Melee1.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector2(transform.position.x, transform.position.y + ycheckOffset), checkdistances);

        Gizmos.DrawRay(new Vector2(transform.position.x, transform.position.y + ywallcheckoffset), (Vector2.right * facingDir) * wallcheckdistances );
    }
}