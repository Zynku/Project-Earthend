using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyBox;

public class Charattacks : MonoBehaviour
{
    //[Foldout("Variables", true)]
    public string currentState;
    Charcontrol charcontrol;
    Charanimation charanimation;
    Charaudio charaudio;
    Rigidbody2D rb2d;
    CharMeleeHitBox charMeleeHitBox;

    [Separator("Combo Lists")]
    public List<ComboFamily> comboFamilies;
    public List<Combo> allLightCombosEver;  //Set these in inspector
    public List<Combo> allHeavyCombosEver;
    public List<Combo> allRangedCombosEver;
    public List<Combo> currentPossibleCombos;
    public List<Attack> currentAttacks;

    [Separator("Current Combo")]
    //public Combo currentCombo;
    public bool currentlyComboing;

    [Separator("Combo Management Timers")]
    //public float comboSustainTargetTime = 0.5f; 
    //[ReadOnly] public float comboSustainTime;   //How long you have to input another attack before you can chain with previous attacks to make a combo. If it runs out, currentcombo is set to null
    public float inputTargetTime;               
    [ReadOnly] public float inputTime;          //Inputs for attacks can only be received when this timer is 0 i.e. This dictates when inputs can be received.
    private GameObject onScreenComboSustainTimer;
    private GameObject onScreenComboExecuteTimer;
    private GameObject inputTimer;
    private float buttonHeldFloat;
    public float buttonHeldThreshold;           //How much time needs to pass (measured from time.deltaTime) for the button to be considered as "held"

    [Separator("Misc")]
    public int longestComboLength;
    public Combo longestCombo;

    [Separator("Damage Variables")]
    public int lightDamageMax, lightDamageMin;
    public int heavyDamageMax, heavyDamageMin;
    public int rangedDamageMax, rangedDamageMin;
    public int currentDamageMax, currentDamageMin;

    [Separator("Effects Variables")]
    public float screenShakeIntensity;
    public float screenShakeTime;


    public delegate void AttackClearEvent();
    public event AttackClearEvent attacksCleared;

    // Start is called before the first frame update
    void Start()
    {
        charcontrol = GetComponent<Charcontrol>();
        charanimation = GetComponent<Charanimation>();
        charaudio = GetComponent<Charaudio>();
        rb2d = GetComponent<Rigidbody2D>();
        charMeleeHitBox = GetComponentInChildren<CharMeleeHitBox>();
        inputTime = inputTargetTime;
        //onScreenComboSustainTimer = TimerManager.instance.CreateTimer(comboSustainTime, comboSustainTargetTime, "Combo Sustain Time", false);
        inputTimer =                TimerManager.instance.CreateTimer(inputTime, inputTargetTime, "Input Time", false);
        FindLongestCombo();
        OrganizeComboFamilies();
    }

