using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;


public class Charcontrol : MonoBehaviour
{
    [Foldout("Variables", true)]
    public static Charcontrol Instance;
    Charanimation charanimation;
    Charattacks charattacks;
    Animator animator;
    [HideInInspector] public Rigidbody2D rb2d;
    BoxCollider2D boxCol;

    public State currentState;

    [Separator("Variables")]
    public float xVel;
    public float yVel;
    [HideInInspector] public float inputX;
    [HideInInspector] public float inputY;
    public GameObject closestNPC;
    [HideInInspector] public bool playerDead;
    [HideInInspector] public bool checkForSlopes;

    [Separator("Movement Variables")]
    public float currentDrag;
    public float runThreshold;
    [ReadOnly] public float facingDir = 1;

    [Separator("Crouching Variables")]
    public bool inCrouchingTrigger;             //This is the area the player can press interact in to crouch
    public bool inCrouchingTriggerStayZone;     //This is the area the player must stay in to stay crouched. If they leave it they are automatically uncrouched
    public float crouchWalkingSpeed = 4.75f;

    [Separator("Walking Variables")]
    public float walkSpeed = 9.5f;
    public bool allowWalking;
    //[SerializeField] private float maxWalkSpeed = 1.6f;

    [Separator("Running Variables")]
    public float runSpeed = 9.5f;
    //[SerializeField] private float maxRunSpeed = 1.6f;

    [Separator("Dodging Variables")]
    public bool rolled = false;
    [SerializeField] private float rollDrag = 0.5f;

    [Separator("Jump Variables")]
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float airLinearDrag = 2.5f;
    [SerializeField] private float groundLinearDrag = 4.67f;
    public float HorizontalDirection;
    public float airHorizontalAcc;
    [HideInInspector] public bool jumped = false;
    [HideInInspector] public float fallThreshold;
    [HideInInspector] public int airJumps = 2;
    [HideInInspector] public int airJumpsHas;

    [Separator("Ground and Wall Checks")]
    public bool isGrounded;
    [HideInInspector] public float groundCheckDistances = 0.05f;
    [HideInInspector] public float groundYCheckOffset = -0.26f;

    public bool isAgainstWallLeft, isAgainstWallLeftBottom, isAgainstWallLeftTop, isAgainstWallLeftMid;
    public float wallLeftCheckDistances;        //How large is the circle to check for?
    public float wallLeftYCheckOffset;          //How far from center is the circle vertically?
    public float wallLeftXCheckOffset;          //How far from center is the circle horizontally?
    public float wallLeftDuplicatesYOffset;     //How far from the middle circle are the two circles spaced out vertically?

    public bool isAgainstWallRight, isAgainstWallRightBottom, isAgainstWallRightTop, isAgainstWallRightMid;
    public float wallRightCheckDistances;
    public float wallRightYCheckOffset;
    public float wallRightXCheckOffset;
    public float wallRightDuplicatesYOffset;

    public bool hasEnoughSpaceToStand;
    public float standSpaceCheckDistances;
    public float standSpaceYCheckOffset;
    public float standSpaceXCheckOffset;


    [Separator("Combat State Variables")]
    public bool inCombat;   
    public float combatStateTime;
    public float combatStateTargetTime = 7f;
    private GameObject onscreenTimer;

    [Separator("Melee Variables")]
    public GameObject MeleeObject;
    public float attackTimerTargetTime;
    private float attackTimer;
    public float attackComboTargetTime;
    [HideInInspector] public float attackComboTimer;
    [HideInInspector] public int attackdamageMax;
    [HideInInspector] public int attackdamageMin;
    //private SpriteRenderer meleeSpriteR;
    //private ParticleSystem particles;

    [Separator("Dialogue Variables")]
    public bool playerInConversation;

    public float switchingDirTime;
    public float switchingDirTargetTime = 0.2f;
    public bool switchedDirToLeft, switchedDirToRight;
    //public GameObject switchedDirOnscreenTimer;

    

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
        COMBAT_Dodging,
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
        Switching_to_Crouching,
        Switching_from_Crouching,
        Crouching_Idle,
        Crouching_Walking,
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
        //switchedDirOnscreenTimer = TimerManager.instance.CreateTimer(switchingDirTime, switchingDirTargetTime, "Switching Direction Timer", false);

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
        //Debug.Log($"Current state is {currentState}");
        xVel = rb2d.velocity.x;
        yVel = rb2d.velocity.y;
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        onscreenTimer.GetComponent<SimpleTimerScript>().timerTime = combatStateTime;
        playerInConversation = DialogueManager.instance.playerInConversation;
        //switchedDirOnscreenTimer.GetComponent<SimpleTimerScript>().timerTime = switchingDirTime;

