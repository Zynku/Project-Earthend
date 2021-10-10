using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Charattacks : MonoBehaviour
{
    public string currentState;
    Charcontrol charcontrol;
    Charanimation charanimation;

    [Header("Combo Variables")]
    public List<Combo> allLightCombosEver;  //Set these in inspector
    public List<Combo> allHeavyCombosEver;
    public List<Combo> allRangedCombosEver;
    public List<Combo> currentPossibleCombos;
    public List<Attack> currentAttacks;
    public Combo currentCombo;
    public float comboSustainTargetTime = 0.5f; //How long you have to input another attack before the combo basically fizzles out
    public float comboSustainTime;
    public float comboExecuteTargetTime = 0.5f;  //How long before current combo returns to nothing
    public float comboExecuteTime;
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
        comboSustainTime = comboSustainTargetTime;
        comboExecuteTime = comboExecuteTargetTime;
        onScreenComboSustainTimer = TimerManager.instance.CreateTimer(comboSustainTime, comboSustainTargetTime, "Combo Sustain Time", false);
        onScreenComboExecuteTimer = TimerManager.instance.CreateTimer(comboExecuteTime, comboExecuteTargetTime, "Combo Execute Time", false);
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

        //Make a timer dictating how long current attacks are kept in current attacks list
        //When player presses any attack key, immediately transition to combat = true, and the appropriate state,
        //Check which button was pressed, add that attack to list of current attacks

        onScreenComboSustainTimer.GetComponent<SimpleTimerScript>().timerTime = comboSustainTime;
        onScreenComboExecuteTimer.GetComponent<SimpleTimerScript>().timerTime = comboExecuteTime;

        if (Input.GetButtonDown("Light Attack"))
        {
            //Debug.Log("Pressing light attack");
            Attack newattack = new Attack();
            newattack.SetupAttack("Light", lightDamageMin, Attack.AttackType.LIGHT);
            currentAttacks.Add(newattack);
            comboSustainTime = comboSustainTargetTime;
            CheckforCombos();
        }

        if (Input.GetButtonDown("Heavy Attack"))
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
                currentAttacks.Clear();
            }
        }

        if (currentCombo.comboName != null) //If we executed a combo start counting down to end the combo
        {
            comboExecuteTime -= Time.deltaTime;
            if (comboExecuteTime < 0)
            {
                comboExecuteTime = comboExecuteTargetTime;
                currentCombo = null;
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
        foreach (Combo combo in currentPossibleCombos) //TODO: This temporarily checks all combos, change it to check all current possible combos instead
        {
            if (currentAttacks.Count == combo.attackList.Count)    //Has the same amount of attacks in its list
            {
                //Debug.Log(combo.comboName + " has the same amount of attacks as the current attack count, which is " + currentAttacks.Count);
                for (int i = 0; i < combo.attackList.Count; i++)    //Loop through all attacks in the combo attack list, checking if they match
                {
                    //Debug.Log("Checking " + combo.comboName + " to see if " + combo.attackList[i].attackType + " is the same as " + currentAttacks[i].attackType);
                    if (combo.attackList[i].attackType == currentAttacks[i].attackType)
                    {
                        if (i == combo.attackList.Count - 1 && combo.attackList[i].attackType == currentAttacks[i].attackType)     
                        {
                            //Debug.Log(combo.comboName + " matches perfectly");
                            comboExecuteTime = comboExecuteTargetTime;
                            currentCombo = combo;
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    public void AddCombo()  //Moves a combo from all combos to possible combos
    {

    }

    public void RemoveCombo()   //Moves a combo from possible combos to all combos
    {

    }
}
