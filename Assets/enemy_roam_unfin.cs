using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_roam_unfin : MonoBehaviour
{
    Rigidbody2D rb2d;
    
    private State currentState;
    private bool groundDetected, wallDetected;
    [SerializeField] private Transform groundCheck, wallCheck;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float groundCheckdist, wallCheckdist;

    public Vector2 roamToPos;
    public float decisionTimer, decisionTimerAmount = 5f;
    public float pointDirection;
    public float moveSpeed;

    private enum State
    {
        Idle,
        Roaming,
        Attacking,
        Dead
    }

    private void Awake()
    {
        currentState = State.Roaming;
    }

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        decisionTimer = decisionTimerAmount;
    }

    private void FixedUpdate()
    {
        decisionTimer -= Time.fixedDeltaTime;
        


        if (transform.position.x - roamToPos.x > 0)
        {
            pointDirection = -1;
        }
        if (transform.position.x - roamToPos.x < 0)
        {
            pointDirection = 1;
        }
    }

    
    private void Update()
    {


        switch (currentState)
        {
            case State.Idle:
                //Idle for a set amount of time. At end, random chance to idle again or Roam
                break;
            case State.Roaming:
                //Pick a random location a certain distance from transform. After random amount of time walk to it. When reached State.Idle a random amount of time, pick a new position, repeat
                if (decisionTimer <= 0)
                {
                    rb2d.velocity = new Vector2(0f, 0f);
                    roamToPos = new Vector2((transform.position.x + Random.Range(-2.0f, 2.0f)), transform.position.y);
                    decisionTimer = decisionTimerAmount;
                }
                rb2d.AddForce(new Vector2(moveSpeed * pointDirection, 0));
                break;
            case State.Attacking:
                //Attacking
                break;
            case State.Dead:
                //Deadass lmao
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color= Color.white;
        Gizmos.DrawSphere(roamToPos, 0.1f);
        
    }
}
