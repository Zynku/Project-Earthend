using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyBox;
using System;
using System.Threading;
using UnityEngine.InputSystem;

public class Charattacks : MonoBehaviour
{
    //[Foldout("Variables", true)]
    public string currentState;
    Charcontrol charcontrol;
    Charanimation charanimation;
    Charaudio charaudio;
    Rigidbody2D rb2d;
    Animator animator;
    CharMeleeHitBox charMeleeHitBox;

    [Separator("Combo Lists")]
    public List<ComboFamily> comboFamilies; //This is a list of lists, hence a new type and not just List<Combo>
    public List<ComboFamilyType> comboFamilyTypes;     //Same as above
    public List<Combo> allLightCombosEver;  //Set these in inspector
    public List<Combo> allHeavyCombosEver;
    public List<Combo> allRangedCombosEver;
    public List<Combo> currentPossibleCombos;
    public List<Attack> currentAttacks;
    public List<Combo> comboBufferRef;  //Reference to the charanimation combo buffer

    [Separator("Current Combo")]
    [HideInInspector] public List<Combo> currentComboRef; //Don't ask me why it's a list. Refer to charanimation to see what past me wrote
    public bool currentlyComboing;

    [Separator("Combo Management Timers")]
    //public float comboSustainTargetTime = 0.5f; 
    //[ReadOnly] public float comboSustainTime;   //How long you have to input another attack before you can chain with previous attacks to make a combo. If it runs out, currentcombo is set to null
    public float comboChainTimeLocLeeway;       //How much time should be subtracted from the combo chain time location (located in all combos) to make a new time that is checked for in ManageComboTimer();
    public bool canAcceptAttackInput;           //Attack inputs can only be registered while this is true. It is set false while a combo is playing and before its combo chain time location.
    public float clearAttacksDelay;              //How long should we wait before receiving the command to clear the attack list.

    [HideInInspector] public int longestComboLength;
    [HideInInspector] public Combo longestCombo;

    [Separator("Damage Variables")]
    public int lightDamageMax;
    public int lightDamageMin;
    public int heavyDamageMax, heavyDamageMin;
    public int rangedDamageMax, rangedDamageMin;
    public int currentDamageMax, currentDamageMin;

    [Separator("Effects Variables")]
    public float screenShakeIntensity;
    public float screenShakeTime;


    public delegate void AttackClearEvent();
    public event AttackClearEvent attacksCleared;

    public delegate void AttackInputEvent();
    public event AttackInputEvent attackInputRegistered;

    // Start is called before the first frame update
    void Start()
    {
        charcontrol = GetComponent<Charcontrol>();
        charanimation = GetComponent<Charanimation>();
        charaudio = GetComponent<Charaudio>();
        rb2d = GetComponent<Rigidbody2D>();
        charMeleeHitBox = GetComponentInChildren<CharMeleeHitBox>();
        animator = GetComponent<Animator>();
        canAcceptAttackInput = true;
        FindLongestCombo();
        //OrganizeComboFamilies();
        OrganizeComboFamilyTypes();

        Charinputs.instance.lightAttack.canceled += OnLightPressed;  //Subscribes OnLightPressed to the canceled callback of the "Light attack" input action. It is fired every time "light attack" is let go
        Charinputs.instance.lightAttack.performed += OnLightHeld;    //Subscribes OnLightHeld to the performed callback of the "light attack" input action. It is fired every time "lightattack" is held longer than the held threshold, defined in the input settings.
        Charinputs.instance.heavyAttack.canceled += OnHeavyPressed;
        Charinputs.instance.heavyAttack.performed += OnHeavyHeld;
        Charinputs.instance.rangedAttack.canceled += OnRangedPressed;
        Charinputs.instance.rangedAttack.performed += OnRangedHeld;
    }