        //Crouching Stuff---------------------------------------------------------------------------------------------------------------------------------------
        if (inCrouchingTrigger && Input.GetButtonDown("Interact"))
        {
            if (currentState != State.Crouching_Idle)
            {
                currentState = State.Switching_to_Crouching;
            }
            else
            {
                if (!hasEnoughSpaceToStand)
                {
                    Debug.LogWarning($"Player does not have enough space to stand!");
                }
                else
                {
                    currentState = State.Switching_from_Crouching;
                }
            }
        }
        //Crouching Stuff---------------------------------------------------------------------------------------------------------------------------------------


        if (inCombat) { combatStateTime -= Time.deltaTime; }
        else { combatStateTime = combatStateTargetTime; }
        if(combatStateTime < 0) { combatStateTime = 0; }

        switchingDirTime -= Time.deltaTime;
        if (switchingDirTime < 0) { switchingDirTime = 0; switchedDirToLeft = false; switchedDirToRight = false; }

        if (currentState == State.Dodging || currentState == State.COMBAT_Dodging)
        {
            ApplyRollDrag();
        }
        else
        {
            if (isGrounded) { ApplyGroundLinearDrag(); }
            else { ApplyAirLinearDrag(); }
        }

        //The great state machine ------------------------------------------------------------------------------------------------------------------------------
        switch (currentState)
        {
            case State.COMBAT_Idle:
                charattacks.Combat_Idle();
                charanimation.ClearComboBuffer();
                inCombat = true;

                if (combatStateTime == 0) { currentState = State.Idle; }

                if (Input.GetAxisRaw("Vertical") > 0) { currentState = State.COMBAT_Jumping; }
                if (Input.GetAxisRaw("Horizontal") != 0) { currentState = State.COMBAT_Running; }
                if (Input.GetButtonDown("Dodge")) { currentState = State.COMBAT_Dodging; }
                break;

            case State.COMBAT_Walking:
                inCombat = true;
                charanimation.ClearComboBuffer();
                combatStateTime = combatStateTargetTime;

                if (Input.GetAxisRaw("Vertical") > 0) { currentState = State.COMBAT_Jumping; }
                if (Input.GetButtonDown("Dodge")) { currentState = State.COMBAT_Dodging; }
                break;

            case State.COMBAT_Running:
                inCombat = true;
                charattacks.Combat_Running();
                charanimation.ClearComboBuffer();
                combatStateTime = combatStateTargetTime;
                checkforSwitchingDir();

                if (Input.GetAxisRaw("Vertical") > 0) { currentState = State.COMBAT_Jumping; }
                if (Mathf.Abs(Mathf.Ceil(rb2d.velocity.x)) == 0) { currentState = State.COMBAT_Idle; }
                if (Input.GetButtonDown("Dodge")) { currentState = State.COMBAT_Dodging; }
                //Transition back to Walk
                //Nothing haha fuck you
                break;

            case State.COMBAT_Jumping:
                inCombat = true;
                combatStateTime = combatStateTargetTime;
                charanimation.ClearComboBuffer();
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
                charanimation.ClearComboBuffer();
                combatStateTime = combatStateTargetTime;
                AirJump();
                //Transition to Falling
                if (rb2d.velocity.y < fallThreshold)
                {
                    currentState = State.COMBAT_Falling;
                }
                break;

            case State.COMBAT_Falling:
                inCombat = true;
                charanimation.ClearComboBuffer();
                combatStateTime = combatStateTargetTime;
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
                combatStateTime = combatStateTargetTime;
                break;

            case State.COMBAT_Attacking:        //Transition to this is handled in charattacks AnimateCombos() and Charanimation ManageComboBuffer()
                inCombat = true;
                combatStateTime = combatStateTargetTime;
/*                if (!charanimation.currentlyComboing)
                {
                    currentState = State.COMBAT_Idle;
                }*/
                break;

            case State.COMBAT_Dodging:
                inCombat = true;
                charanimation.ClearComboBuffer();
                combatStateTime = combatStateTargetTime;
                //Animation is called from Charanimation
                break;

            case State.COMBAT_Air_Attacking:
                inCombat = true;
                combatStateTime = combatStateTargetTime;
                if (isGrounded)
                {
                    currentState = State.COMBAT_Idle;
                }
                Falling();
                break;

            case State.COMBAT_Rolling:
                inCombat = true;
                combatStateTime = combatStateTargetTime;
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
                    if (allowWalking) 
                    { 
                        currentState = State.Walking;
                    }
                    else
                    {
                        currentState = State.Running;
                    }
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
                checkforSwitchingDir();
                //Transition back to Idle
                //if (Input.GetAxisRaw("Horizontal") == 0 /*&& Mathf.Abs(Mathf.Ceil(rb2d.velocity.x)) == 0*/)
                if (Mathf.Abs(Mathf.Ceil(rb2d.velocity.x)) == 0)
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

            case State.Switching_to_Crouching:
                //Actual switching to Crouching_Idle is handled in Charanimation
                rb2d.velocity = rb2d.velocity;
                break;

            case State.Switching_from_Crouching:
                //Actual switching from Crouching_Idle is handled in Charanimation
                rb2d.velocity = rb2d.velocity;
                break;

            case State.Crouching_Idle:
                inCombat = false;
                if (Input.GetAxisRaw("Horizontal") != 0) { currentState = State.CrouchWalking; }
                //boxCol.size = boxColSize;
                //boxCol.offset = boxColOffset;

                if (!inCrouchingTriggerStayZone)
                {
                    currentState = State.Switching_from_Crouching;
                }

                rb2d.velocity = rb2d.velocity;
                break;

            case State.CrouchWalking:
                CrouchingWalking();
                inCombat = false;

                if (Input.GetAxisRaw("Horizontal") == 0)
                {
                    currentState = State.Crouching_Idle;
                }

                if (!isGrounded)
                {
                    currentState = State.Falling;
                }
                break;

            case State.Attacking:
                inCombat = false;
                break;

            case State.Air_Attacking:
                inCombat = false;
                break;
            case State.Dodging:     //Is the same as rolling
                inCombat = false;
                //Animation is called in Charanimation. The last frame of the animation calls onDodgeTransition from here, transitioning back to idle.
                break;

            case State.Stunned:
                inCombat = false;
                break;

            case State.Dead:
                inCombat = false;
                Dead();
                break;

            default:
                inCombat = false;
                break;
        }

