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
    public Combo currentCombo;
    public string currentAnimName;
    public float currentAnimLength;
    public float currentStateNormalizedTime;

    [Header("Combo Animation Variables")]
    public List<Combo> comboBuffer;        //Holds combos that are to be played if a combo is currently being played.
    public int comboBufferSize = 1;        //How many combos can be stored and buffered.
    public bool currentlyComboing = false;
    public bool inCombat = false;
    public string currentComboAnimName;
    public float currentComboAnimLength;

    Animator animator;
    Rigidbody2D rb2d;
    Charcontrol charcontrol;
    Charanimation charanimation;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        charcontrol = GetComponent<Charcontrol>();
        charanimation = GetComponent<Charanimation>();
    }


    private void FixedUpdate()
    {
        switch (charcontrol.currentState)
        {
            case Charcontrol.State.COMBAT_Idle:
                animator.SetBool("In combat", true);
                animator.SetBool("Running", false);
                currentlyComboing = false;
                break;

            case Charcontrol.State.COMBAT_Running:
                animator.SetBool("In combat", true);
                animator.SetBool("Walking", false);
                animator.SetBool("Running", true);
                break;

            case Charcontrol.State.COMBAT_Attacking:
                animator.SetBool("In combat", true);
                break;

            case Charcontrol.State.COMBAT_Dodging:
                animator.Play("L_P_C Combat Roll into Idle");
                //Last frame of this animation calls onDodgeTransition from Charcontrol to switch back to Idle.
                //animator.SetTrigger("Dodging");
                //animator.SetTrigger("Rolling");
                animator.SetBool("Running", false);
                break;

            case Charcontrol.State.COMBAT_Jumping:
                if (jumped == false)
                {
                    animator.SetBool("In combat", true);
                    animator.Play("Combat Jump Transition");
                    animator.SetBool("Jumping", true);
                    jumped = true;
                }
                break;

            case Charcontrol.State.COMBAT_AirJumping:
                animator.SetBool("In combat", true);
                animator.Play("Combat Jump Transition");
                animator.SetBool("Jumping", true);
                break;

            case Charcontrol.State.COMBAT_Falling:
                
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
                    animator.Play("Jump Transition");   //This is an empty state that immediately transitions to the correct state
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
                    animator.Play("L_P Combat Roll into Idle"); //Last frame of this animation calls onDodgeTransition from Charcontrol to switch back to Idle.
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
        isGrounded = charcontrol.isGrounded;
        currentState = charcontrol.currentState.ToString();
        inCombat = charcontrol.inCombat;
    }

    private void Update()
    {
        //if (!currentlyComboing) { StartCoroutine(ManageComboBuffer()); }
        ManageComboBuffer();
    }

    public void AnimateCombos(Combo combo) //Is called from Charattacks. The combo name is passed in. Manages whether combo is played or added to combo buffer
    {
        if (comboBuffer.Count == 0) //If buffer is empty
        {
            Debug.Log("Combo buffer is empty...");
            if (!currentlyComboing) //If no combos are playing...
            {
                Debug.Log($"...and no combos are playing currently. Playing {combo.comboName}");
                animator.Play(combo.animationName);
                currentCombo = combo;
                currentlyComboing = true;
            }
            else if (comboBuffer.Count < comboBufferSize) //Combos are playing, add the combo to the buffer
            {
                Debug.Log($"...and {currentCombo.comboName} is playing, assigning {combo.comboName} to the combo buffer to await JUDGEMENT");
                comboBuffer.Add(combo);
            }
        }
        else
        {
            Debug.Log($"Combo buffer has {comboBuffer[0].comboName} queued to play already. Checking to see if {combo.comboName} can fit in the combo buffer");
            if (comboBuffer.Count < comboBufferSize)
            {
                Debug.Log($"There's space! {combo.comboName} can join the combo buffer :)");
                comboBuffer.Add(combo);
            }
            else if (comboBuffer.Count >= comboBufferSize)
            {
                Debug.Log($"Combo buffer is FULL, discarding {combo.comboName} :(");
            }
        }
    }

    public void ManageComboBuffer()  //Plays animations from the combo buffer if no other animations are playing
    {
        //Debug.Log("Managing Combo Buffer...");
        if (comboBuffer.Count > 0 && currentCombo != null)
        {
            AnimatorStateInfo currentAnimSInfo = animator.GetCurrentAnimatorStateInfo(0);
            Combo nextCombo = comboBuffer[0];
            if (currentAnimSInfo.normalizedTime >= currentCombo.comboChainTimeLocation)   //If there current animation is ~60% finished, it plays the first animation stored in the buffer
            {
                string nextComboAnimName = nextCombo.animationName;
                animator.Play(nextComboAnimName);
                comboBuffer.Remove(nextCombo);
                Debug.Log($"Removing {nextCombo.comboName} from combo buffer by playing it");
                currentlyComboing = true;

                if (nextCombo.endOfComboChain)
                {
                    ClearComboBuffer();
                }
                //UpdateComboStates();
            }
        }
    }

    public void ClearComboBuffer()
    {
        comboBuffer.Clear();
    }

    private Charcontrol.State stateWhenStartedLookingForComboStateChanges;
    private bool stateSet;

    public IEnumerator ComboCharcontrolStates(Combo combo)  //Forces Charcontrol to stay in a certain state until the stateChangeTime has been reached, according to the combo
    {
        //Debug.Log($"{stateWhenStartedLookingForComboStateChanges} is the state {this} is trying to keep Charcontrol at");
        AnimatorStateInfo currentAnimSInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (currentAnimSInfo.normalizedTime <= combo.stateChangeTimeLocation)
        {
            if (!stateSet)
            {
                stateWhenStartedLookingForComboStateChanges = charcontrol.currentState;
            }
            charcontrol.currentState = stateWhenStartedLookingForComboStateChanges;
            yield return new WaitForSecondsRealtime(0.1f);
            StartCoroutine(ComboCharcontrolStates(combo));
        }
        else if (currentAnimSInfo.normalizedTime >= combo.stateChangeTimeLocation)   //Detects when the current animation has passed the time required to allow a state change
        {
            charcontrol.currentState = combo.stateChange;
            stateSet = false;
        }
    }

    public void UpdateComboStates()
    {
        AnimatorStateInfo currentAnimSInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (currentlyComboing)
        {
            currentComboAnimName = animClipInfo[0].clip.name;
            currentComboAnimLength = animClipInfo[0].clip.length;

            if (currentAnimSInfo.normalizedTime >= 0.85f)   //Detects when the current animation is ended (Technically 85% ended). Ideally this would be 100% ended, but the timer never actually reaches 1
            {
                currentlyComboing = false;
                currentCombo = null;
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
        //currentStateNormalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        AnimatorStateInfo currentAnimSInfo = animator.GetCurrentAnimatorStateInfo(0);
        currentStateNormalizedTime = currentAnimSInfo.normalizedTime;


        animator.SetFloat("yVel", Mathf.Clamp(rb2d.velocity.y, -1, 1));
        animator.SetFloat("xVel", Mathf.Clamp(rb2d.velocity.x, -1, 1));
        animator.SetFloat("yVelAbs", Mathf.Abs(Mathf.Clamp(rb2d.velocity.y, -1, 1)));
        animator.SetFloat("xVelAbs", Mathf.Abs(Mathf.Clamp(rb2d.velocity.x, -1, 1)));
        animator.SetFloat("verticalPressed", Input.GetAxis("Vertical"));
        animator.SetFloat("horizontalPressed", Mathf.Abs(Input.GetAxis("Horizontal")));
    }
}




        

