using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charcontrol : MonoBehaviour
{
    public static Charcontrol Instance;

    Animator animator;
    Rigidbody2D rb2d;
    BoxCollider2D boxCol;

    public State currentState;

    

    [Header("Variables")]
    public float xVel;
    public float yVel;
    private float inputX;
    private float inputY;
    [HideInInspector] public bool playerDead;

    [Header("Movement Variables")]
    public float currentDrag;
    public float runThreshold;
    public float facingDir = 1;
    [SerializeField] private float walkAcceleration = 9.5f;
    [SerializeField] private float maxWalkSpeed = 1.6f;
    [SerializeField] private float runAcceleration = 9.5f;
    [SerializeField] private float maxRunSpeed = 1.6f;

    [Header("Crouch Hitbox Variables")]
    public Vector2 boxColSize;
    public Vector2 boxColOffset;
    public Vector2 boxColCrouchSize;
    public Vector2 boxColCrouchOffset;

    [Header("Slide Variables")]
    private bool slid = false;
    public float slideForce;
    public float slideDrag;

    [Header("Jump Variables")]
    public static bool isGrounded;
    private bool jumped;
    public float fallThreshold;
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float airLinearDrag = 2.5f;
    [SerializeField] private float groundLinearDrag = 4.67f;
    public float HorizontalDirection;
    public float checkdistances = 0.05f;
    public float ycheckOffset = -0.23f;
    public int airJumps = 2;
    private int airJumpsHas;
    public float airHorizontalAcc;

    [Header("Melee Variables")]
    public GameObject MeleeObject;
    public float attackTimerTargetTime;
    private float attackTimer;
    public float attackComboTargetTime;
    private float attackComboTimer;
    public int attackdamageMax;
    public int attackdamageMin;
    BoxCollider2D meleeBoxCol;
    SpriteRenderer meleeSpriteR;


    void Awake()
    {
        //Ensures there's only ever one charcontrol script
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerDead = false;
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();

        currentState = State.Idle;

        boxCol = GetComponent<BoxCollider2D>();
        boxColSize = boxCol.size;
        boxColOffset = boxCol.offset;

        meleeSpriteR = MeleeObject.GetComponent<SpriteRenderer>();
        meleeBoxCol = MeleeObject.GetComponent<BoxCollider2D>();

        
    }

    public enum State
    {
        Idle,
        Walking,
        Running,
        Jumping,
        AirJumping,
        Falling,
        Landing,
        Crouching,
        CrouchWalking,
        Attacking,
        Air_Attacking,
        Sliding,
        Ledgegrabbing,
        Ledgejumping,
        Stunned,
        Dead
    }

    
    // Update is called once per frame
    void FixedUpdate()
    {
        xVel = rb2d.velocity.x;
        yVel = rb2d.velocity.y;
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        //FLIP THOSE ANIMS BABY UNLESS DED
        if (!playerDead)
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            }

            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                transform.localScale = new Vector3(-0.9f, 0.9f, 0.9f);
            }
        }
        if (currentState != State.Sliding)
        {
            if (isGrounded) { ApplyGroundLinearDrag(); }
            else { ApplyAirLinearDrag(); }
        }
        else
        {
            ApplySlideDrag();
        }
        
        


        //Grounded Check
        if (Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y + ycheckOffset), checkdistances, 1 << LayerMask.NameToLayer("Ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        onAnimate();

        //-------------------------------------------------------------------------------------------------------------------------------------
        switch (currentState)
        {
            case State.Idle:
                Idle();
                //Transition to Walking
                if (Input.GetAxis("Horizontal") != 0)
                {
                    currentState = State.Walking;
                }
                //Transition to Jumping
                if (Input.GetAxisRaw("Vertical") > 0)
                {
                    currentState = State.Jumping;
                }
                //Transition to Crouching
                if (Input.GetAxis("Vertical") < 0)
                {
                    currentState = State.Crouching;
                }
                //Transition to Attacking
                if (Input.GetAxisRaw("Light Attack") != 0 || Input.GetAxisRaw("Heavy Attack") != 0 || Input.GetAxisRaw("Ranged Attack") != 0)
                {
                    currentState = State.Attacking;
                }
                break;

            case State.Walking:
                Walking();
                //Transition back to Idle
                if (Input.GetAxisRaw("Horizontal") == 0 && Mathf.Abs(Mathf.Ceil(rb2d.velocity.x)) == 0)
                {
                    currentState = State.Idle;
                }
                //Transition to Running
                if (Mathf.Abs(rb2d.velocity.x) > runThreshold)
                {
                    currentState = State.Running;
                }
                //Transition to Attacking
                if (Input.GetAxisRaw("Light Attack") != 0 || Input.GetAxisRaw("Heavy Attack") != 0 || Input.GetAxisRaw("Ranged Attack") != 0)
                {
                    currentState = State.Attacking;
                }
                //Transition to Jumping
                if (Input.GetAxisRaw("Vertical") > 0)
                {
                    currentState = State.Jumping;
                }
                break;

            case State.Running:
                Running();
                //Transition back to Idle
                if (Input.GetAxisRaw("Horizontal") == 0 && Mathf.Abs(Mathf.Ceil(rb2d.velocity.x)) == 0)
                {
                    currentState = State.Idle;
                }
                //Transition back to Walk
                /*if (Mathf.Abs(rb2d.velocity.x) < runThreshold)
                {
                    currentState = State.Walking;
                }*/
                //Transition to Sliding
                if (Input.GetAxisRaw("Vertical") < 0)
                {
                    currentState = State.Sliding;
                }
                //Transition to Attacking
                if (Input.GetAxisRaw("Light Attack") != 0 || Input.GetAxisRaw("Heavy Attack") != 0 || Input.GetAxisRaw("Ranged Attack") != 0)
                {
                    currentState = State.Attacking;
                }
                //Transition to Jumping
                if (Input.GetAxisRaw("Vertical") > 0)
                {
                    currentState = State.Jumping;
                }
                break;

            case State.Jumping:
                Jump();

                //Transition to Falling
                if (rb2d.velocity.y < fallThreshold)
                {
                    currentState = State.Falling;
                }

                //Transition back to Idle
                if (isGrounded)
                {
                    currentState = State.Idle;
                }
                break;
            case State.AirJumping:
                AirJump();
                //Transition to Falling
                if (rb2d.velocity.y < fallThreshold)
                {
                    currentState = State.Falling;
                }
                break;

            case State.Falling:
                Falling();

                //Transition back to Idle
                if (isGrounded)
                {
                    currentState = State.Idle;
                }

                //Transition to AirJumping
                if (Input.GetAxisRaw("Vertical") > 0 && airJumpsHas != 0)
                {
                    currentState = State.AirJumping;
                }
                break;

            case State.Landing:
                break;

            case State.Crouching:
                Crouching();

                //Transition back to Idle
                if (Input.GetAxisRaw("Vertical") == 0)
                {
                    currentState = State.Idle;
                }
                break;

            case State.CrouchWalking:
                break;

            case State.Attacking:
               
                Attacking();
                //if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) { currentState = State.Idle; }
                break;

            case State.Air_Attacking:
                break; 

            case State.Sliding:
                Sliding();
                //Transition back to Running
                if (Input.GetAxisRaw("Vertical") == 0)
                {
                    currentState = State.Running;
                }
                //Transition to Crouching
                if (rb2d.velocity.x == 0)
                {
                    currentState = State.Crouching;
                }
                break;

            case State.Ledgegrabbing:
                break;

            case State.Ledgejumping:
                break;

            case State.Stunned:
                break;

            case State.Dead:
                Dead();
                break;
        }
        //-------------------------------------------------------------------------------------------------------------------------------------
    }

    private void Update()
    {
        if (currentState != State.Crouching)
        {
            boxCol.size = boxColSize;
            boxCol.offset = boxColOffset;
            rb2d.velocity = rb2d.velocity;
        }

        currentDrag = GetComponent<Rigidbody2D>().drag;

        if (Input.GetAxisRaw("Horizontal") == 1) { facingDir = 1; }
        if (Input.GetAxisRaw("Horizontal") == -1) { facingDir = -1; }
    }

    public void onAnimate()
    {
        if (isGrounded)
        {
            animator.SetBool("Grounded", true);
            animator.SetBool("Jumping", false);
        }
        if (!isGrounded)
        {
            animator.SetBool("Grounded", false);
        }

        if (GetComponent<Charhealth>().currentHealth <= 0)
        {
            currentState = State.Dead;
        }

        animator.SetFloat("yVel", Mathf.Clamp(rb2d.velocity.y, -1, 1));
        animator.SetFloat("xVel", Mathf.Clamp(rb2d.velocity.x, -1, 1));
        animator.SetFloat("yVelAbs", Mathf.Abs(Mathf.Clamp(rb2d.velocity.y, -1, 1)));
        animator.SetFloat("xVelAbs", Mathf.Abs(Mathf.Clamp(rb2d.velocity.x, -1, 1)));
        animator.SetFloat("verticalPressed", Input.GetAxis("Vertical"));
        animator.SetFloat("horizontalPressed", Mathf.Abs(Input.GetAxis("Horizontal")));
    }

    public void Idle()
    {
        currentState = State.Idle;
        airJumpsHas = airJumps;
        slid = false;
        animator.SetBool("Run", false);
        animator.SetBool("Crouch", false);
        animator.SetBool("Sliding", false);
        animator.SetBool("Jumping", false);
    }

    public void Walking()
    {
        //Adds a force equal to horizontaldirection variable * acceleration
        rb2d.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * walkAcceleration, 0f) );
        //If velocity is more than maxmovespeed, set speed to maxmovespeed
        if (Mathf.Abs(rb2d.velocity.x) > maxWalkSpeed)
        {
            rb2d.velocity = new Vector2(maxWalkSpeed * Input.GetAxisRaw("Horizontal"), rb2d.velocity.y);
        }

        //animator.SetBool("Run", true);

    }

    public void Running()
    {
        //Adds a force equal to horizontaldirection variable * acceleration
        rb2d.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * runAcceleration, 0f));
        //If velocity is more than maxmovespeed, set speed to maxmovespeed
        if (Mathf.Abs(rb2d.velocity.x) > maxRunSpeed)
        {
            rb2d.velocity = new Vector2(maxRunSpeed * Input.GetAxisRaw("Horizontal"), rb2d.velocity.y);
        }
        slid = false;
        animator.SetBool("Run", true);
        animator.SetBool("Crouch", false);
        animator.SetBool("Sliding", false);

    }

    public void Attacking()
    {
        //Dictates whether in attack state or not based on time
        attackTimer -= Time.deltaTime;
        if (attackTimer < 0) { attackTimer = 0; currentState = State.Idle; }

        //Dictates speed at which you can transition to next melee state based on when you press attack key
        attackComboTimer -= Time.deltaTime;
        if (attackComboTimer < 0) { attackComboTimer = 0; }

        if (attackComboTimer > 0 && Input.GetAxisRaw("Horizontal") != 0)
        {
            animator.SetTrigger("Melee");
            attackComboTimer = attackComboTargetTime;
        }

        
    }

    public void Sliding()
    {
        if (slid == false)
        {
            slid = true;
            rb2d.drag = slideDrag;
            rb2d.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * slideForce, 0f));
            animator.SetBool("Sliding", true);
            animator.SetBool("Crouch", false);
            animator.SetBool("Run", false);
        }
    }

    public void Crouching()
    {
        animator.SetBool("Crouch", true);
        animator.SetBool("Run", false);
        animator.SetBool("Sliding", false);
        rb2d.velocity = new Vector2(0, 0);
        boxCol.size = boxColCrouchSize;
        boxCol.offset = boxColCrouchOffset;

    }

    public void Jump()
    {
        if (jumped == false)
        {
            rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            rb2d.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * runAcceleration * airHorizontalAcc, 0f));

            jumped = true;
            animator.Play("Jump Transition");
            animator.SetBool("Jumping", true);
        }
        //Jumped Check
        if (isGrounded)
        {
            jumped = false;
        }
    }

    public void AirJump()
    {
        if (airJumpsHas != 0)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
            rb2d.AddForce(Vector2.up * jumpForce * 2, ForceMode2D.Impulse);
            rb2d.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * runAcceleration * airHorizontalAcc, 0f));
            jumped = true;
            airJumpsHas -= 1;
            currentState = State.Jumping;
            animator.Play("Jump Transition");
            animator.SetBool("Jumping", true);
            
        }
    }

    public void Dead()
    {
        animator.Play("Low Poly Death HD3");
        transform.localScale = transform.localScale;
        playerDead = true;
    }

    private void ApplyAirLinearDrag()
    {
        rb2d.drag = airLinearDrag;
    }

    private void ApplyGroundLinearDrag()
    {
        rb2d.drag = groundLinearDrag;
    }

    private void ApplySlideDrag()
    {
        rb2d.drag = slideDrag;
    }

    public void Falling()
    {
        //animator.Play("Fall");
    }

    public void OnMelee1Start()
    {
        //Used by Animation Events in Melee Animation. On Event 1 activates hitbox, on Event 2 deactivates hitbox
        meleeBoxCol.enabled = true;
    }

    public void OnMelee1End()
    {
        meleeBoxCol.enabled = false;
    }

    public void OnMeleeForceAdd(int forceX)
    {
        rb2d.AddForce(new Vector2(facingDir * forceX , 0), ForceMode2D.Impulse);
    }

    public void SetMeleeSprite(Sprite sprite)
    {
        meleeSpriteR.sprite = sprite;
    }

    private void OnDrawGizmos()
    {
        //Ground check sphere
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector2(transform.position.x, transform.position.y + ycheckOffset), checkdistances);
    }
}
