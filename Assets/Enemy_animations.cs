using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_animations : MonoBehaviour
{
    Rigidbody2D rb2d;
    Animator animator;
    enemy_controller enemy_control;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemy_control = GetComponent<enemy_controller>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (enemy_control.currentState)
        {
            case enemy_controller.State.Spawning:
                break;
            case enemy_controller.State.Idle:
                break;
            case enemy_controller.State.Attacking:
                if (enemy_control.attack) { animator.SetBool("Attack", true); }
                else { animator.SetBool("Attack", false); }
                break;
            case enemy_controller.State.MovingToPlayer:
                break;
            case enemy_controller.State.MovingAwayFromPlayer:
                break;
            case enemy_controller.State.Stunned:
                animator.SetBool("Stunned", true);
                break;
            case enemy_controller.State.Dodging:
                break;
            case enemy_controller.State.Waiting:
                break;
            case enemy_controller.State.Dead:
                animator.Play("Dead");
                break;
            default:
                break;
        }

        if (enemy_control.canFollowPlayer == true && Mathf.Abs(rb2d.velocity.x) > 0) { animator.SetBool("Running", true); }
        else { animator.SetBool("Running", false); }

        if (enemy_control.isGrounded == true)
        {
            animator.SetBool("Grounded", true);
        }
        else
        {
            animator.SetBool("Grounded", false);
        }
    }
}
