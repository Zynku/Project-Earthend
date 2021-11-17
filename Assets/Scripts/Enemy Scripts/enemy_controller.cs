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
    public bool dead, appliedDeadKB;
    public float stunchance, stunTime, attackChance;
    public float coolDownTimer = 0f, coolDownTargetTime = 0.2f;
    public float attackdamageMax = 0;
    public float attackdamageMin = 0;
    public Vector2 playerPos;
    public Vector2 myPos;
    public Vector2 speed;
    public float deadKnockBX;
    public float deadKnockBY;
    public float collisionDir;
    public float despawnTime;

    [Header("Melee Variables")]
    public GameObject Melee1;

    Rigidbody2D rb2d;
    GameObject Player;

    public enum State
    {
        Spawning,
        Idle,
        Attacking,
        MovingToPlayer,
        MovingAwayFromPlayer,
        Stunned,
        Dodging,
        Waiting,
        Dead
    }
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        Melee1.SetActive(false);
        currentState = State.Idle;
        coolDownTimer = coolDownTargetTime;
        Player = gamemanager.instance.Player;
    }

    private void Update()
    {
        
        playerPos = Player.transform.position;
        collisionDir = GetComponent<Enemyhealth>().collisionDir;
        myPos = transform.position;
        speed = rb2d.velocity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //In Sense Radius?
        if (Mathf.Abs(playerDist) < senseRadius) {canFollowPlayer = true;}
        else { canFollowPlayer = false;}

        //In attack radius
        if (Mathf.Abs(playerDist) < attackRadius) {canAttackPlayer = true;}
        else {canAttackPlayer = false;}

        //In too close radius
        if (Mathf.Abs(playerDist) < playerTooCloseRadius) {playerTooClose = true;}
        else {playerTooClose = false;}

        if (GetComponent<Enemyhealth>().currentHealth <= 0)
        {
            currentState = State.Dead;
        }

        //Check for stunnedcheck from enemy_collider script, runs random chance to determine if stunned. Chance = 1/(stunchance + 1)
        stuncheck = GetComponent<Enemyhealth>().stunnedcheck;
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
                    transform.localScale = new Vector2(-insideRadiusDir * 1.5f, 1.5f);
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
                //Start Timer counting down
                coolDownTimer -= Time.fixedDeltaTime;
                //When it reaches zero or less, attack, start cooldown with a 0.1s delay
                if (coolDownTimer <= 0)
                {
                    attack = true;
                    Invoke("AttackCoolDown", 0.1f);
                }

                //Face the player
                transform.localScale = new Vector2(insideRadiusDir * 1.5f, 1.5f);

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
                dead = true;
                if (appliedDeadKB == false)
                {
                    rb2d.AddForce(new Vector2(Random.Range(deadKnockBX, deadKnockBX + 0.5f) * collisionDir, deadKnockBY), ForceMode2D.Impulse);
                    appliedDeadKB = true;
                }
                Invoke("DestroyGameObject", despawnTime);
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
            transform.localScale = new Vector2(-1.5f, 1.5f);
        }

        //Moving Right
        if (rb2d.velocity.x > 0.05f && currentState == State.MovingToPlayer)
        {
            movingDir = 1f;
            transform.localScale = new Vector2(1.5f, 1.5f);
        }

    }

    public void AttackCoolDown()
    {
        coolDownTimer = coolDownTargetTime;
        attack = false;
    }

    public void onMelee1Start()
    {
        Melee1.SetActive(true);
    }

    public void onMelee1End()
    {
        Melee1.SetActive(false);
    }

    public void DestroyGameObject()
    {
        Destroy(gameObject);
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
