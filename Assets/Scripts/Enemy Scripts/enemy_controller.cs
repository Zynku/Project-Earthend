using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_controller : MonoBehaviour
{
    public State currentState;
    public float senseRadius, attackRadius;
    public float insideRadiusDir, playerDist, moveSpeed, maxMoveSpeed, jumpForce, movingDir;
    public bool canFollowPlayer, canAttackPlayer;
    public float wallCheckLength, groundCheckDistance;
    public bool wallInfront, isGrounded, attack;
    public float attackTargetTime = 0.2f;
    public Vector2 playerPos;
    public Vector2 myPos;

    Rigidbody2D rb2d;

    public enum State
    {
        Idle,
        Attacking,
        MovingToPlayer,
        Dead
    }
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        currentState = State.Idle;
    }

    private void Update()
    {
        
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

        if (Mathf.Abs(playerDist) < attackRadius)
        {
            canAttackPlayer = true;
        }
        else
        {
            canAttackPlayer = false;
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
            case State.Attacking:
                //-------------------------------------------------------------------------------------------------------------------------------------
                canFollowPlayer = false;

                StartCoroutine(AttackCoolDown());

                if (attack == true)
                {
                    Debug.Log("HIYAAAA");
                }

                //Exit Condition
                if (canAttackPlayer == false)
                {
                    currentState = State.MovingToPlayer;
                }
                break;
            //-------------------------------------------------------------------------------------------------------------------------------------
            case State.Dead:
                //-------------------------------------------------------------------------------------------------------------------------------------

                break;
        }

        playerPos = GameObject.FindWithTag("Player").transform.position;
        myPos = transform.position;
        PlayerInRadius();
        EnvironmentChecks();
        MovementDir();
    }

    public IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(attackTargetTime);
        attack = true;
    }

    public void PlayerInRadius()
    {
        insideRadiusDir = Mathf.Sign(-transform.position.x + playerPos.x);
        playerDist = playerPos.x - transform.position.x;
    }

    public void MovementDir()
    {
        //Moving Left
        if (rb2d.velocity.x < 0)
        {
            movingDir = -1f;
            transform.localScale = new Vector2(-1, 1);
        }

        //Moving Right
        if (rb2d.velocity.x > 0)
        {
            movingDir = 1f;
            transform.localScale = new Vector2(1, 1);
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

        //Attack player raidus
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        //Line to player position
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, playerPos);
    }
}
