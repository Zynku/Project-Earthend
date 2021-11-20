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
        if (GetComponent<enemy_controller>().currentState == enemy_controller.State.Idle)
        {
            AnimationStates = animstate.Idle;
        }

        if (GetComponent<enemy_controller>().currentState == enemy_controller.State.MovingToPlayer)
        {
            AnimationStates = animstate.Running;
        }

        if (GetComponent<enemy_controller>().currentState == enemy_controller.State.Attacking)
        {
            AnimationStates = animstate.Attacking;
        }

        if (GetComponent<enemy_controller>().currentState == enemy_controller.State.Dead)
        {
            AnimationStates = animstate.Dead;
        }

        if (GetComponent<enemy_controller>().currentState == enemy_controller.State.Stunned)
        {
            AnimationStates = animstate.Stunned;
        }

        if (GetComponent<enemy_controller>().currentState == enemy_controller.State.Dead)
        {
            AnimationStates = animstate.Dead;
        }





        if (GetComponent<enemy_controller>().attack == true && AnimationStates == animstate.Attacking)
        {
            animator.SetBool("Attack", true);
        }
        else
        {
            animator.SetBool("Attack", false);
        }

        if (GetComponent<enemy_controller>().canFollowPlayer == true && rb2d.velocity.x > 0|| rb2d.velocity.x < 0 && AnimationStates != animstate.Attacking)
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

        if (GetComponent<enemy_controller>().isGrounded == true)
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
