using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_animations : MonoBehaviour
{
    Rigidbody2D rb2d;
    Animator animator;

    public animstate AnimationStates;
    
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }


    public enum animstate
    {
        Idle,
        Running,
        Attacking,
        Dead
    }
    // Update is called once per frame
    void Update()
    {


        if (GetComponent<enemy_controller>().attack == true)
        {
            animator.SetBool("Attack", true);
        }
        else
        {
            animator.SetBool("Attack", false);
        }

        if (GetComponent<enemy_controller>().canFollowPlayer == true && rb2d.velocity.x > 0 || rb2d.velocity.x < 0)
        {
            animator.SetBool("Running", true);
        }
        else
        {
            animator.SetBool("Running", false);
        }

        if (GetComponent<enemy_controller>().isGrounded == true)
        {
            animator.SetBool("Grounded", true);
        }
        else
        {
            animator.SetBool("Grounded", false);
        }


        switch (AnimationStates)
        {
            case animstate.Idle:
                break;

            case animstate.Attacking:
                break;

            case animstate.Running:
                break;

            case animstate.Dead:
                break;
        }

    }
}