    void OrganizeComboFamilyTypes()
    {
        comboFamilyTypes = new List<ComboFamilyType>();
        foreach (var Combo in currentPossibleCombos)
        {
            string thisComboFamilyTypeName = Combo.comboType.ToString();

            if (comboFamilyTypes.Count == 0)   //This is the first combo being assigned to a family type
            {
                ComboFamilyType newFamType = new ComboFamilyType(thisComboFamilyTypeName);  //Create new type list
                newFamType.familyTypeList.Add(Combo);   //Add the combo to the new type list
                comboFamilyTypes.Add(newFamType);       //Add the type list to the list of lists
            }
            else //Families already exist, check 'em
            {
                for (int i = 0; i < comboFamilyTypes.Count; i++)   //Loop through all combo family types already created
                {
                    if(comboFamilyTypes[i].cFamilyTypeName == thisComboFamilyTypeName)     //If we found a family type with the same name, add combo to it
                    {
                        comboFamilyTypes[i].familyTypeList.Add(Combo);
                        break;
                    }
                    else if (i == comboFamilyTypes.Count - 1 && comboFamilyTypes[i].cFamilyTypeName != thisComboFamilyTypeName)   //If we reach the end of the list, and nothing matches, make a new fam
                    {
                        ComboFamilyType newFamType = new ComboFamilyType(thisComboFamilyTypeName);
                        newFamType.familyTypeList.Add(Combo);
                        comboFamilyTypes.Add(newFamType);
                        break;
                    }
                }
            }
        }
    }

    void OrganizeComboFamilies()//Would delete this but it seems like useful code so idk
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
                    if (comboFamilies[i].cFamilyName == thisComboFamilyName)     //If we found a family with the same name, add combo to it
                    {
                        comboFamilies[i].familyList.Add(Combo);
                        Combo.myComboFamilyOrder = comboFamilies[i].familyList.Count;
                    }
                    else if (i == comboFamilies.Count - 1 && comboFamilies[i].cFamilyName != thisComboFamilyName)   //If we reach the end of the list, and nothing matches, make a new fam
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

    public void OnLightPressed(InputAction.CallbackContext context)
    {
        if(!canAcceptAttackInput) { return; }
        if (charcontrol.currentState == Charcontrol.State.COMBAT_Air_Attacking /* Add condition for regular combat air */)
        {
            return; //This would be an air light attack
        }
        Debug.Log("Light!");
        Attack newattack = new("Light", lightDamageMin, Attack.AttackType.LIGHT);
        currentAttacks.Add(newattack);
        canAcceptAttackInput = false;
        CheckforCombos();
        attackInputRegistered?.Invoke();
    }

    public void OnLightHeld(InputAction.CallbackContext context)
    {
        if (!canAcceptAttackInput) { return; }
        Debug.Log($"Light attack is being held");
        Attack newattack = new("Light_Held", lightDamageMin, Attack.AttackType.LIGHT_HELD);
        currentAttacks.Add(newattack);
        canAcceptAttackInput = false;
        CheckforCombos();
        attackInputRegistered?.Invoke();
    }

    public void OnHeavyPressed(InputAction.CallbackContext context)
    {
        if (!canAcceptAttackInput) { return; }
        if (charcontrol.currentState == Charcontrol.State.COMBAT_Air_Attacking /* Add condition for regular combat air */)
        {
            return; //This would be an air heavy attack
        }
        Debug.Log("Heavy!");
        Attack newattack = new("Heavy", heavyDamageMin, Attack.AttackType.HEAVY);
        currentAttacks.Add(newattack);
        canAcceptAttackInput = false;
        CheckforCombos();
        attackInputRegistered?.Invoke();
    }

    public void OnHeavyHeld(InputAction.CallbackContext context)
    {
        if (!canAcceptAttackInput) { return; }
        Debug.Log($"Heavy attack is being held");
        Attack newattack = new("Heavy_Held", heavyDamageMin, Attack.AttackType.HEAVY_HELD);
        currentAttacks.Add(newattack);
        canAcceptAttackInput = false;
        CheckforCombos();
        attackInputRegistered?.Invoke();
    }

