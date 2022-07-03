using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehaviour : StateMachineBehaviour
{

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject player = GameManager.instance.Player;
        Charcontrol charcontrol = player.GetComponent<Charcontrol>();
        Charanimation charanimation = player.gameObject.GetComponent<Charanimation>();
        Charattacks charattacks = player.gameObject.GetComponent<Charattacks>();

        charanimation.SetStuffToFalseOnIdle();
        charcontrol.currentState = Charcontrol.State.Idle;

        //Debug.Log($"We doin' Idle tings");
        charanimation.currentlyComboing = false;
        charattacks.ClearAttackList();
        if (charanimation.currentCombo.Count > 0) { charanimation.currentCombo.Clear(); }
        if (charcontrol.currentState == Charcontrol.State.COMBAT_Attacking)
        {
            charcontrol.currentState = Charcontrol.State.COMBAT_Idle;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
