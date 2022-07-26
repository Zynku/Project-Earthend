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

    public float IFrameBlinkDelayStart; //The time between each blink at the start of the effect. Will slowly lerp to end delay
    public float IFrameBlinkDelayEnd;
    public float IFrameBlinkDelayCurr;  //What is the value currently
    public float IFrameBlinkTest;

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
            StartCoroutine(DoIframeBlink());
        }
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
        //StopCoroutine(DoIframeBlink());
        //StartCoroutine(DoIframeBlink());
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
        while (damageCoolDownTimeRef >= 0)
        {
            //spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);    //Set player sprite to transparent
            Debug.Log("Disabling renderer");
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(IFrameBlinkTest);
            //spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);    //Set player sprite to opaque
            Debug.Log("Enabling renderer");
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(IFrameBlinkTest);
        }
    }

    public void PlayMeleeSwingFX(string animName)
    {
        meleeFXAnim.Play(animName);
    }
}