    public void OnRangedPressed(InputAction.CallbackContext context)
    {
        if (!canAcceptAttackInput) { return; }
        if (charcontrol.currentState == Charcontrol.State.COMBAT_Air_Attacking /* Add condition for regular combat air */)
        {
            return; //This would be an air ranged attack
        }
        Debug.Log($"Ranged!");
        Attack newattack = new("Ranged", rangedDamageMin, Attack.AttackType.RANGED);
        currentAttacks.Add(newattack);
        canAcceptAttackInput = false;
        CheckforCombos();
        attackInputRegistered?.Invoke();
    }

    public void OnRangedHeld(InputAction.CallbackContext context)
    {
        if (!canAcceptAttackInput) { return; }
        Debug.Log($"Ranged is being held");
        Attack newattack = new("Ranged", rangedDamageMin, Attack.AttackType.RANGED_HELD);
        currentAttacks.Add(newattack);
        canAcceptAttackInput = false;
        CheckforCombos();
        attackInputRegistered?.Invoke();
    }

    void Update()
    {
        currentState = charcontrol.currentState.ToString();
        ManageAttackInputs();
        //Debug.LogWarning($"Accept input is {canAcceptAttackInput}!");
    }

    public void ManageAttackInputs()
    {
        AnimatorStateInfo currentAnimSInfo = animator.GetCurrentAnimatorStateInfo(0);
        comboBufferRef = charanimation.comboBuffer;
        currentComboRef = charanimation.currentCombo;

        if (currentComboRef.Count > 0)
        {
            float newComboChainTimeLoc = currentComboRef[0].comboChainTimeLocation - comboChainTimeLocLeeway;
            if (currentAnimSInfo.normalizedTime >= newComboChainTimeLoc)    //If we pass the combo's chain time location + the offset, consider it enough time passing for inputs to be allowed again
            {
                canAcceptAttackInput = true;
            }
            else
            {
                canAcceptAttackInput = false;
            }
        }
        else
        {
            canAcceptAttackInput = true;
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
        //This is a debug section-------------------------------------------------------------------------
        List<string> currentAttackNames = new List<string>();
        foreach (var atk in currentAttacks) { currentAttackNames.Add(atk.attackName);}
        Debug.Log($"Current attack list is {string.Join(", ", currentAttackNames)}");
        //End of debug section----------------------------------------------------------------------------

        charcontrol.canFlipXDir();
        //Get the current longest combo. If the current attacks length exceeds that, it means no combo will be possible and inputs will just be added for no reason. Clear the attack list
        if (currentAttacks.Count > longestComboLength + 1) { ClearAttackList(); }

        ComboFamilyType comboFamType;   //Find the combofamily we're going to search through
        if (currentAttacks.Count > 0)
        {
            foreach (var cFamType in comboFamilyTypes)  //Loop through all families
            {
                if (cFamType.cFamilyTypeName == currentAttacks[0].attackType.ToString()) //If it matches, we using that fam
                {
                    comboFamType = cFamType;
                    Debug.Log($"Combo family is {comboFamType.cFamilyTypeName}");
                    for (int h = 0; h < comboFamType.familyTypeList.Count; h++)  
                    {
                        Combo combo = comboFamType.familyTypeList[h];   //Loop through all combos in this combo family
                        Debug.Log($"Checking combo named {combo.comboName}.");
                        if (currentAttacks.Count == combo.attackList.Count)//If the combo being checked has the same amount of attacks in its list
                        {
                            Debug.Log(combo.comboName + " has the same amount of attacks as the current attack count, which is " + currentAttacks.Count);
                            for (int i = 0; i < combo.attackList.Count; i++)    //Loop through all attacks in the combo attack list, checking if they match
                            {
                                Debug.Log($"Checking {combo.comboName} to see if {combo.attackList[i].attackType} is the same as {currentAttacks[i].attackType} for attack #{i + 1}");
                                if (combo.attackList[i].attackType == currentAttacks[i].attackType)
                                {
                                    if (i == combo.attackList.Count - 1 && combo.attackList[i].attackType == currentAttacks[i].attackType)  //If the last attack matches...
                                    {
                                        Debug.Log($"----------------------------------{combo.comboName} identified, sending off to be animated.-------------------------------------");
                                        currentlyComboing = true;
                                        AnimateCombos(combo);

                                        if (combo.canChangeState) { StartCoroutine(charanimation.ComboCharcontrolStates(combo)); } //Checks the current combo to see if it requires a state change
                                        if (combo.endOfComboChain) { ClearAttackList(); }
                                        return;
                                    }

                                    if (i == combo.attackList.Count - 1 && combo.attackList[i].attackType != currentAttacks[i].attackType)  //If the last attack DOESN'T match...
                                    {
                                        Debug.Log($"So close! the final attack of {combo.comboName} doesn't match! ({combo.attackList[i].attackType} vs. {currentAttacks[i].attackType})");
                                        ClearAttackList();
                                        return;
                                    }
                                }
                                else
                                {
                                    Debug.Log($"{combo.attackList[i].attackType} is NOT the same as {currentAttacks[i].attackType}. Abandoning this list by clearing it.");
                                    ClearAttackList();
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            Debug.Log($"{combo.comboName} doesn't have the same amount of attacks, checking next combo...");
                            continue;
                        }
                        if (h == comboFamType.familyTypeList.Count)
                        {
                            //Debug.Log($"No combos in this family type match, clearing attack list and adding the new attack instead");
                            ClearAttackList();

                            currentlyComboing = true;
                            AnimateCombos(combo);

                            if (combo.canChangeState) { StartCoroutine(charanimation.ComboCharcontrolStates(combo)); } //Checks the current combo to see if it requires a state change
                            if (combo.endOfComboChain) { ClearAttackList(); }
                            continue;
                        }
                    }
                }
            }
        }
    }

    public void ClearAttackList()
    {
        currentlyComboing = false;
        StopCoroutine(ClearAttackListDelay());
        StartCoroutine(ClearAttackListDelay());
    }

    public IEnumerator ClearAttackListDelay()
    {
        yield return new WaitForSeconds(clearAttacksDelay);
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

    public void HitEnemy(GameObject enemy, Attack.AttackType attackType) //Is called from the player hitboxes, alongside all relevant information.
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
        GameManager.instance.DoScreenShake(screenShakeIntensity, screenShakeTime);

        switch (currentComboRef[0].comboType)
        {
            case Combo.ComboType.LIGHT:
                currentDamageMax = lightDamageMax;
                currentDamageMin = lightDamageMin;
                break;

            case Combo.ComboType.HEAVY:
                currentDamageMax = heavyDamageMax;
                currentDamageMin = heavyDamageMin;
                break;

            case Combo.ComboType.RANGED:
                currentDamageMax = rangedDamageMax;
                currentDamageMin = rangedDamageMin;
                break;
            default:
                currentDamageMax = -1;
                currentDamageMin = -1;
                break;
        }

        switch (enemy.tag)
        {
            case "target_dummy":
                //target_dummy_controller dummyScript = enemy.GetComponent<target_dummy_controller>();
                //dummyScript.damageDoneToMeMax = Mathf.FloorToInt(currentDamageMax); //These values are assigned in case a script needs to reference them
                //dummyScript.damageDoneToMeMin = Mathf.FloorToInt(currentDamageMin);
                //int damageDoneDummy = (UnityEngine.Random.Range(currentDamageMax, currentDamageMin));
                //dummyScript.damageDoneToMe = damageDoneDummy;
                //dummyScript.TakeDamage(damageDoneDummy);
                break;
            case "enemy":
                Enemymain enemyScript = enemy.GetComponent<Enemymain>();
                enemyScript.damageDoneToMeMax = Mathf.FloorToInt(currentDamageMax); 
                enemyScript.damageDoneToMeMin = Mathf.FloorToInt(currentDamageMin);
                int damageDoneEnemy = (UnityEngine.Random.Range(currentDamageMax, currentDamageMin));
                enemyScript.damageDoneToMe = damageDoneEnemy;
                enemyScript.lastDmgSource = gameObject;
                enemyScript.TakeDamage(damageDoneEnemy);
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
