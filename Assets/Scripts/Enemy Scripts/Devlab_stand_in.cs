using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devlab_stand_in : MonoBehaviour
{
    Animator animator;
    Enemymain enemymainscript;
    Collider2D hitbox;

    public bool appeared, disappeared;  //Has the stand appeared or disappeared yet?


    public enum Standstate
    {
        Appearing,
        Idle,
        Disappearing
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        enemymainscript = GetComponent<Enemymain>();
        hitbox.enabled = false;
        Enemymain.EnemyBeenHit += BeenHit;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemymainscript.playerInsideRadius) 
        {
            ActivateStand(); 
        }
        else if (appeared && enemymainscript.playerInsideRadius)
        {
            DeactivateStand();
        }
    }

    public void BeenHit()
    {

    }

    public void ActivateStand()
    {
        if (!appeared)
        { 
            animator.SetTrigger("Appear");
            appeared = true;
            disappeared = false;
        }
    }

    public void ActivateHitBox()
    {
        hitbox.enabled = true;
    }


    public void DeactivateStand()
    {
        if (!disappeared)
        {
            animator.SetTrigger("Disappear");
            disappeared = true;
            appeared=false;
        }
    }

    public void DeactivateHitbox()
    {
        hitbox.enabled = false;
    }
}
