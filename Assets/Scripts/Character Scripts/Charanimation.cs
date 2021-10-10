using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charanimation : MonoBehaviour
{
    public string currentState;
    public float xvel;
    public float yvel;
    public bool isGrounded;
    private bool jumped;
    [Header("General Animation Variables")]
    public AnimatorClipInfo[] animClipInfo;
    public string currentAnimName;
    public float currentAnimLength;
    public float currentStateNormalizedTime;

    [Header("Combo Animation Variables")]
    public List<string> comboBuffer;        //Holds combos that are to be played if a combo is currently being played.
    public bool currentlyComboing = false;
    public bool inCombat = false;
    public string currentComboAnimName;
    public float currentComboAnimLength;

    Animator animator;
    Rigidbody2D rb2d;
    Charcontrol charcontrol;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        charcontrol = GetComponent<Charcontrol>();
        
    }


    private void FixedUpdate()
    {
        switch (charcontrol.currentState)
        {
            case Charcontrol.State.COMBAT_Idle:
                animator.SetBool("In combat", true);
                animator.SetBool("Running", false);
                break;

            case Charcontrol.State.COMBAT_Running:
                animator.SetBool("In combat", true);
                animator.SetBool("Walking", false);
                animator.SetBool("Running", true);
                break;

            case Charcontrol.State.COMBAT_Attacking:
                break;

            case Charcontrol.State.Idle:
                animator.SetBool("Running", false);
                animator.SetBool("Jumping", false);
                animator.SetBool("Walking", false);
                animator.SetBool("In combat", false);
                break;

            case Charcontrol.State.Walking:
                animator.SetBool("Running", false);
                animator.SetBool("Walking", true);
                break;

            case Charcontrol.State.Running:
                animator.SetBool("Running", true);
                animator.SetBool("Walking", false);
                break;

            case Charcontrol.State.Jumping:
                if (jumped == false)
                {
                    animator.Play("Jump Transition");
                    animator.SetBool("Jumping", true);
                    jumped = true;
                }
                break;

            case Charcontrol.State.AirJumping:
                animator.Play("Jump Transition");
                animator.SetBool("Jumping", true);
                break;

            case Charcontrol.State.Falling:
                break;

            case Charcontrol.State.Landing:
                break;

            case Charcontrol.State.CrouchWalking:
                break;

            case Charcontrol.State.Attacking:
                break;

            case Charcontrol.State.Air_Attacking:
                break;

            case Charcontrol.State.Dodging:
                    animator.Play("L_P Combat Roll into Idle");
                    //animator.SetTrigger("Dodging");
                    //animator.SetTrigger("Rolling");
                    animator.SetBool("Running", false);
                break;

            case Charcontrol.State.Ledgegrabbing:
                break;

            case Charcontrol.State.Stunned:
                break;

            case Charcontrol.State.Dead:
                break;

        }
        onAnimate();
        UpdateComboStates();
        ManageComboBuffer();
        isGrounded = charcontrol.isGrounded;
        currentState = charcontrol.currentState.ToString();
        inCombat = charcontrol.inCombat;
    }

    public void AnimateCombos(string comboname)
    {
        if (currentlyComboing && comboBuffer.Count < 1)
        {
            comboBuffer.Add(comboname);
        }
        else if (!currentlyComboing && comboBuffer.Count == 0)
        {
            animator.Play(comboname);
            currentlyComboing = true;
        }
    }

    public void ManageComboBuffer()
    {
        AnimatorStateInfo currentAnimSInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (currentAnimSInfo.normalizedTime >= 0.6f && comboBuffer.Count > 0)
        {
            string nextCombo = comboBuffer[0];
            comboBuffer.Remove(nextCombo);
            animator.Play(nextCombo);
            currentlyComboing = true;
            UpdateComboStates();
        }
    }

    public void UpdateComboStates()
    {
        AnimatorStateInfo currentAnimSInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (currentlyComboing)
        {
            currentComboAnimName = animClipInfo[0].clip.name;
            currentComboAnimLength = animClipInfo[0].clip.length;

            if (currentAnimSInfo.normalizedTime >= 0.9f)   //Detects when the current animation is ended (Technically 95% ended). Ideally this would be 100% ended, but the timer never actually reaches 1
            {
                currentlyComboing = false;
            }
        }
        else
        {
            currentComboAnimLength = 0f;
            currentComboAnimName = null;
        }
    }

    public void onAnimate()
    {
        if (isGrounded)
        {
            animator.SetBool("Grounded", true);
            animator.SetBool("Jumping", false);
            jumped = false;
        }
        if (!isGrounded)
        {
            animator.SetBool("Grounded", false);
        }

        animClipInfo = this.animator.GetCurrentAnimatorClipInfo(0);
        currentAnimName = animClipInfo[0].clip.name;
        currentAnimLength = animClipInfo[0].clip.length;
        currentStateNormalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;


        animator.SetFloat("yVel", Mathf.Clamp(rb2d.velocity.y, -1, 1));
        animator.SetFloat("xVel", Mathf.Clamp(rb2d.velocity.x, -1, 1));
        animator.SetFloat("yVelAbs", Mathf.Abs(Mathf.Clamp(rb2d.velocity.y, -1, 1)));
        animator.SetFloat("xVelAbs", Mathf.Abs(Mathf.Clamp(rb2d.velocity.x, -1, 1)));
        animator.SetFloat("verticalPressed", Input.GetAxis("Vertical"));
        animator.SetFloat("horizontalPressed", Mathf.Abs(Input.GetAxis("Horizontal")));
    }
}




        