        currentDrag = GetComponent<Rigidbody2D>().drag;

        if (Input.GetAxisRaw("Horizontal") == 1) { facingDir = 1; }
        if (Input.GetAxisRaw("Horizontal") == -1) { facingDir = -1; }

        FindClosestNPC();
        checkforGrounded();
        checkforWallsAndStandingSpace();
    }

    void checkforGrounded()
    {
        
        //Grounded Check
        if (Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y + groundYCheckOffset), groundCheckDistances, 1 << LayerMask.NameToLayer("Ground")))
        { isGrounded = true; }
        else { isGrounded = false; }
    }

    void checkforWallsAndStandingSpace()
    {
        LayerMask groundAndWalls = LayerMask.GetMask("Ground", "Walls");
        //Check for left wall main circle
        if (Physics2D.OverlapCircle(new Vector2(transform.position.x + wallLeftXCheckOffset, transform.position.y + wallLeftYCheckOffset), wallLeftCheckDistances, groundAndWalls))
        { isAgainstWallLeftMid = true; }
        else { isAgainstWallLeftMid = false; }

        //Check for left wall duplicates
        //Duplicate 1
        if (Physics2D.OverlapCircle(new Vector2(transform.position.x + wallLeftXCheckOffset, transform.position.y + wallLeftYCheckOffset + wallLeftDuplicatesYOffset), wallLeftCheckDistances, groundAndWalls))
        { isAgainstWallLeftTop = true; Debug.Log("Collided at top"); }
        else { isAgainstWallLeftTop = false; }
        //Duplicate 2
        if (Physics2D.OverlapCircle(new Vector2(transform.position.x + wallLeftXCheckOffset, transform.position.y + wallLeftYCheckOffset - wallLeftDuplicatesYOffset), wallLeftCheckDistances, groundAndWalls))
        { isAgainstWallLeftBottom = true; Debug.Log("Collided at bottom"); }
        else { isAgainstWallLeftBottom = false; }


        //Check for right wall main circle
        if (Physics2D.OverlapCircle(new Vector2(transform.position.x + wallRightXCheckOffset, transform.position.y + wallRightYCheckOffset), wallRightCheckDistances, groundAndWalls))
        { isAgainstWallRightMid = true; }
        else { isAgainstWallRightMid = false; }

        //Check for right wall duplicates
        //Duplicate1
        if (Physics2D.OverlapCircle(new Vector2(transform.position.x + wallRightXCheckOffset, transform.position.y + wallRightYCheckOffset + wallRightDuplicatesYOffset), wallRightCheckDistances, groundAndWalls))
        { isAgainstWallRightTop = true; Debug.Log("Collided at top"); }
        else { isAgainstWallRightTop = false; }
        //Duplicate2
        if (Physics2D.OverlapCircle(new Vector2(transform.position.x + wallRightXCheckOffset, transform.position.y + wallRightYCheckOffset - wallRightDuplicatesYOffset), wallRightCheckDistances, groundAndWalls))
        { isAgainstWallRightBottom = true; Debug.Log("Collided at bottom"); }
        else { isAgainstWallRightBottom = false;
        }

        //Check for a ceiling
        if (Physics2D.OverlapCircle(new Vector2(transform.position.x + standSpaceXCheckOffset, transform.position.y + standSpaceYCheckOffset), standSpaceCheckDistances, groundAndWalls))
        { hasEnoughSpaceToStand = false; }
        else { hasEnoughSpaceToStand = true; }

        if (isAgainstWallLeftBottom || isAgainstWallLeftTop || isAgainstWallLeftMid)
        {
            isAgainstWallLeft = true;
        }
        else
        {
            isAgainstWallLeft = false;
        }

        if (isAgainstWallRightBottom || isAgainstWallRightTop || isAgainstWallRightMid)
        {
            isAgainstWallRight = true;
        }
        else
        {
            isAgainstWallRight = false;
        }
    }

    void checkforSwitchingDir()
    {
        if (Input.GetAxisRaw("Horizontal") == 1)    //If moving right....
        {
            switchingDirTime = switchingDirTargetTime;
        }

        if (Input.GetAxisRaw("Horizontal") == -1)    //If moving left....
        {
            switchingDirTime = switchingDirTargetTime;
        }

        if (switchingDirTime > 0 && facingDir == 1) //If within the switchingdirtime and you're moving right
        {
            if (Input.GetAxisRaw("Horizontal") == -1)   //...and you press left
            {
                switchedDirToLeft = true;
            }
        }

        if (switchingDirTime > 0 && facingDir == -1) //If within the switchingdirtime and you're moving left
        {
            if (Input.GetAxisRaw("Horizontal") == 1)   //...and you press right
            {
                switchedDirToRight = true;
            }
        }
        //If holding down a movement key, make timer = timertime
        //If timertime is still above 0 and player presses the other key, we know we switched direction
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

    public void CrouchingWalking()
    {
        rb2d.velocity = new Vector2(crouchWalkingSpeed * Input.GetAxis("Horizontal"), rb2d.velocity.y);
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
        //Against a right wall, only allows left movement
        if (!isAgainstWallLeft && isAgainstWallRight)
        {
            rb2d.velocity = new Vector2((Input.GetAxisRaw("Horizontal") * airHorizontalAcc), rb2d.velocity.y);  //Allows horizontal movement
        }

        //Against a left wall, only allows right movement
        if (!isAgainstWallRight && isAgainstWallLeft)
        {
            rb2d.velocity = new Vector2((Input.GetAxisRaw("Horizontal") * airHorizontalAcc), rb2d.velocity.y);  //Allows horizontal movement
        }


        //If moving right and not moving into a right wall...
        if (Input.GetAxisRaw("Horizontal") > 0 && !isAgainstWallRight)
        {
            rb2d.velocity = new Vector2((Input.GetAxisRaw("Horizontal") * airHorizontalAcc), rb2d.velocity.y);  //Allows horizontal movement
        }

        //If moving left and not moving to a left wall, allow movement
        if (Input.GetAxisRaw("Horizontal") < 0 && !isAgainstWallLeft)
        {
            rb2d.velocity = new Vector2((Input.GetAxisRaw("Horizontal") * airHorizontalAcc), rb2d.velocity.y);  //Allows horizontal movement
        }
        if (!jumped)
        {
            //Debug.Log("Jumping");
            //rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Force); 
            rb2d.velocity = new Vector2((Input.GetAxisRaw("Horizontal") * airHorizontalAcc), jumpForce);    //Adds jumpforce once
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
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0);                                           //Sets Y velocity to 0 so calculations are consistent
            rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);                                //Adds a force upwards
            //rb2d.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * runSpeed * airHorizontalAcc, 0f));
            if (!isAgainstWallLeft && !isAgainstWallRight)
            {
                rb2d.velocity = new Vector2((Input.GetAxisRaw("Horizontal") * airHorizontalAcc), rb2d.velocity.y);  //Allows horizontal movement
            }
            jumped = true;
            --airJumpsHas;
            currentState = State.Jumping;
        }
        canFlipXDir();
    }

    public void Falling()
    {
        //rb2d.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * airHorizontalAcc, 0f));
        //If moving right and not moving into a right wall...
        if (Input.GetAxisRaw("Horizontal") > 0 && !isAgainstWallRight)
        {
            rb2d.velocity = new Vector2((Input.GetAxisRaw("Horizontal") * airHorizontalAcc), rb2d.velocity.y);  //Allows horizontal movement
        }

        //If moving left and not moving to a left wall, allow movement
        if (Input.GetAxisRaw("Horizontal") < 0 && !isAgainstWallLeft)
        {
            rb2d.velocity = new Vector2((Input.GetAxisRaw("Horizontal") * airHorizontalAcc), rb2d.velocity.y);  //Allows horizontal movement
        }
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

    private Vector3 scale;
    private bool scaleSet = false;
    public void canFlipXDir()
    {
        if (scaleSet == false) { scale = transform.localScale; scaleSet = true; }
        //FLIP THOSE ANIMS BABY
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            transform.localScale = scale;
        }

        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
        }
    }

    public void onDodge(int force)  //This function called on the last frame of the dodge animation via AnimationEvent
    {
        rb2d.velocity = new Vector2(0f, 0f);
        rb2d.AddForce(new Vector2 (rb2d.velocity.x + (force * facingDir), rb2d.velocity.y));
        rolled = true;
    }

    public void onDodgeTransition() //This function called on the last frame of the dodge animation via AnimationEvent
    {
        currentState = State.Idle;
    }

    public void onDodgeTransitionCombat()
    {
        currentState = State.COMBAT_Idle;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //This is the area the player can press interact in to crouch
        if (collision.CompareTag("crouching_trigger_area"))
        {
            inCrouchingTrigger = true;
        }

        //This is the area the player must stay in to stay crouched. If they leave it they are automatically uncrouched
        if (collision.CompareTag("crouching_trigger_stay_zone"))
        {
            inCrouchingTriggerStayZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("crouching_trigger_area"))
        {
            inCrouchingTrigger = false;
        }

        if (collision.CompareTag("crouching_trigger_stay_zone"))
        {
            inCrouchingTriggerStayZone = false;
        }
    }

    private void OnDrawGizmos()
    {
        //GroundCheck check
        if (isGrounded) { Gizmos.color = Color.green; } else { Gizmos.color = Color.white; }
        Gizmos.DrawWireSphere(new Vector2(transform.position.x, transform.position.y + groundYCheckOffset), groundCheckDistances);

        //Wall left check Gizmo
        if (isAgainstWallLeftMid) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }
        Gizmos.DrawWireSphere(new Vector2(transform.position.x + wallLeftXCheckOffset, transform.position.y + wallLeftYCheckOffset), wallLeftCheckDistances);
        //Wall left check Dupe 1
        if (isAgainstWallLeftTop) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }
        Gizmos.DrawWireSphere(new Vector2(transform.position.x + wallLeftXCheckOffset, transform.position.y + wallLeftYCheckOffset + wallLeftDuplicatesYOffset), wallLeftCheckDistances);
        //Wall left check Dupe 2
        if (isAgainstWallLeftBottom) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }
        Gizmos.DrawWireSphere(new Vector2(transform.position.x + wallLeftXCheckOffset, transform.position.y + wallLeftYCheckOffset - wallLeftDuplicatesYOffset), wallLeftCheckDistances);

        //Wall right check Gizmo
        if (isAgainstWallRightMid) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }
        Gizmos.DrawWireSphere(new Vector2(transform.position.x + wallRightXCheckOffset, transform.position.y + wallRightYCheckOffset), wallRightCheckDistances);
        //Wall right check Dupe 1
        if (isAgainstWallRightTop) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }
        Gizmos.DrawWireSphere(new Vector2(transform.position.x + wallRightXCheckOffset, transform.position.y + wallRightYCheckOffset + wallRightDuplicatesYOffset), wallRightCheckDistances);
        //Wall right check Dupe 2
        if (isAgainstWallRightBottom) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }
        Gizmos.DrawWireSphere(new Vector2(transform.position.x + wallRightXCheckOffset, transform.position.y + wallRightYCheckOffset - wallRightDuplicatesYOffset), wallRightCheckDistances);


        //Standing space check Gizmo
        if (hasEnoughSpaceToStand) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }
        Gizmos.DrawWireSphere(new Vector2(transform.position.x + standSpaceXCheckOffset, transform.position.y + standSpaceYCheckOffset), standSpaceCheckDistances);
    }

}