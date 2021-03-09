using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Find_char : MonoBehaviour
{
    [Header("Movement Variables")]
    private Rigidbody2D rb2d;
    public float senselength;
    public bool PlayerRight;
    public bool PlayerLeft;
    public float yoffset;
    public float enemyspeed = 1;
    public float maxMoveSpeed = 3f;


    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        MovetoPlayer();
        DetectingRight();
        DetectingLeft();
    }

    public void DetectingRight()
    {
        if (Physics2D.Raycast(transform.position + new Vector3(0, yoffset), new Vector2(1, 0), senselength, 1 << LayerMask.NameToLayer("Player")))
        {
            PlayerRight = true;
            MovetoPlayer();
        }
        else
        {
            PlayerRight = false;
        }
    }

    public void DetectingLeft()
    {
        if (Physics2D.Raycast(transform.position + new Vector3(0, yoffset), new Vector2(-1, 0), senselength, 1 << LayerMask.NameToLayer("Player")))
        {
            PlayerLeft = true;
            MovetoPlayer();
        }
        else
        {
            PlayerRight = false;
        }
    }

    private void MovetoPlayer()
    {
        if ( PlayerRight == true )
        {
            rb2d.AddForce(new Vector2(1, 0f));

            if (Mathf.Abs(rb2d.velocity.x) > maxMoveSpeed)
                rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxMoveSpeed, rb2d.velocity.y);
        }

        if (PlayerLeft == true)
        {
            rb2d.AddForce(new Vector2(-1, 0f));

            if (Mathf.Abs(rb2d.velocity.x) > maxMoveSpeed)
                rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxMoveSpeed, rb2d.velocity.y);
        }
    }

    private void OnDrawGizmos()
    {
        if (PlayerRight == true)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position + new Vector3(0, yoffset), new Vector2(1, 0) * senselength);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + new Vector3(0, yoffset), new Vector2(1, 0) * senselength);
        }
        
        if (PlayerLeft == true)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position + new Vector3(0, yoffset), new Vector2(-1, 0) * senselength);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + new Vector3(0, yoffset), new Vector2(-1, 0) * senselength);
        }
        
           /* // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + new Vector3(0, yoffset), 1);*/
    }
}
