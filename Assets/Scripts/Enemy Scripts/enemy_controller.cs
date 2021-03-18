using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_controller : MonoBehaviour
{
    public State currentState;
    public float senseRadius, attackRadius, playerTooCloseRadius;
    public float insideRadiusDir, playerDist, moveSpeed, maxMoveSpeed, jumpForce, movingDir;
    public bool canFollowPlayer, canAttackPlayer, playerTooClose;
    public bool inControl;
    public float wallCheckLength, groundCheckDistance;
    public bool wallInfront, isGrounded, attack, stuncheck;
    public bool stunned;
    public float stunchance, stunTime;
    public float coolDownTimer = 0f, coolDownTargetTime = 0.2f;
    public Vector2 playerPos;
    public Vector2 myPos;
    public Vector2 speed;

    Rigidbody2D rb2d;

    public enum State
    {
        Idle,
        Attacking,
        MovingToPlayer,
        MovingAwayFromPlayer,
        Stunned,
        Dead
    }
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        
        currentState = State.Idle;
        coolDownTimer = coolDownTargetTime;
    }

    private void Update()
    {
        //If cooldown is greater than 0
        if (coolDownTimer > 0f)
        {
            //start timer, if cooldown is less than 0
            coolDownTimer -= Time.deltaTime;
            if (coolDownTimer < 0f)
            {
                //cooldown is 0
                coolDownTimer = 0f;
            }
        }

        
        playerPos = GameObject.FindWithTag("Player").transform.position;
        myPos = transform.position;
        speed = rb2d.velocity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //In Sense Radius?
        if (Mathf.Abs(playerDist) < senseRadius)
        {
            canFollowPlayer = true;
        }
        else
        {
            canFollowPlayer = false;
        }

        //In attack radius
        if (Mathf.Abs(playerDist) < attackRadius)
        {
            canAttackPlayer = true;
        }
        else
        {
            canAttackPlayer = false;
        }

        //In too close radius
        if (Mathf.Abs(playerDist) < playerTooCloseRadius)
        {
            playerTooClose = true;
        }
        else
        {
            playerTooClose = false;
        }

        stuncheck = GetComponentInChildren<enemy_collider>().stunnedcheck;
        if (stuncheck == true)
        {
            if ((Mathf.Ceil(Random.Range(0f, stunchance)) == 1f))
            {
                currentState = State.Stunned;
                stunned = true;
            }
        }


        switch (currentState)
        {


                //-------------------------------------------------------------------------------------------------------------------------------------
            case State.Idle:
                //-------------------------------------------------------------------------------------------------------------------------------------
                if (canFollowPlayer == true)
                {
                    currentState = State.MovingToPlayer;
                }
                break;




                //-------------------------------------------------------------------------------------------------------------------------------------
            case State.MovingToPlayer:
                //-------------------------------------------------------------------------------------------------------------------------------------
                //Check for player and move towards them if inside sense radius
                if (canFollowPlayer == true)
                {
                    rb2d.AddForce(new Vector2(insideRadiusDir, 0f) * moveSpeed);
                    if (Mathf.Abs(rb2d.velocity.x) > maxMoveSpeed)
                    {
                        rb2d.velocity = new Vector2((maxMoveSpeed * insideRadiusDir), rb2d.velocity.y);
                    }
                }

                //If it finds a wall, it jumps
                if (wallInfront == true && isGrounded)
                {
                    Jump();
                }

                if (canAttackPlayer)
                {
                    currentState = State.Attacking;
                }

                if (!canFollowPlayer)
                {
                    currentState = State.Idle;
                }

                attack = false;

                break;





                //-------------------------------------------------------------------------------------------------------------------------------------
            case State.MovingAwayFromPlayer:
                //-------------------------------------------------------------------------------------------------------------------------------------
                //Checks to see if inside radius, if player is, move away
                rb2d.AddForce(new Vector2(-insideRadiusDir, 0f) * moveSpeed);
                if (Mathf.Abs(rb2d.velocity.x) > maxMoveSpeed)
                {
                    rb2d.velocity = new Vector2((maxMoveSpeed * -insideRadiusDir), rb2d.velocity.y);
                    transform.localScale = new Vector2(-insideRadiusDir, 1);
                }

                //If outside too close radius, return to attacking
                if (Mathf.Abs(playerDist) > playerTooCloseRadius)
                {
                    currentState = State.Attacking;
                }

                coolDownTimer = coolDownTargetTime;
               
                break;





                //-------------------------------------------------------------------------------------------------------------------------------------
            case State.Attacking:
                //-------------------------------------------------------------------------------------------------------------------------------------
                canFollowPlayer = false;
                //STOP MOVING
                rb2d.velocity = new Vector2(0f, rb2d.velocity.y);

                //Sets attack timer to 0, attack is true at 0. If attack, reset timer to cooldown
                attack = false;
                if (coolDownTimer == 0f)
                {
                    attack = true;
                    coolDownTimer = coolDownTargetTime;
                }

                //Face the player
                transform.localScale = new Vector2(insideRadiusDir, 1);

                //If inside radius, stop attacking, move to keep player at radius edge
                if (playerTooClose == true)
                {
                    currentState = State.MovingAwayFromPlayer;
                }

                //Exit Condition
                if (canAttackPlayer == false)
                {
                    currentState = State.MovingToPlayer;
                }
                break;





            //-------------------------------------------------------------------------------------------------------------------------------------
            case State.Stunned:
                //-------------------------------------------------------------------------------------------------------------------------------------
                if (stunned == true) 
                {
                    StartCoroutine(StunnedState()); 
                }
                break;
            



            //-------------------------------------------------------------------------------------------------------------------------------------
            case State.Dead:
            //-------------------------------------------------------------------------------------------------------------------------------------

                break;
        }

        PlayerInRadius();
        EnvironmentChecks();
        MovementDir();
    }

    public void PlayerInRadius()
    {
        insideRadiusDir = Mathf.Sign(-transform.position.x + playerPos.x);
        playerDist = playerPos.x - transform.position.x;
    }

    public void MovementDir()
    {
        //Moving Left
        if (rb2d.velocity.x < -0.05f && currentState == State.MovingToPlayer)
        {
            movingDir = -1f;
            transform.localScale = new Vector2(-1, 1);
        }

        //Moving Right
        if (rb2d.velocity.x > 0.05f && currentState == State.MovingToPlayer)
        {
            movingDir = 1f;
            transform.localScale = new Vector2(1, 1);
        }

    }

    IEnumerator StunnedState()
    {
        //Suspends code for *stunTime* seconds, returns to idle after
        stunned = false;
        yield return new WaitForSeconds(stunTime);

        //If inside attack radius, go back to attacking, if not go to idle
        if (Mathf.Abs(playerDist) < attackRadius)
        {
            currentState = State.Attacking;
        }
        else
        {
            currentState = State.Idle;
        }
    }

    public void EnvironmentChecks()
    {
        //Front Wallcheck
        if (Physics2D.Raycast(new Vector2((transform.position.x), (transform.position.y + 0.15f)), (Vector2.right * movingDir), wallCheckLength, 1 << LayerMask.NameToLayer("Ground")))
        {
            wallInfront = true;
        }
        else 
        { 
            wallInfront = false; 
        }

        //Grounded Check
        if (Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, 1 << LayerMask.NameToLayer("Ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void Jump()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0f);
        rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnDrawGizmos()
    {
        //Ground Check Ray
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector2.down * groundCheckDistance);

        //Wall Check Ray
        Gizmos.color = Color.green;
        Gizmos.DrawRay(new Vector2(transform.position.x, transform.position.y + 0.15f), Vector2.right * wallCheckLength * movingDir);

        //Sense player radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, senseRadius);

        //Attack player radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        //Player too close radius
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, playerTooCloseRadius);

        //Line to player position
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, playerPos);
    }
}
