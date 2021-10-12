using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charcontrol_wastebin : MonoBehaviour
{
    public static Charcontrol Instance;
    Animator animator;
    Rigidbody2D rb2d;
    BoxCollider2D boxCol;

    public State currentState;

    public bool tree;

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
        Rolling,
        Ledgegrabbing,
        Ledgejumping,
        Stunned,
        Dead
    }

    [Header("Variables")]
    public float xVel;
    public float yVel;
    public float inputX;
    public float inputY;
    //public static GameObject closestNPC;
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

    [Header("Crouch Hitbox Variables")]
    public Vector2 boxColSize;
    public Vector2 boxColOffset;
    public Vector2 boxColCrouchSize;
    public Vector2 boxColCrouchOffset;

    [Header("Roll Variables")]
    public float rollForce;
    public float rollDrag;
    [HideInInspector] public bool rolled = false;

    [Header("Jump Variables")]
    public static bool isGrounded;
    [HideInInspector] public bool jumped;
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
        particles = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void FixedUpdate()
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
            if (isGrounded) { ApplyGroundLinearDrag(); }
            else { ApplyAirLinearDrag(); }
        }
        else
        {
            ApplyRollDrag();
        }


        //checkforSlope();
        FindClosestNPC();

        //-------------------------------------------------------------------------------------------------------------------------------------
        switch (currentState)
        {
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
                if (Input.GetAxisRaw("Vertical") < 0)
                {
                    currentState = State.Rolling;
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
                break;

            case State.Jumping:
                Jump();
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
                if (!Input.GetButton("Vertical"))
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

            case State.Rolling:
                Rolling();
                //Transition back to Running
                if (!Input.GetButton("Vertical"))
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

    void checkforSlope()
    {
        //THIS SHIT IS FUCKING BROKE. ENABLING ABSOLUTELY DECIMATES THE FIND CLOSEST NPC METHOD AND THE CHECK FOR GROUNDED METHOD. WHY? NO IDEA. UN COMMENT IN
        //FIXED UPDATE AT YOUR OWN FUCKING RISK
        //Checks for SLOPES
        Collider2D[] result = new Collider2D[1]; // requires a size, it will return only up to a max of the size available.
        ContactFilter2D filter2D = new ContactFilter2D();
        filter2D.layerMask = LayerMask.NameToLayer("Ground_slopes");
        Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y + ycheckOffset), checkdistances, filter2D, new Collider2D[1]);
        Debug.Log(result[0].gameObject);
        if (result[0].gameObject.CompareTag("Ground_slopes"))
        {
            Debug.Log("on a slope");
        }
    }

    public void Idle()
    {
        currentState = State.Idle;
        airJumpsHas = airJumps;
        rolled = false;

        attackTimer = attackTimerTargetTime;
        attackComboTimer = attackComboTargetTime;
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
        rolled = false;
    }

    public void Attacking()
    {
        //Dictates whether in attack state or not based on time
        attackTimer -= Time.deltaTime;
        if (attackTimer < 0) { attackTimer = 0; currentState = State.Idle; }

        //Dictates speed at which you can transition to next melee state based on when you press attack key
        attackComboTimer -= Time.deltaTime;
        if (attackComboTimer < 0) { attackComboTimer = 0; }

        if (attackComboTimer > 0)
        {
            animator.SetTrigger("Melee");
            attackComboTimer = attackComboTargetTime;
        }
    }

    /*    public void Sliding()
        {
            if (slid == false)
            {
                slid = true;
                rb2d.drag = slideDrag;
                rb2d.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * slideForce, 0f));
            }
        }*/

    public void Rolling()
    {
        if (!rolled)
        {
            rolled = true;
            rb2d.drag = rollDrag;
            rb2d.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * rollForce, 0f));
        }
    }

    public void Crouching()
    {
        rb2d.velocity = new Vector2(0, 0);
        boxCol.size = boxColCrouchSize;
        boxCol.offset = boxColCrouchOffset;
    }

    public void Jump()
    {
        if (jumped == false)
        {
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

    public void Falling()
    {
        rb2d.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * airHorizontalAcc, 0f));
    }

    public void OnMelee1Start()
    {
        //Used by Animation Events in Melee Animation. On Event 1 activates hitbox, on Event 2 deactivates hitbox
        MeleeObject.SetActive(true);
        var emission = particles.emission; // Stores the module in a local variable
        emission.enabled = true; // Applies the new value directly to the Particle 
    }

    public void OnMelee1End()
    {
        MeleeObject.SetActive(false);
        var emission = particles.emission; // Stores the module in a local variable
        emission.enabled = false; // Applies the new value directly to the Particle 
    }

    public void OnMeleeForceAdd(int forceX)
    {
        rb2d.AddForce(new Vector2(facingDir * forceX, 0), ForceMode2D.Impulse);
    }

    public void SetMeleeSprite(Sprite sprite)
    {
        meleeSpriteR.sprite = sprite;
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
        //closestNPC = closest;
    }

    private void OnDrawGizmos()
    {
        //Ground check sphere
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector2(transform.position.x, transform.position.y + ycheckOffset), checkdistances);
    }
}

