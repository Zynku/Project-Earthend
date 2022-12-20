using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devlab_Holo : MonoBehaviour
{
    Enemymain enemymain;
    Animator animator;

    ParticleSystem hitParticles;
    void Start()
    {
        enemymain = GetComponent<Enemymain>();
        animator = GetComponent<Animator>();
        hitParticles = GetComponent<ParticleSystem>();
        enemymain.enemyBeenHit += HitReaction;
    }

    void Update()
    {
        
    }

    public void HitReaction()
    {
        float randomVal = Random.Range(0, 2);

        switch (randomVal)
        {
            case 0:
                animator.SetTrigger("Hit Forward");
                hitParticles.Play();
                break;
            case 1:
                animator.SetTrigger("Hit Backwards");
                hitParticles.Play();
                break;
            default:
                break;
        }
    }
}
