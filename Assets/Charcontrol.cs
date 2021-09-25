using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Charcontrol : MonoBehaviour
{
    public static Charcontrol Instance;
    Animator animator;
    Rigidbody2D rb2d;
    BoxCollider2D boxCol;


    [Header("Variables")]
    public float xVel;
    public float yVel;
    public float inputX;
    public float inputY;
    public static GameObject closestNPC;
    [HideInInspector] public bool playerDead;
    public bool checkForSlopes;

    [Header("Movement Variables")]
    public float currentDrag;
    public float runThreshold;
    public float facingDir = 1;
    [SerializeField] private float walkAcceleration = 9.5f;
    [SerializeField] private float maxWalkSpeed = 1.6f;
    [SerializeField] private float runAcceleration = 9.5f;
    [SerializeField] private float maxRunSpeed = 1.6f;

    [Header("Jump Variables")]
    public static bool isGrounded;
    public bool jumped = false;
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
    [HideInInspector] public float attackComboTimer;
    public int attackdamageMax;
    public int attackdamageMin;
    SpriteRenderer meleeSpriteR;
    ParticleSystem particles;

    private bool timerMade = false;
    public float switchingDirTime;
    public float switchingDirTargetTime = 20f;

    public State currentState;

    public enum State
    {
        COMBAT_Idle,
        COMBAT_Walking,
        COMBAT_Running,
        COMBAT_Jumping,
        COMBAT_AirJumping,
        COMBAT_Falling,
        COMBAT_Landing,
        COMBAT_Attacking,
        COMBAT_Air_Attacking,
        COMBAT_Rolling,
        COMBAT_Stunned,
        COMBAT_Dead,
        Switching_to_COMBAT,
        Ledgegrabbing,
        Ledgejumping,
        Idle,
        Walking,
        Switching_Dir,
        Running,
        Jumping,
        AirJumping,
        Falling,
        Landing,
        Crouching,
        CrouchWalking,
        Attacking,
        Air_Attacking,
        Rolling,
        Stunned,
        Dead
    }

    void Start()
    {
        switchingDirTime = switchingDirTargetTime;

        //playerDead = false;
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();

        currentState = State.Idle;

        boxCol = GetComponent<BoxCollider2D>();
        //boxColSize = boxCol.size;
        //boxColOffset = boxCol.offset;

        meleeSpriteR = MeleeObject.GetComponent<SpriteRenderer>();
        particles = GetComponentInChildren<ParticleSystem>();
    }

    private void FixedUpdate()
    {
        xVel = rb2d.velocity.x;
        yVel = rb2d.velocity.y;
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

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
        if (currentState != State.Rolling)
        {
            //if (isGrounded) { ApplyGroundLinearDrag(); }
            //else { ApplyAirLinearDrag(); }
        }
        else
        {
            //ApplyRollDrag();
        }

        switch (currentState)
        {
            case State.COMBAT_Idle:
                break;
            case State.COMBAT_Walking:
                break;
            case State.COMBAT_Running:
                break;
            case State.COMBAT_Jumping:
                break;
            case State.COMBAT_AirJumping:
                break;
            case State.COMBAT_Falling:
                break;
            case State.COMBAT_Landing:
                break;
            case State.COMBAT_Attacking:
                break;
            case State.COMBAT_Air_Attacking:
                break;
            case State.COMBAT_Rolling:
                break;
            case State.COMBAT_Stunned:
                break;
            case State.COMBAT_Dead:
                break;
            case State.Switching_to_COMBAT:
                break;
            case State.Ledgegrabbing:
                break;
            case State.Ledgejumping:
                break;
            case State.Idle:
                Idle();
                //Transition to Walking
                if (Input.GetAxisRaw("Horizontal") != 0)
                {
                    currentState = State.Walking;
                }
                //Transition to Jumping
                if (Input.GetAxisRaw("Vertical") > 0)
                {
                    currentState = State.Jumping;
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
                if (Input.GetAxisRaw("Vertical") < 0)
                {
                    currentState = State.Rolling;
                }
                //Transition to Swithing_Dir
                if (xVel > 0 && Input.GetAxis("Horizontal") < 0)        //If you're running right, and you press the left key, start switching direction
                {
                    currentState = State.Switching_Dir;
                }
                if (xVel < 0 && Input.GetAxis("Horizontal") > 0)        //If you're running left, and you press the right key, start switching direction
                {
                    currentState = State.Switching_Dir;
                }
                break;
            case State.Switching_Dir:
                if (xVel == 0)
                {
                    currentState = State.Running;
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
                //Nothing haha fuck you
                //Transition to Sliding
                if (Input.GetAxisRaw("Vertical") < 0)
                {
                    currentState = State.Rolling;
                }
                //Transition to Attacking
                /*if (Input.GetAxisRaw("Light Attack") != 0 || Input.GetAxisRaw("Heavy Attack") != 0 || Input.GetAxisRaw("Ranged Attack") != 0)
                {
                    currentState = State.Attacking;
                }*/
                //Transition to Jumping
                if (Input.GetAxisRaw("Vertical") > 0)
                {
                    currentState = State.Jumping;
                }
                //Transition to Swithing_Dir
                if (!timerMade)
                {
                    GameObject timer;
                    timer = TimerManager.instance.CreateAutoTimer(switchingDirTargetTime, "Test Timer", true, true);
                    timerMade = true;
                }
                if (xVel > 0 && Input.GetAxis("Horizontal") < 0)        //If you're running right, and you press the left key, start switching direction
                {
                    currentState = State.Switching_Dir;
                }
                if (xVel < 0 && Input.GetAxis("Horizontal") > 0)        //If you're running left, and you press the right key, start switching direction
                {
                    currentState = State.Switching_Dir;
                }
                switchingDirTime -= Time.deltaTime;
                
                break;
            case State.Jumping:
                Jumping();
                //Transition to Falling
                if (rb2d.velocity.y < fallThreshold)
                {
                    currentState = State.Falling;
                }
                //Transition back to Idle
                if (isGrounded && yVel < 0)
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
                //if (Input.GetAxisRaw("Vertical") > 0 && airJumpsHas != 0)
                if (Input.GetButtonDown("Vertical") && airJumpsHas != 0)
                {
                    currentState = State.AirJumping;
                }
                break;
            case State.Landing:
                break;
            case State.Crouching:
                break;
            case State.CrouchWalking:
                break;
            case State.Attacking:
                break;
            case State.Air_Attacking:
                break;
            case State.Rolling:
                break;
            case State.Stunned:
                break;
            case State.Dead:
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        
        if (currentState != State.Crouching)
        {
            //boxCol.size = boxColSize;
            //boxCol.offset = boxColOffset;
            rb2d.velocity = rb2d.velocity;
        }

        currentDrag = GetComponent<Rigidbody2D>().drag;

        if (Input.GetAxisRaw("Horizontal") == 1) { facingDir = 1; }
        if (Input.GetAxisRaw("Horizontal") == -1) { facingDir = -1; }

        checkforGrounded();
    }

    void checkforGrounded()
    {
        //Grounded Check
        if (Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y + ycheckOffset), checkdistances, 1 << LayerMask.NameToLayer("Ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void Idle()
    {
        currentState = State.Idle;
        airJumpsHas = airJumps;
        //rolled = false;

        //attackTimer = attackTimerTargetTime;
        //attackComboTimer = attackComboTargetTime;
    }

    public void Walking()
    {
        //Adds a force equal to horizontaldirection variable * acceleration
        rb2d.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * walkAcceleration, 0f));
        //If velocity is more than maxmovespeed, set speed to maxmovespeed
        if (Mathf.Abs(rb2d.velocity.x) > maxWalkSpeed)
        {
            rb2d.velocity = new Vector2(maxWalkSpeed * Input.GetAxisRaw("Horizontal"), rb2d.velocity.y);
        }
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
        //rolled = false;
    }

    public void Jumping()
    {
        if (!jumped)
        {
            Debug.Log("Jumping");
            //rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Force); 
            rb2d.velocity = new Vector2((Input.GetAxisRaw("Horizontal") * airHorizontalAcc), jumpForce);
            //rb2d.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * runAcceleration * airHorizontalAcc, 0f));
            jumped = true;
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
            rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            rb2d.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * runAcceleration * airHorizontalAcc, 0f));
            jumped = true;
            --airJumpsHas;
            currentState = State.Jumping;
        }
    }

    public void Falling()
    {
        rb2d.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * airHorizontalAcc, 0f));
    }

    public void FindClosestNPC()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("NPC");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        closestNPC = closest;
    }
}