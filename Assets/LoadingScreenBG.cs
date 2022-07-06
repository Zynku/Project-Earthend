using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenBG : MonoBehaviour
{
    public Pause_menu_manager manager;
    Animator animator;

    private void Start()
    {
        manager = GetComponentInParent<Pause_menu_manager>();
        animator = GetComponent<Animator>();
    }

    public void StartFading()
    {
        Start();
        animator.SetTrigger("Start Fade");
    }

    public void FinishedFading()
    {
        manager.FadeOutFinished();
    }
}
