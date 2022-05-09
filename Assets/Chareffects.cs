using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MyBox;

public class Chareffects : MonoBehaviour
{
    Charcontrol charcontrol;

    [Foldout("Initialization Fields", true)]
    [Header("Assign Please!")]
    public ParticleSystem dustParticles;
    public GameObject meleeFX;
    Animator meleeFXAnim;

    // Start is called before the first frame update
    void Start()
    {
        charcontrol = GetComponent<Charcontrol>();
        meleeFXAnim = meleeFX.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        DoMovementParticles();
    }

    bool particlesTriggered;
    public void DoMovementParticles()
    {
        if (charcontrol.currentState == Charcontrol.State.Running || charcontrol.currentState == Charcontrol.State.COMBAT_Running)
        {
            if (!particlesTriggered)
            {
                Debug.Log("Playing particle sys");
                dustParticles.Play();
                particlesTriggered = true;
            }
        }
        else
        {
            dustParticles.Stop();
            particlesTriggered= false;
        }

    }

    public void PlayMeleeSwingFX(string animName)
    {
        meleeFXAnim.Play(animName);
    }
}