    void OrganizeComboFamilies()
    {
        comboFamilies = new List<ComboFamily>();
        foreach (var Combo in currentPossibleCombos)
        {
            string thisComboFamilyName = Combo.myComboFamilyName;

            if (comboFamilies.Count == 0)   //This is the first combo being assigned to a family
            {
                ComboFamily newFam = new ComboFamily(thisComboFamilyName);
                newFam.familyList.Add(Combo);
                Combo.myComboFamilyOrder = 1;
                comboFamilies.Add(newFam);
            }
            else                            //Families already exist, check 'em
            {
                for (int i = 0; i < comboFamilies.Count; i++)   //Loop through all combo families already created
                {
                    if(comboFamilies[i].cFamilyName == thisComboFamilyName)     //If we found a family with the same name, add combo to it
                    {
                        comboFamilies[i].familyList.Add(Combo);
                        Combo.myComboFamilyOrder = comboFamilies[i].familyList.Count;
                    }
                    else if (i == comboFamilies.Count - 1&& comboFamilies[i].cFamilyName != thisComboFamilyName)   //If we reach the end of the list, and nothing matches, make a new fam
                    {
                        ComboFamily newFam = new ComboFamily(thisComboFamilyName);
                        newFam.familyList.Add(Combo);
                        Combo.myComboFamilyOrder = 1;
                        comboFamilies.Add(newFam);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentState = charcontrol.currentState.ToString();
        inputTime -= Time.deltaTime;
        if (inputTime < 0) { inputTime = 0; }

        switch (charcontrol.currentState)
        {
            case Charcontrol.State.COMBAT_Idle:
                //ClearAttackList();
                break;
        }

        //onScreenComboSustainTimer.GetComponent<SimpleTimerScript>().timerTime = comboSustainTime;
        inputTimer.GetComponent<SimpleTimerScript>().timerTime = inputTime;

        if (Input.GetButton("Light Attack") && inputTime == 0)
        {
            buttonHeldFloat += Time.deltaTime;              //If player holds the light attack button, start counting the buttonHeldFloat by adding Time.deltaTime each frame

            if (buttonHeldFloat > buttonHeldThreshold) //If we have passed the buttonHeldThreshold time, this button was held long enough to count as a held press.
            {
                Debug.Log($"Light attack is being held");
                Attack newattack = new Attack("Light_Held", lightDamageMin, Attack.AttackType.LIGHT_HELD);
                currentAttacks.Add(newattack);
                //comboSustainTime = comboSustainTargetTime;              //Resets the combo sustain timer

                CheckforCombos();
            }
        }

        if (Input.GetButtonUp("Light Attack") && inputTime == 0)
        {
            if (charcontrol.currentState == Charcontrol.State.COMBAT_Air_Attacking)
            {
                return; //This would be an air light attack
            }

            if (buttonHeldFloat < buttonHeldThreshold)
            {
                Debug.Log("Light!");
                Attack newattack = new Attack("Light", lightDamageMin, Attack.AttackType.LIGHT);
                currentAttacks.Add(newattack);
                //comboSustainTime = comboSustainTargetTime;              //Resets the combo sustain timer
                inputTime = inputTargetTime;                            //Resets input timer

                CheckforCombos();
            }
            buttonHeldFloat = 0;
        }

        if (Input.GetButtonDown("Heavy Attack"))
        {
            //Debug.Log("Heavy!");
            Attack newattack = new Attack("Heavy", heavyDamageMin, Attack.AttackType.HEAVY);
            currentAttacks.Add(newattack);
            //comboSustainTime = comboSustainTargetTime;

            CheckforCombos();
        }

        if (Input.GetButtonDown("Ranged Attack"))
        {
            Attack newattack = new Attack("Ranged", rangedDamageMin, Attack.AttackType.RANGED);
            currentAttacks.Add(newattack);
            //comboSustainTime = comboSustainTargetTime;

            CheckforCombos();
        }

        if (currentAttacks.Count > 0)   //If we pressed an attack, start counting down the combosustain timer
        {
            //comboSustainTime -= Time.deltaTime;
        }

        //if (comboSustainTime < 0)
        //{
        //    comboSustainTime = comboSustainTargetTime;
        //    ClearAttackList();
        //}
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
        //Get the current longest combo. If the current attacks length exceeds that, it means no combo will be possible and inputs will just be added for no reason. Clear the attack list
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
                            Debug.Log($"{combo.comboName} identified, sending off to be animated.");
                            currentlyComboing = true;
                            AnimateCombos(combo);

                            if (combo.canChangeState) { StartCoroutine(charanimation.ComboCharcontrolStates(combo)); } //Checks the current combo to see if it requires a state change
                            if (combo.endOfComboChain) 
                            { 
                                ClearAttackList(); 
                            }
                            break;
                        }
                    }
                    else
                    {
                        
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
        currentlyComboing = false;
        currentAttacks.Clear();
        attacksCleared?.Invoke();   //Invokes attack cleared event;
    }

    public void AnimateCombos(Combo combo)
    {
        if (charanimation) { charanimation.AnimateCombos(combo); }
        else { Debug.LogWarning("Charattacks does not have a reference to Charanimation! No combo animations can be played!"); }
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

    //Moves a combo from all light, heavy, or ranged combo lists to current possible combos
    public void AddCombo(string ComboName)  //Moves a combo from all combos to possible combos
    {
        bool comboFound = false;
        Combo foundCombo;
        for (int i = 0; i < allLightCombosEver.Count; i++)
        {
            if (allLightCombosEver[i].comboName == ComboName)
            {
                foundCombo = allLightCombosEver[i];
                allLightCombosEver.RemoveAt(i);
                currentPossibleCombos.Add(foundCombo);
                comboFound = true;
                return;
            }
        }

        if (!comboFound)
        {
            for (int i = 0; i < allHeavyCombosEver.Count; i++)
            {
                if (allHeavyCombosEver[i].comboName == ComboName)
                {
                    foundCombo = allHeavyCombosEver[i];
                    allHeavyCombosEver.RemoveAt(i);
                    currentPossibleCombos.Add(foundCombo);
                    comboFound=true;
                    return;
                }
            }
        }
        
        if (!comboFound)
        {
            for (int i = 0; i < allRangedCombosEver.Count; i++)
            {
                if (allRangedCombosEver[i].comboName == ComboName)
                {
                    foundCombo = allRangedCombosEver[i];
                    allRangedCombosEver.RemoveAt(i);
                    currentPossibleCombos.Add(foundCombo);
                    comboFound = true;
                    return;
                }
            }
        }
    }

    public void RemoveCombo(string ComboName)   //Moves a combo from possible combos to all combos
    {
        for (int i = 0; i < currentPossibleCombos.Count; i++)
        {
            Combo foundCombo;
            if (currentPossibleCombos[i].comboName == ComboName)
            {
                foundCombo = currentPossibleCombos[i];
                currentPossibleCombos.RemoveAt(i);

                Attack firstAttack = foundCombo.attackList[0];
                switch (firstAttack.attackType)
                {
                    case Attack.AttackType.LIGHT:
                        allLightCombosEver.Add(foundCombo);
                        break;
                    case Attack.AttackType.LIGHT_HELD:
                        allLightCombosEver.Add(foundCombo);
                        break;
                    case Attack.AttackType.HEAVY:
                        allHeavyCombosEver.Add(foundCombo);
                        break;
                    case Attack.AttackType.HEAVY_HELD:
                        allHeavyCombosEver.Add(foundCombo);
                        break;
                    case Attack.AttackType.RANGED:
                        allRangedCombosEver.Add(foundCombo);
                        break;
                    case Attack.AttackType.RANGED_HELD:
                        allRangedCombosEver.Add(foundCombo);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void HitEnemy(GameObject enemy, Attack.AttackType attackType) 
    {
        if (enemy.transform.position.x > transform.position.x)
        {
            //Collision to the left
        }
        else if (enemy.transform.position.x < transform.position.x)
        {
            //Collision to the right
        }

        GameManager.instance.Particle_Manager.PlayHitParticles(enemy.GetComponent<Collider2D>().ClosestPoint(transform.position));
        StartCoroutine(GameManager.instance.MeleeHitStop());
        StartCoroutine(GameManager.instance.DoScreenShake(screenShakeIntensity, screenShakeTime));

        switch (attackType)
        {
            case Attack.AttackType.LIGHT:
                currentDamageMax = lightDamageMax;
                currentDamageMin = lightDamageMin;
                break;
            case Attack.AttackType.LIGHT_HELD:
                currentDamageMax = lightDamageMax;
                currentDamageMin = lightDamageMin;
                break;
            case Attack.AttackType.HEAVY:
                currentDamageMax = heavyDamageMax;
                currentDamageMin = heavyDamageMin;
                break;
            case Attack.AttackType.HEAVY_HELD:
                currentDamageMax = heavyDamageMax;
                currentDamageMin = heavyDamageMin;
                break;
            case Attack.AttackType.RANGED:
                currentDamageMax = rangedDamageMax;
                currentDamageMin = rangedDamageMin;
                break;
            case Attack.AttackType.RANGED_HELD:
                currentDamageMax = rangedDamageMax;
                currentDamageMin = rangedDamageMin;
                break;
            default:
                break;
        }

        switch (enemy.tag)
        {
            case "target_dummy":
                target_dummy_controller dummyScript = enemy.GetComponent<target_dummy_controller>();
                dummyScript.damageDoneToMeMax = Mathf.FloorToInt(currentDamageMax);
                dummyScript.damageDoneToMeMin = Mathf.FloorToInt(currentDamageMin);
                dummyScript.damageDoneToMe = (Random.Range(currentDamageMax, currentDamageMin));
                dummyScript.TakeDamage(dummyScript.damageDoneToMe);
                break;
            default:
                break;
        }

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
