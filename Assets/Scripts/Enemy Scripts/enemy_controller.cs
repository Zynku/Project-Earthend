using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_controller : MonoBehaviour
{
    private State currentState;
    public float senseradius;
    public float insideRadiusDir;
    public bool insideRadius;
    public Vector2 playerPos;
    public Vector2 myPos;

    private enum State
    {
        Attacking,
        MovingtoPlayer,
        Dead
    }
    // Start is called before the first frame update
    void Start()
    {
        currentState = State.MovingtoPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.MovingtoPlayer:
                //Define a range enemy can sense player, get position of player within range every frame, move towards position at set speed, stop when position reached, switch to Attacking
                //If player is *senseradius* units away from enemy transform, they're outside radius
                //If player.x > (transform.x - *senseradius*) they're inside radius to the left
                //If player.x > (transform.x + *senseradius*) they're inside radius to the right || Bigger than transform.x, smaller than transform.x + senselength
                //If player.x < (transform.x - *senseradius*) they're outside radius to left
                //If player.x < (transform.x + *senseradius*) they're outside radius to the right
                break;
            case State.Attacking:

                break;
            case State.Dead:

                break;
        }
    }

    private void FixedUpdate()
    {
        playerPos = GameObject.FindWithTag("Player").transform.position;
        myPos = transform.position;
        PlayerInRadius();

    }

    public void PlayerInRadius()
    {
        insideRadiusDir = Mathf.Sign(-transform.position.x + playerPos.x);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, senseradius);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, playerPos);
    }
}
