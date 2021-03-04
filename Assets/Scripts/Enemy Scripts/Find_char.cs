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


    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        if (DetectingLeft() == true)
        {
            PlayerLeft = true;
        }
        else
        {
            PlayerLeft = false;
        }

        if (DetectingRight() == true)
        {
            PlayerRight = true;
        }
        else
        {
            PlayerRight = false;
        }
    }

    private void FixedUpdate()
    {
        MovetoPlayer();
    }

    public bool DetectingRight()
    {
        RaycastHit2D hitr = Physics2D.Raycast(transform.position + new Vector3(0, yoffset), new Vector2(1, 0), senselength, 1 << LayerMask.NameToLayer("Player"));
        if (hitr.collider != null)
        {
           return (true);
        }
        else return (false);
    }

    public bool DetectingLeft()
    {
        RaycastHit2D hitl = Physics2D.Raycast(transform.position + new Vector3(0, yoffset), new Vector2(-1, 0), senselength, 1 << LayerMask.NameToLayer("Player"));
        if (hitl.collider != null)
        {
            return true;
        }
        else return false;
    }

    private void MovetoPlayer()
    {
        if ( PlayerRight == true )
        {
            rb2d.velocity = new Vector2(enemyspeed, rb2d.velocity.y);
        }


        if (PlayerLeft == true)
        {
            rb2d.velocity = new Vector2(-enemyspeed, rb2d.velocity.y);
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
