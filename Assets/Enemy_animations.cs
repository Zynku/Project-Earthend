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
        Dead,
        Stunned
    }


    // Update is called once per frame
    void Update()
    {

        //I can't figure out how to get a reference to the enum from here lol
        if (GetComponent<Enemy_controller>().currentState == Enemy_controller.State.Idle)
        {
            AnimationStates = animstate.Idle;
        }

        if (GetComponent<Enemy_controller>().currentState == Enemy_controller.State.MovingToPlayer)
        {
            AnimationStates = animstate.Running;
        }

        if (GetComponent<Enemy_controller>().currentState == Enemy_controller.State.Attacking)
        {
            AnimationStates = animstate.Attacking;
        }

        if (GetComponent<Enemy_controller>().currentState == Enemy_controller.State.Dead)
        {
            AnimationStates = animstate.Dead;
        }

        if (GetComponent<Enemy_controller>().currentState == Enemy_controller.State.Stunned)
        {
            AnimationStates = animstate.Stunned;
        }

        if (GetComponent<Enemy_controller>().currentState == Enemy_controller.State.Dead)
        {
            AnimationStates = animstate.Dead;
        }





        if (GetComponent<Enemy_controller>().attack == true && AnimationStates == animstate.Attacking)
        {
            animator.SetBool("Attack", true);
        }
        else
        {
            animator.SetBool("Attack", false);
        }

        if (GetComponent<Enemy_controller>().canFollowPlayer == true && rb2d.velocity.x > 0|| rb2d.velocity.x < 0 && AnimationStates != animstate.Attacking)
        {
            animator.SetBool("Running", true);
        }
        else
        {
            animator.SetBool("Running", false);
        }

        if (AnimationStates == animstate.Stunned)
        {
            animator.SetBool("Stunned", true);
        }
        else
        {
            animator.SetBool("Stunned", false);
        }

        if (GetComponent<Enemy_controller>().isGrounded == true)
        {
            animator.SetBool("Grounded", true);
        }
        else
        {
            animator.SetBool("Grounded", false);
        }

        if (AnimationStates == animstate.Dead)
        {
            animator.Play("Dead");
        }
    }
}
