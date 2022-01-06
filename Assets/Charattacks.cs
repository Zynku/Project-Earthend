using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Charattacks : MonoBehaviour
{
    public string currentState;
    Charcontrol charcontrol;
    Charanimation charanimation;
    Rigidbody2D rb2d;
    CharMeleeHitBox charMeleeHitBox;

    [Header("Combo Variables")]
    public List<Combo> allLightCombosEver;  //Set these in inspector
    public List<Combo> allHeavyCombosEver;
    public List<Combo> allRangedCombosEver;
    public List<Combo> currentPossibleCombos;
    public List<Attack> currentAttacks;
    public Combo currentCombo;
    public bool currentlyComboing;
    public float comboSustainTargetTime = 0.5f; //How long you have to input another attack before you can chain with previous attacks to make a combo. If it runs out, currentcombo is set to null
    public float comboSustainTime;
    public float comboExecuteTargetTime = 0.5f;  //How long before current combo returns to nothing after being executed
    public float comboExecuteTime;
    public int longestComboLength;
    private float buttonHeldFloat;
    public float buttonHeldThreshold;           //How much time needs to pass (measured from time.deltaTime) for the button to be considered as "held"
    public Combo longestCombo;
    public GameObject onScreenComboSustainTimer;
    public GameObject onScreenComboExecuteTimer;

    [Header("Damage Variables")]
    public int lightDamageMax, lightDamageMin;
    public int heavyDamageMax, heavyDamageMin;
    public int rangedDamageMax, rangedDamageMin;

    // Start is called before the first frame update
    void Start()
    {
        charcontrol = GetComponent<Charcontrol>();
        charanimation = GetComponent<Charanimation>();
        rb2d = GetComponent<Rigidbody2D>();
        charMeleeHitBox = GetComponentInChildren<CharMeleeHitBox>();
        comboSustainTime = comboSustainTargetTime;
        comboExecuteTime = comboExecuteTargetTime;
        onScreenComboSustainTimer = TimerManager.instance.CreateTimer(comboSustainTime, comboSustainTargetTime, "Combo Sustain Time", false);
        onScreenComboExecuteTimer = TimerManager.instance.CreateTimer(comboExecuteTime, comboExecuteTargetTime, "Combo Execute Time", false);
        FindLongestCombo();
    }

    // Update is called once per frame
    void Update()
    {
        currentState = charcontrol.currentState.ToString();
        switch (charcontrol.currentState)
        {
            case Charcontrol.State.COMBAT_Idle:
                break;
        }

        onScreenComboSustainTimer.GetComponent<SimpleTimerScript>().timerTime = comboSustainTime;
        onScreenComboExecuteTimer.GetComponent<SimpleTimerScript>().timerTime = comboExecuteTime;
        //onScreenComboExecuteTimer.GetComponent<SimpleTimerScript>().timerTargetTime = charanimation.currentAnimLength;

        if (Input.GetButtonUp("Light Attack"))
        {
            if (charcontrol.currentState == Charcontrol.State.COMBAT_Air_Attacking)
            {
                return;
            }
            else
            {
                if (buttonHeldFloat > buttonHeldThreshold) { return; }
                //Debug.Log("Pressing light attack");
                Attack newattack = new Attack();
                newattack.SetupAttack("Light", lightDamageMin, Attack.AttackType.LIGHT);
                currentAttacks.Add(newattack);
                comboSustainTime = comboSustainTargetTime;              //Resets the combo sustain timer

                CheckforCombos();
                buttonHeldFloat = 0;
            }
        }

        if (Input.GetButton("Light Attack"))
        {
            buttonHeldFloat += Time.deltaTime;
            if (buttonHeldFloat > buttonHeldThreshold)
            {
                buttonHeldFloat = 0;
                //Debug.Log($"Light attack is being held");
                Attack newattack = new Attack();
                newattack.SetupAttack("Light_Held", lightDamageMin, Attack.AttackType.LIGHT_HELD);
                currentAttacks.Add(newattack);
                comboSustainTime = comboSustainTargetTime;              //Resets the combo sustain timer

                CheckforCombos();
            }
            //Debug.Log($"{buttonHeldFloat}");
        }


        if (Input.GetButtonUp("Heavy Attack"))
        {
            Attack newattack = new Attack();
            newattack.SetupAttack("Heavy", heavyDamageMin, Attack.AttackType.HEAVY);
            currentAttacks.Add(newattack);
            comboSustainTime = comboSustainTargetTime;

            CheckforCombos();
        }

        if (Input.GetButtonDown("Ranged Attack"))
        {
            Attack newattack = new Attack();
            newattack.SetupAttack("Ranged", rangedDamageMin, Attack.AttackType.RANGED);
            currentAttacks.Add(newattack);
            comboSustainTime = comboSustainTargetTime;

            CheckforCombos();
        }

        if (currentAttacks.Count > 0)   //If we pressed an attack, start counting down the combosustain timer
        {
            comboSustainTime -= Time.deltaTime;
            if (comboSustainTime < 0)
            {
                comboSustainTime = comboSustainTargetTime;
                ClearAttackList();
            }
        }

        if (currentlyComboing) //If we executed a combo start counting down to end the combo
        {
            comboExecuteTime -= Time.deltaTime;
            if (comboExecuteTime < 0)
            {
                comboExecuteTime = comboExecuteTargetTime;
                //comboExecuteTime = charanimation.currentAnimLength;
                currentCombo = null;
                ClearAttackList();
                currentlyComboing = false;
            }
        }
    }

    public void Combat_Idle()
    {
        charcontrol.currentState = Charcontrol.State.COMBAT_Idle;
        charcontrol.airJumpsHas = charcontrol.airJumps;
        charcontrol.rolled = false;

        charcontrol.canFlipXDir();
    }

    public void Combat_Running()
    {
        charcontrol.rb2d.velocity = new Vector2(charcontrol.runSpeed * Input.GetAxis("Horizontal"), charcontrol.rb2d.velocity.y);

        charcontrol.rolled = false;
        charcontrol.canFlipXDir();
    }

    public void CheckforCombos()
    {
        charcontrol.canFlipXDir();
        if (currentAttacks.Count > longestComboLength + 1)
        {
            ClearAttackList();
        }

        foreach (Combo combo in currentPossibleCombos)
        {
            if (currentAttacks.Count == combo.attackList.Count)    //Has the same amount of attacks in its list
            {
                //Debug.Log(combo.comboName + " has the same amount of attacks as the current attack count, which is " + currentAttacks.Count);
                for (int i = 0; i < combo.attackList.Count; i++)    //Loop through all attacks in the combo attack list, checking if they match
                {
                    //Debug.Log("Checking " + combo.comboName + " to see if " + combo.attackList[i].attackType + " is the same as " + currentAttacks[i].attackType);
                    if (combo.attackList[i].attackType == currentAttacks[i].attackType)
                    {
                        if (i == combo.attackList.Count - 1 && combo.attackList[i].attackType == currentAttacks[i].attackType)  //If the last attack matches...
                        {
                            //Debug.Log(combo.comboName + " matches perfectly");

                            //comboExecuteTime = comboExecuteTargetTime;
                            comboExecuteTime = charanimation.currentAnimLength;
                            currentCombo = combo;
                            currentlyComboing = true;
                            AnimateCombos(combo);

                            if (combo.canChangeState) { StartCoroutine(charanimation.ComboCharcontrolStates(combo)); } //Checks the current combo to see if it requires a state change
                            if (combo.endOfComboChain) { ClearAttackList(); }
                            break;
                        }
                    }
                    else
                    {
                        //Get the current longest combo. If the current attacks length exceeds that, it means no combo will be possible and inputs will just be added for no reason. Clear the attack list
                        //Also, if more than 2 inputs are put in and no combos match, the player is down a path that wont result in combos ever, clear attack list.
                        //Or maybe just clear after it finds no combos that match since that means no combos will ever match down that path.
                        break;
                    }
                }
            }
        }
    }


    public void ClearAttackList()
    {
        currentAttacks.Clear();
    }

    public void AnimateCombos(Combo combo)
    {
        if (charanimation) { charanimation.AnimateCombos(combo); }
        else { Debug.LogWarning("Charattacks does not have a reference to Charanimation! No combo animations can be played!"); }

        charcontrol.currentState = Charcontrol.State.COMBAT_Attacking;
    }

    public void FindLongestCombo()
    {
        foreach (Combo combo in currentPossibleCombos)
        {
            int currentComboLength = combo.attackList.Count;
            if (currentComboLength > longestComboLength)
            {
                longestComboLength = currentComboLength;
                longestCombo = combo;
            }
        }

    }

    public void AddCombo()  //Moves a combo from all combos to possible combos
    {

    }

    public void RemoveCombo()   //Moves a combo from possible combos to all combos
    {

    }

    public void onAddAttackForceHorizontal(int force)  //This function called on the last frame of the dodge animation via AnimationEvent
    {
        rb2d.AddForce(new Vector2(rb2d.velocity.x + (force * charcontrol.facingDir), rb2d.velocity.y));
    }

    public void onAddAttackForceVertical(int force)  //This function called on the last frame of the dodge animation via AnimationEvent
    {
        rb2d.AddForce(new Vector2(rb2d.velocity.x, rb2d.velocity.y + (force)));
    }

    public void onChangeGravityScale(float gravity)
    {
        rb2d.gravityScale = gravity;
    }

    public void onResetGravity()
    {
        rb2d.gravityScale = 0.8f; //0.8 is just a temporary value. Please make a defaultGravityScale float and use that instead.
    }
}
