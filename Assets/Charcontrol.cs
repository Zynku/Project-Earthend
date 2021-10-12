using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Charcontrol : MonoBehaviour
{
    public static Charcontrol Instance;
    Charanimation charanimation;
    Charattacks charattacks;
    Animator animator;
    [HideInInspector] public Rigidbody2D rb2d;
    BoxCollider2D boxCol;

    public State currentState;

    [Header("Variables")]
    public float xVel;
    public float yVel;
    [HideInInspector] public float inputX;
    [HideInInspector] public float inputY;
    public static GameObject closestNPC;
    [HideInInspector] public bool playerDead;
    [HideInInspector] public bool checkForSlopes;

    [Header("Movement Variables")]
    public float currentDrag;
    public float runThreshold;
    [HideInInspector] public float facingDir = 1;

    [Header("Walking Variables")]
    public float walkSpeed = 9.5f;
    //[SerializeField] private float maxWalkSpeed = 1.6f;

    [Header("Running Variables")]
    public float runSpeed = 9.5f;
    //[SerializeField] private float maxRunSpeed = 1.6f;

    [Header("Dodging Variables")]
    public bool rolled = false;
    [SerializeField] private float rollDrag = 0.5f;

    [Header("Jump Variables")]
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float airLinearDrag = 2.5f;
    [SerializeField] private float groundLinearDrag = 4.67f;
    public float HorizontalDirection;
    public float airHorizontalAcc;
    public bool isGrounded;
    [HideInInspector] public bool jumped = false;
    [HideInInspector] public float fallThreshold;
    [HideInInspector] public float checkdistances = 0.05f;
    [HideInInspector] public float ycheckOffset = -0.26f;
    [HideInInspector] public int airJumps = 2;
    [HideInInspector] public int airJumpsHas;

    [Header("Combat State Variables")]
    public bool inCombat;   
    public float combatStateTime;
    public float combatStateTargetTime = 7f;
    private GameObject onscreenTimer;

    [Header("Melee Variables")]
    public GameObject MeleeObject;
    public float attackTimerTargetTime;
    private float attackTimer;
    public float attackComboTargetTime;
    [HideInInspector] public float attackComboTimer;
    [HideInInspector] public int attackdamageMax;
    [HideInInspector] public int attackdamageMin;
    //private SpriteRenderer meleeSpriteR;
    //private ParticleSystem particles;

    public float switchingDirTime;
    public float switchingDirTargetTime = 20f;

    

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
        Dodging,
        Stunned,
        Dead
    }

    void Start()
    {
        charattacks = GetComponent<Charattacks>();
        charanimation = GetComponent<Charanimation>();

        switchingDirTime = switchingDirTargetTime;
        combatStateTime = combatStateTargetTime;
        onscreenTimer = TimerManager.instance.CreateTimer(combatStateTime, combatStateTargetTime, "Combo State Duration Time", false);

        //playerDead = false;
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();

        currentState = State.Idle;

        boxCol = GetComponent<BoxCollider2D>();
        //boxColSize = boxCol.size;
        //boxColOffset = boxCol.offset;

        //meleeSpriteR = MeleeObject.GetComponent<SpriteRenderer>();
        //particles = GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
        xVel = rb2d.velocity.x;
        yVel = rb2d.velocity.y;
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        onscreenTimer.GetComponent<SimpleTimerScript>().timerTime = combatStateTime;

        if (inCombat) { combatStateTime -= Time.deltaTime; }
        else { combatStateTime = combatStateTargetTime; }

        if(combatStateTime < 0) { combatStateTime = 0; }

        if (currentState != State.Dodging)
        {
            if (isGrounded) { ApplyGroundLinearDrag(); }
            else { ApplyAirLinearDrag(); }
        }
        else
        {
            ApplyRollDrag();
        }

        switch (currentState)
        {
            case State.COMBAT_Idle:
                charattacks.Combat_Idle();
                inCombat = true;

                if (combatStateTime == 0)
                {
                    currentState = State.Idle;
                }

                if (Input.GetAxisRaw("Vertical") > 0)
                {
                    currentState = State.COMBAT_Jumping;
                }

                if (Input.GetAxisRaw("Horizontal") != 0)
                {
                    currentState = State.COMBAT_Running;
                }
                break;

            case State.COMBAT_Walking:
                inCombat = true;
                if (Input.GetAxisRaw("Vertical") > 0)
                {
                    currentState = State.COMBAT_Jumping;
                }
                break;

            case State.COMBAT_Running:
                inCombat = true;
                charattacks.Combat_Running();
                combatStateTime = combatStateTargetTime;

                if (Input.GetAxisRaw("Vertical") > 0)
                {
                    currentState = State.COMBAT_Jumping;
                }

                if (Mathf.Abs(Mathf.Ceil(rb2d.velocity.x)) == 0)
                {
                    currentState = State.COMBAT_Idle;
                }
                //Transition back to Walk
                //Nothing haha fuck you
                //Transition to Sliding
/*                if (Input.GetButtonDown("Dodge"))
                {
                    currentState = State.Dodging;
                }*/
                //Transition to Attacking
                /*if (Input.GetAxisRaw("Light Attack") != 0 || Input.GetAxisRaw("Heavy Attack") != 0 || Input.GetAxisRaw("Ranged Attack") != 0)
                {
                    currentState = State.Attacking;
                }*/
                //Transition to Jumping
/*                if (Input.GetAxisRaw("Vertical") > 0)
                {
                    currentState = State.Jumping;
                }*/
                break;

            case State.COMBAT_Jumping:
                inCombat = true;
                Jumping();
                //Transition to Falling
                if (rb2d.velocity.y < fallThreshold)
                {
                    currentState = State.COMBAT_Falling;
                }
                //Transition back to Idle
                if (isGrounded && yVel < 0)
                {
                    currentState = State.COMBAT_Idle;
                }
                break;

            case State.COMBAT_AirJumping:
                inCombat = true;
                AirJump();
                //Transition to Falling
                if (rb2d.velocity.y < fallThreshold)
                {
                    currentState = State.COMBAT_Falling;
                }
                break;

            case State.COMBAT_Falling:
                inCombat = true;
                Falling();
                //Transition back to Idle
                if (isGrounded)
                {
                    currentState = State.COMBAT_Idle;
                }
                //Transition to AirJumping
                //if (Input.GetAxisRaw("Vertical") > 0 && airJumpsHas != 0)
                if (Input.GetButtonDown("Vertical") && airJumpsHas != 0)
                {
                    currentState = State.COMBAT_AirJumping;
                }
                break;

            case State.COMBAT_Landing:
                inCombat = true;
                break;

            case State.COMBAT_Attacking:
                inCombat = true;
                combatStateTime = combatStateTargetTime;
                if (!charanimation.currentlyComboing)
                {
                    currentState = State.COMBAT_Idle;
                }
                break;

            case State.COMBAT_Air_Attacking:
                inCombat = true;
                break;

            case State.COMBAT_Rolling:
                inCombat = true;
                break;

            case State.COMBAT_Stunned:
                inCombat = true;
                break;

            case State.COMBAT_Dead:
                inCombat = true;
                break;

            case State.Switching_to_COMBAT:
                inCombat = true;
                break;

            case State.Ledgegrabbing:
                break;

            case State.Ledgejumping:
                break;

            case State.Idle:
                inCombat = false;
                Idle();
                //Transition to Walking
                if (Input.GetAxisRaw("Horizontal") != 0)
                {
                    currentState = State.Walking;
                }

/*                if (Input.GetButtonDown("Light Attack"))
                {
                    currentState = State.COMBAT_Idle;
                }*/
                //Transition to Jumping
                if (Input.GetAxisRaw("Vertical") > 0)
                {
                    currentState = State.Jumping;
                }
                //Transition to Attacking
/*                if (Input.GetAxisRaw("Light Attack") != 0 || Input.GetAxisRaw("Heavy Attack") != 0 || Input.GetAxisRaw("Ranged Attack") != 0)
                {
                    currentState = State.Attacking;
                }*/
                if (Input.GetButtonDown("Dodge"))
                {
                    currentState = State.Dodging;
                }
                break;

            case State.Walking:
                inCombat = false;
                Walking();
                //Transition back to Idle
                if (Input.GetAxisRaw("Horizontal") == 0 /*&& Mathf.Abs(Mathf.Floor(rb2d.velocity.x)) == 0*/)
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
                if (Input.GetButtonDown("Dodge"))
                {
                    currentState = State.Dodging;
                }
                //Transition to Swithing_Dir
                /*if (xVel > 0 && Input.GetAxis("Horizontal") < 0)        //If you're running right, and you press the left key, start switching direction
                {
                    currentState = State.Switching_Dir;
                }
                if (xVel < 0 && Input.GetAxis("Horizontal") > 0)        //If you're running left, and you press the right key, start switching direction
                {
                    currentState = State.Switching_Dir;
                }*/
                break;

            case State.Switching_Dir:
                inCombat = false;
                if (xVel == 0)
                {
                    currentState = State.Running;
                }
                break;

            case State.Running:
                inCombat = false;
                Running();
                //Transition back to Idle
                //if (Input.GetAxisRaw("Horizontal") == 0 /*&& Mathf.Abs(Mathf.Ceil(rb2d.velocity.x)) == 0*/)
                if(Mathf.Abs(Mathf.Ceil(rb2d.velocity.x)) == 0)
                {
                    currentState = State.Idle;
                }
                //Transition back to Walk
                //Nothing haha fuck you
                //Transition to Sliding
                if (Input.GetButtonDown("Dodge"))
                {
                    currentState = State.Dodging;
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
/*                if (!timerMade)
                {
                    GameObject timer;
                    timer = TimerManager.instance.CreateAutoTimer(switchingDirTargetTime, "Test Timer", true, true);
                    timerMade = true;
                }*/
     /*           if (xVel > 0 && Input.GetAxis("Horizontal") < 0)        //If you're running right, and you press the left key, start switching direction
                {
                    currentState = State.Switching_Dir;
                }
                if (xVel < 0 && Input.GetAxis("Horizontal") > 0)        //If you're running left, and you press the right key, start switching direction
                {
                    currentState = State.Switching_Dir;
                }
                switchingDirTime -= Time.deltaTime;*/
                
                break;

            case State.Jumping:
                inCombat = false;
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
                inCombat = false;
                AirJump();
                //Transition to Falling
                if (rb2d.velocity.y < fallThreshold)
                {
                    currentState = State.Falling;
                }
                break;

            case State.Falling:
                inCombat = false;
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
                inCombat = false;
                break;

            case State.Crouching:
                inCombat = false;
                //boxCol.size = boxColSize;
                //boxCol.offset = boxColOffset;
                rb2d.velocity = rb2d.velocity;
                break;

            case State.CrouchWalking:
                inCombat = false;
                break;

            case State.Attacking:
                inCombat = false;
                break;

            case State.Air_Attacking:
                inCombat = false;
                break;

            case State.Dodging:     //Is the same as rolling
                inCombat = false;
                //Animation is called in Charanimation
                break;

            case State.Stunned:
                inCombat = false;
                break;

            case State.Dead:
                inCombat = false;
                break;

            default:
                inCombat = false;
                break;
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
        rolled = false;

        //attackTimer = attackTimerTargetTime;
        //attackComboTimer = attackComboTargetTime;
        canFlipXDir();
    }

    public void Walking()
    {
        //Adds a force equal to horizontaldirection variable * acceleration
        //rb2d.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * walkAcceleration, 0f));
        //rb2d.velocity = new Vector2(Mathf.Lerp(0f, walkAcceleration*facingDir, 3f), 0f);
        rb2d.velocity = new Vector2(walkSpeed * Input.GetAxis("Horizontal"), rb2d.velocity.y);
        //If velocity is more than maxmovespeed, set speed to maxmovespeed

        /*        if (rb2d.velocity.x != 0)
                {
                    if (rb2d.velocity.x > -0.1 && Input.GetAxisRaw("Horizontal") == -1)  //Moving right and press left
                    {
                        Debug.Log("Switching direction left");
                    }
                    if (rb2d.velocity.x < 0.1 && Input.GetAxisRaw("Horizontal") == 1)  //Moving left and press right
                    {
                        Debug.Log("Switching direction right");
                    }
                }*/
        canFlipXDir();
    }

    public void Running()
    {
        //Adds a force equal to horizontaldirection variable * acceleration
        //rb2d.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * runAcceleration, 0f));
        //rb2d.velocity = new Vector2(Mathf.Lerp(0f, runAcceleration*facingDir, 3f), 0f);
        rb2d.velocity = new Vector2(runSpeed * Input.GetAxis("Horizontal"), rb2d.velocity.y);
        //If velocity is more than maxmovespeed, set speed to maxmovespeed
        rolled = false;
        canFlipXDir();
    }

    public void Jumping()
    {
        if (!jumped)
        {
            //Debug.Log("Jumping");
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
        canFlipXDir();
    }

    public void AirJump()
    {
        if (airJumpsHas != 0)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
            rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            rb2d.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * runSpeed * airHorizontalAcc, 0f));
            jumped = true;
            --airJumpsHas;
            currentState = State.Jumping;
        }
        canFlipXDir();
    }

    public void Falling()
    {
        //rb2d.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * airHorizontalAcc, 0f));
        rb2d.velocity = new Vector2((Input.GetAxisRaw("Horizontal") * airHorizontalAcc), rb2d.velocity.y);
        canFlipXDir();
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

    private void ApplyRollDrag()
    {
        rb2d.drag = rollDrag;
    }

    public void canFlipXDir()
    {
        //FLIP THOSE ANIMS BABY
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    public void onDodge(int force)
    {
        rb2d.AddForce(new Vector2 (rb2d.velocity.x + (force * facingDir), rb2d.velocity.y));
        rolled = true;
    }

    public void onDodgeTransition()
    {
        currentState = State.Idle;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(new Vector2(transform.position.x, transform.position.y + ycheckOffset), checkdistances);
    }

}