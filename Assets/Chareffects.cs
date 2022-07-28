using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MyBox;

public class Chareffects : MonoBehaviour
{
    Charcontrol charcontrol;
    Charattacks charattacks;
    Charhealth charhealth;
    SpriteRenderer spriteRenderer;

    [Foldout("Initialization Fields", true)]
    [Header("Assign Please!")]
    public ParticleSystem runParticles;
    public ParticleSystem jumpParticles;
    public GameObject meleeFX;
    Animator meleeFXAnim;

    public float IFrameBlinkDelayStart; //The time between each blink at the start of the effect. Will lerp to end delay
    public float IFrameBlinkDelayEnd;
    [ReadOnly] public float IFrameBlinkDelayCurr;  //What is the value currently
    public bool IFrameBlinking;         //Are we blinking?
    private float damageCoolDownTimeRef;
    private float damageCoolDownTargetTimeRef;

    // Start is called before the first frame update
    void Start()
    {
        charcontrol = GetComponent<Charcontrol>();
        meleeFXAnim = meleeFX.GetComponent<Animator>();
        charattacks = GetComponent<Charattacks>();
        charhealth = GetComponent<Charhealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Charhealth.Hit += IframeBlink;
    }

    // Update is called once per frame
    void Update()
    {
        DoMovementParticles();
        DoJumpParticles();
        if (!charhealth.onDamageCoolDown)
        {
            IFrameBlinkDelayCurr = IFrameBlinkDelayStart;
        }
        else
        {
            damageCoolDownTargetTimeRef = charhealth.dmgCooldownTargetTime;
            damageCoolDownTimeRef = charhealth.dmgCooldownTime;
            StartCoroutine(CalculateIFrameBlinkDelay());
        }

        if(damageCoolDownTimeRef <= 0 && IFrameBlinking)  //If we're off the damage cooldown
        {
            StopCoroutine(DoIframeBlink());
            IFrameBlinking = false;
            spriteRenderer.enabled = true;
        }

        //if (Input.GetKeyDown(KeyCode.Y)) { IFrameBlinkToggle ^= true; } //Apparently ^= is the XOR operator, so it inverts the value of the left hand side
        //if (IFrameBlinkToggle && !IFrameBlinking) { StartCoroutine(DoIframeBlink()); IFrameBlinking = true; }
    }

    bool runParticlesTriggered;
    public void DoMovementParticles()
    {
        if (charcontrol.currentState == Charcontrol.State.Running || charcontrol.currentState == Charcontrol.State.COMBAT_Running)
        {
            if (!runParticlesTriggered)
            {
                //Debug.Log("Playing particle sys");
                runParticles.Play();
                runParticlesTriggered = true;
            }
        }
        else
        {
            runParticles.Stop();
            runParticlesTriggered= false;
        }

    }
    bool jumpParticlesTriggered;
    public void DoJumpParticles()
    {
        if (charcontrol.currentState == Charcontrol.State.Jumping)
        {
            if (!jumpParticlesTriggered)
            {
                //Debug.Log("Playing particle sys");
                jumpParticles.Play();
                jumpParticlesTriggered = true;
            }
        }
        else
        {
            jumpParticles.Stop();
            jumpParticlesTriggered = false;
        }
    }

    public void DoScreenShakeManual()
    {
        StartCoroutine(GameManager.instance.DoScreenShake(charattacks.screenShakeIntensity, charattacks.screenShakeTime));
    }

    public void IframeBlink()
    {
        if (!IFrameBlinking)
        {
            IFrameBlinking = true;
            StartCoroutine(DoIframeBlink()); 
        }
    }

    public IEnumerator CalculateIFrameBlinkDelay()
    {
        while (damageCoolDownTimeRef >= 0)
        {
            float elapsedTime = damageCoolDownTimeRef / damageCoolDownTargetTimeRef;
            IFrameBlinkDelayCurr = Mathf.Lerp(IFrameBlinkDelayEnd, IFrameBlinkDelayStart, elapsedTime);
            yield return null;
        }
    }

    public IEnumerator DoIframeBlink()
    {
        yield return new WaitForSeconds(0.05f); //This ensures there's enough time for the damagecooldown value to update since I had issues with the next time checking too quickly
        if(damageCoolDownTimeRef >= 0)  //This coroutine will run once as long as it we're on damage cool down. It's called again futher down and will do it again if we're still true
        {
            IFrameBlinking = true;
            //Debug.Log("Disabling renderer");
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(IFrameBlinkDelayCurr);
            //Debug.Log("Enabling renderer");
            spriteRenderer.enabled = true;
            StartCoroutine(DoIframeBlink());    //Make sure this loops as long as it needs to
        }
    }

    public void PlayMeleeSwingFX(string animName)
    {
        meleeFXAnim.Play(animName);
    }
}
