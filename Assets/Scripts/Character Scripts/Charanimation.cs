using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Charanimation : MonoBehaviour
{
    [Foldout("Variables", true)]
    public string currentState;
    public float xvel;
    public float yvel;
    public bool isGrounded;
    private bool jumped;
    private bool ledgeGrabbed;
    private bool ledgePulledUp;

    [Foldout("General Animation Variables", true)]
    public AnimatorClipInfo[] animClipInfo;
    public string currentAnimName;
    public float currentAnimLength;
    public float currentStateNormalizedTime;

    [Foldout("Combo Animation Variables", true)]
    public List<Combo> currentCombo;       //Its a list because I literally cannot get it to work when I set it as just a combo field
    public List<Combo> comboBuffer;        //Holds combos that are to be played if a combo is currently being played.
    public int comboBufferSize;            //How many combos can be stored and buffered.
    public bool currentlyComboing = false;
    public bool inCombat = false;
    public string currentComboAnimName;
    public float currentComboAnimLength;

    Animator animator;
    Rigidbody2D rb2d;
    Charcontrol charcontrol;
    Charanimation charanimation;
    Charaudio charaudio;
    Charattacks charattacks;
    Chareffects chareffects;

    public delegate void ComboBufferClearEvent();
    public event ComboBufferClearEvent comboBufferCleared;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        charcontrol = GetComponent<Charcontrol>();
        charanimation = GetComponent<Charanimation>();
        charaudio = GetComponent<Charaudio>();
        charattacks = GetComponent<Charattacks>();
        chareffects = GetComponent<Chareffects>();
    }


    public void FixedUpdate()
    {
        AnimatorStateInfo currentAnimSInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] currentAnimCInfo = animator.GetCurrentAnimatorClipInfo(0);
        switch (charcontrol.currentState)
        {
            case Charcontrol.State.COMBAT_Idle:
                animator.SetBool("In combat", true);
                animator.SetBool("Running", false);
                animator.SetBool("Falling", false);
                currentlyComboing = false;
                break;

            case Charcontrol.State.COMBAT_Running:
                animator.SetBool("In combat", true);
                animator.SetBool("Walking", false);
                animator.SetBool("Running", true);
                animator.SetBool("Falling", false);
                break;

            case Charcontrol.State.COMBAT_Attacking:
                animator.SetBool("In combat", true);
                animator.SetBool("Falling", false);
                break;

            case Charcontrol.State.COMBAT_Dodging:
                animator.Play("L_P_C Combat Roll into Idle");
                //Last frame of this animation calls onDodgeTransition from Charcontrol to switch back to Idle.
                animator.SetBool("Running", false);
                animator.SetBool("Falling", false);
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
                SetStuffToFalseOnIdle();
                break;

            case Charcontrol.State.Walking:
                animator.SetBool("Running", false);
                animator.SetBool("Walking", true);
                animator.SetBool("Falling", false);
                break;

            case Charcontrol.State.Running:
                animator.SetBool("Running", true);
                animator.SetBool("Walking", false);
                animator.SetBool("Falling", false);
                break;

            case Charcontrol.State.Jumping:
                if (jumped == false)
                {
                    animator.Play("Jump Transition");   //This is an empty state that immediately transitions to the correct state
                    animator.SetBool("Jumping", true);
                    animator.SetBool("Falling", false);
                    animator.SetBool("Running", false);
                    jumped = true;
                }
                break;

            case Charcontrol.State.AirJumping:
                //Check onAnimate for when double jump is played
                break;

            case Charcontrol.State.Falling:
                if (animator.GetBool("Running")) 
                {
                    animator.SetBool("Running", false);
                    animator.Play("Jump Downwards Loop"); 
                }
                
                animator.SetBool("Falling", true);
                break;

            case Charcontrol.State.Landing:
                break;

            case Charcontrol.State.Switching_to_Crouching:
                if (currentAnimCInfo[0].clip.name != "Idle into Crouch")
                {
                    animator.Play("Idle into Crouch");
                }
                
                animator.SetBool("Crouching", true);
                animator.SetBool("Crouchwalking", false);

                if (currentAnimSInfo.normalizedTime >= 0.9f)
                {
                    charcontrol.currentState = Charcontrol.State.Crouching_Idle;
                }
                break;

            case Charcontrol.State.Switching_from_Crouching:
                animator.SetBool("Crouching", false);
                animator.SetBool("Crouchwalking", false);

                if (currentAnimSInfo.normalizedTime >= 0.9f)
                {
                    charcontrol.currentState = Charcontrol.State.Idle;
                }
                break;

            case Charcontrol.State.Crouching_Idle:
                animator.SetBool("Crouchwalking", false);
                break;

            case Charcontrol.State.CrouchWalking:
                animator.SetBool("Crouchwalking", true);
                break;

            case Charcontrol.State.Attacking:
                break;

            case Charcontrol.State.Air_Attacking:
                break;

            case Charcontrol.State.Dodging:
                    animator.Play("L_P Combat Roll into Idle"); //Last frame of this animation calls onDodgeTransition from Charcontrol to switch back to Idle.
                    animator.SetBool("Running", false);
                break;

            case Charcontrol.State.Ledgegrabbing:
                if (!ledgeGrabbed)
                {
                    ledgeGrabbed = true;
                    animator.Play("Ledge Grab from Jump");
                }
                animator.SetBool("Jumping", false);
                animator.SetBool("Falling", false);
                break;

            case Charcontrol.State.Ledgepullup:
                if (!ledgePulledUp)
                {
                    animator.Play("Ledge Grab into Pull Up");
                    ledgePulledUp = true;
                }
                animator.SetBool("Jumping", false);
                animator.SetBool("Falling", false);
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

    string currentAnimNameNextFrame;
    public void Update()
    {
        if (currentAnimName != currentAnimNameNextFrame) //Different anim is playing
        {
            currentAnimNameNextFrame = currentAnimName;
            //Debug.LogWarning($"Currently playing {currentAnimName}");
        }    
        
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
                charcontrol.currentState = Charcontrol.State.COMBAT_Attacking;
                if (combo.FXAnimationName != null) { chareffects.PlayMeleeSwingFX(combo.FXAnimationName); }
                currentCombo.Clear();
                currentCombo.Add(combo);
                currentlyComboing = true;
            }
            else if (comboBuffer.Count < comboBufferSize) //Combos are playing, add the combo to the buffer
            {
                Debug.Log($"...and {currentCombo[0].comboName} is playing, assigning {combo.comboName} to the combo buffer to await JUDGEMENT");
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
        if (comboBuffer.Count > 0)
        {
            AnimatorStateInfo currentAnimSInfo = animator.GetCurrentAnimatorStateInfo(0);
            Combo nextCombo = comboBuffer[0];
            if (currentCombo.Count > 0 && currentAnimSInfo.normalizedTime >= currentCombo[0].comboChainTimeLocation)   //If the current animation passes the time at which a combo can be chained, it plays the first animation stored in the buffer
            {
                string nextComboAnimName = nextCombo.animationName;
                animator.Play(nextComboAnimName);
                charcontrol.currentState = Charcontrol.State.COMBAT_Attacking;
                if (nextCombo.FXAnimationName != "") { chareffects.PlayMeleeSwingFX(nextCombo.FXAnimationName); }
                currentCombo.Clear();
                currentCombo.Add(nextCombo);
                comboBuffer.Remove(nextCombo);
                Debug.Log($"Removing {nextCombo.comboName} from combo buffer by playing it");
                currentlyComboing = true;
                charcontrol.currentState = Charcontrol.State.COMBAT_Attacking;

                if (nextCombo.endOfComboChain)
                {
                    ClearComboBuffer();
                    ClearAttackList();
                }
            }
            else if (currentCombo.Count == 0)
            {
                ClearComboBuffer();
                ClearAttackList();
                Debug.Log($"Current combo is empty, clearing attack list and combo list");
            }
        }
    }

    public void ClearComboBuffer()
    {
        comboBuffer.Clear();
        comboBufferCleared?.Invoke();
    }

    public void ClearAttackList()
    {
        charattacks.ClearAttackList();
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
        if (currentlyComboing)
        {
            currentComboAnimName = animClipInfo[0].clip.name;
            currentComboAnimLength = animClipInfo[0].clip.length;
        }
        else
        {
            currentComboAnimLength = 0f;
            currentComboAnimName = null;
        }
    }

    public void SetStuffToFalseOnIdle() //Is called from IdleBehaviour script every time player enters Idle animation
    {
        animator.SetBool("Running", false);
        animator.SetBool("Jumping", false);
        animator.SetBool("Walking", false);
        animator.SetBool("In combat", false);
        animator.SetBool("Falling", false);
    }

    public void onAnimate()
    {
        if (isGrounded)
        {
            animator.SetBool("Grounded", true);
            animator.SetBool("Jumping", false);
            jumped = false;
            ledgeGrabbed = false;
            ledgePulledUp = false;
        }
        if (!isGrounded)
        {
            animator.SetBool("Grounded", false);
        }

        if (charcontrol.airJumped && charcontrol.currentState == Charcontrol.State.Jumping)
        {
            animator.Play("Jump Double into Downwards");
            animator.SetBool("Jumping", true);
        }
        else
        {
            animator.SetBool("Jumping", false);
        }

        animClipInfo = this.animator.GetCurrentAnimatorClipInfo(0);
        try {currentAnimName = animClipInfo[0].clip.name; } catch (System.IndexOutOfRangeException) { };
        try {currentAnimLength = animClipInfo[0].clip.length; } catch (System.IndexOutOfRangeException) { };
        AnimatorStateInfo currentAnimSInfo = animator.GetCurrentAnimatorStateInfo(0);
        currentStateNormalizedTime = currentAnimSInfo.normalizedTime;


        //animator.SetFloat("yVel", Mathf.Clamp(rb2d.velocity.y, -1, 1));
        animator.SetFloat("yVel", rb2d.velocity.y);
        //animator.SetFloat("xVel", Mathf.Clamp(rb2d.velocity.x, -1, 1));
        animator.SetFloat("xVel", rb2d.velocity.x);
        animator.SetFloat("yVelAbs", Mathf.Abs(Mathf.Clamp(rb2d.velocity.y, -1, 1)));
        animator.SetFloat("xVelAbs", Mathf.Abs(Mathf.Clamp(rb2d.velocity.x, -1, 1)));
        animator.SetFloat("verticalPressed", Input.GetAxis("Vertical"));
        animator.SetFloat("horizontalPressed", Mathf.Abs(Input.GetAxis("Horizontal")));
    }
}




        

