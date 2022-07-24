using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using TMPro;
using System.Linq;

//[ExecuteAlways]
public class Debugscript : MonoBehaviour
{
    public GameObject Player;
    Charanimation charanimation;
    Charcontrol charcontrol;
    Charattacks charattacks;
    public Debug_Group_Section rightHandSection;
    
    [Foldout("Combo Stuff", true)]
    public List<Combo> allLightCombosEver, allHeavyCombosEver, allRangedCombosEver, currentPossibleCombos;
    //private List<Combo> comboBuffer;
    [HideInInspector] public List<string> comboBufferNames;
    [HideInInspector] public List<string> comboAttackNames;
    public comboList theComboList;
    public enum comboList
    {
        allLightCombosEver,
        allHeavyCombosEver,
        allRangedCombosEver,
    }
    
    public int comboArrayNum;
    public Combo comboBeingMovedFromPossible;
    public Combo comboBeingMovedToPossible;

    [Foldout("Debug", true)]
    [Separator("Debug Toggle")]
    public bool showOnScreenDebug;

    [Separator("Debug Sections")]
    public GameObject debugSectionPrefab;
    public List<GameObject> debugSections;

    [Separator("Debug Switches")]
    public bool showDeltaTime;
    public bool showComboBuffer;
    public bool showComboAttacks;
    public bool showAttackInputs;
    public bool showPlayerState;

    [Separator("Attacks & Combos")]
    private float placeHolder;
    private void OnEnable()
    {
        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            charanimation = Player.GetComponent<Charanimation>();
            charcontrol = Player.GetComponent<Charcontrol>();
            charattacks = Player.GetComponent<Charattacks>();
        }

        if (showDeltaTime) CreateDebugSection(Debug_Section.DebugType.DeltaTime);
        if (showComboBuffer) CreateDebugSection(Debug_Section.DebugType.ComboBuffer);
        if (showComboAttacks) CreateDebugSection(Debug_Section.DebugType.ComboAttacks);
        if (showAttackInputs) CreateDebugSection(Debug_Section.DebugType.AttackInputs);
        if (showPlayerState) CreateDebugSection(Debug_Section.DebugType.PlayerState);
    }

    public void CreateDebugSection(Debug_Section.DebugType dbt)
    {
        GameObject dbs = Instantiate(debugSectionPrefab, Vector3.zero, Quaternion.identity, rightHandSection.gameObject.transform);
        dbs.GetComponent<Debug_Section>().debugType = dbt;
        debugSections.Add(dbs);
    }

    private void OnDisable()
    {
        Player = null;
    }

    public void Update()
    {
        if (comboArrayNum < currentPossibleCombos.Count - 1)
        {
            comboBeingMovedFromPossible = currentPossibleCombos[comboArrayNum];
        }
        else
        {
            comboBeingMovedFromPossible = null;
        }

        //comboBuffer = Player.GetComponent<Charanimation>().comboBuffer;
        comboBufferNames = charanimation.comboBuffer.Select(o => o.comboName).ToList();
        comboAttackNames = charattacks.currentAttacks.Select(o => o.attackType.ToString()).ToList();


        switch (theComboList)
        {
            case comboList.allLightCombosEver:
                if (allLightCombosEver.Count > 0)
                {
                    comboBeingMovedToPossible = allLightCombosEver[comboArrayNum];
                }
                else
                {
                    comboBeingMovedToPossible = null;
                }
                break;
            case comboList.allHeavyCombosEver:
                if (allHeavyCombosEver.Count > 0)
                {
                    comboBeingMovedToPossible = allHeavyCombosEver[comboArrayNum];
                }
                else
                {
                    comboBeingMovedToPossible = null;
                }
                break;
            case comboList.allRangedCombosEver:
                if (allRangedCombosEver.Count > 0)
                {
                    comboBeingMovedToPossible = allRangedCombosEver[comboArrayNum];
                }
                else
                {
                    comboBeingMovedToPossible = null;
                }

                break;
            default:
                break;
        }
        ReassignToPlayer();     //This is needed as when references are assigned, it makes it local. By reassigning, this makes sure all changes made here are reflected in player

        if (showOnScreenDebug)
        {
            rightHandSection.gameObject.SetActive(true);
        }
        else
        {
            rightHandSection.gameObject.SetActive(false);
        }

    }

    public void ReassignToPlayer()
    {
        allLightCombosEver = Player.GetComponent<Charattacks>().allLightCombosEver;
        allHeavyCombosEver = Player.GetComponent<Charattacks>().allHeavyCombosEver;
        allRangedCombosEver = Player.GetComponent<Charattacks>().allRangedCombosEver;
        currentPossibleCombos = Player.GetComponent<Charattacks>().currentPossibleCombos;
    }

    [ButtonMethod]
    public void ForceReassignInEditor()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Player.GetComponent<Charattacks>().allLightCombosEver = allLightCombosEver;
        Player.GetComponent<Charattacks>().allHeavyCombosEver = allHeavyCombosEver;
        Player.GetComponent<Charattacks>().allRangedCombosEver = allRangedCombosEver;
        Player.GetComponent<Charattacks>().currentPossibleCombos = currentPossibleCombos; 

    }

    [ButtonMethod]
    public void AddComboToCurrentPossibleCombos()  //Moves a combo from all combos to possible combos
    {
        Debug.LogWarning("Don't forget to copy component, then paste component as values in Charattacks to make sure changes are saved!");
        Player = GameObject.FindGameObjectWithTag("Player");
        allHeavyCombosEver = Player.GetComponent<Charattacks>().allHeavyCombosEver;
        allRangedCombosEver = Player.GetComponent<Charattacks>().allRangedCombosEver;
        currentPossibleCombos = Player.GetComponent<Charattacks>().currentPossibleCombos;
        Combo foundCombo;

        switch (theComboList)
        {
            case comboList.allLightCombosEver:
                for (int i = 0; i < allLightCombosEver.Count; i++)
                {
                    if (allLightCombosEver[i].comboName == allLightCombosEver[comboArrayNum].comboName)
                    {
                        foundCombo = allLightCombosEver[i];
                        allLightCombosEver.RemoveAt(i);
                        currentPossibleCombos.Add(foundCombo);
                        Debug.Log($"Found a combo named {foundCombo.comboName} in All Light Combos. Moving it to Current Possible Combos");
                        return;
                    }
                }
                break;
            case comboList.allHeavyCombosEver:
                for (int i = 0; i < allHeavyCombosEver.Count; i++)
                {
                    if (allHeavyCombosEver[i].comboName == allHeavyCombosEver[comboArrayNum].comboName)
                    {
                        foundCombo = allHeavyCombosEver[i];
                        allHeavyCombosEver.RemoveAt(i);
                        currentPossibleCombos.Add(foundCombo);
                        Debug.Log($"Found a combo named {foundCombo.comboName} in All Heavy Combos. Moving it to Current Possible Combos");
                        return;
                    }
                }
                break;
            case comboList.allRangedCombosEver:
                for (int i = 0; i < allRangedCombosEver.Count; i++)
                {
                    if (allRangedCombosEver[i].comboName == allRangedCombosEver[comboArrayNum].comboName)
                    {
                        foundCombo = allRangedCombosEver[i];
                        allRangedCombosEver.RemoveAt(i);
                        currentPossibleCombos.Add(foundCombo);
                        Debug.Log($"Found a combo named {foundCombo.comboName} in All Ranged Combos. Moving it to Current Possible Combos");
                        return;
                    }
                }
                break;
            default:
                break;
        }
    }

    [ButtonMethod]
    public void RemoveComboFromCurrentPossibleCombos()   //Moves a combo from possible combos to all combos
    {
        Debug.LogWarning("Don't forget to copy component, then paste component as values in Charattacks to make sure changes are saved!");
        allHeavyCombosEver = Player.GetComponent<Charattacks>().allHeavyCombosEver;
        allRangedCombosEver = Player.GetComponent<Charattacks>().allRangedCombosEver;
        currentPossibleCombos = Player.GetComponent<Charattacks>().currentPossibleCombos;

        for (int i = 0; i < currentPossibleCombos.Count; i++)
        {
            Combo foundCombo;
            if (currentPossibleCombos[i].comboName == currentPossibleCombos[comboArrayNum].comboName)
            {
                foundCombo = currentPossibleCombos[i];
                currentPossibleCombos.RemoveAt(i);

                Attack firstAttack = foundCombo.attackList[0];
                switch (firstAttack.attackType)
                {
                    case Attack.AttackType.LIGHT:
                        allLightCombosEver.Add(foundCombo);
                        Debug.Log($"Found a combo named {foundCombo.comboName} in Current Possible Combos. Moving it to All Light Combos");
                        break;
                    case Attack.AttackType.LIGHT_HELD:
                        allLightCombosEver.Add(foundCombo);
                        Debug.Log($"Found a combo named {foundCombo.comboName} in Current Possible Combos. Moving it to All Light Combos");
                        break;
                    case Attack.AttackType.HEAVY:
                        allHeavyCombosEver.Add(foundCombo);
                        Debug.Log($"Found a combo named {foundCombo.comboName} in Current Possible Combos. Moving it to All Heavy Combos");
                        break;
                    case Attack.AttackType.HEAVY_HELD:
                        allHeavyCombosEver.Add(foundCombo);
                        Debug.Log($"Found a combo named {foundCombo.comboName} in Current Possible Combos. Moving it to All Heavy Combos");
                        break;
                    case Attack.AttackType.RANGED:
                        allRangedCombosEver.Add(foundCombo);
                        Debug.Log($"Found a combo named {foundCombo.comboName} in Current Possible Combos. Moving it to All Ranged Combos");
                        break;
                    case Attack.AttackType.RANGED_HELD:
                        allRangedCombosEver.Add(foundCombo);
                        Debug.Log($"Found a combo named {foundCombo.comboName} in Current Possible Combos. Moving it to All RAnged Combos");
                        break;
                    default:
                        break;
                }
            }
        }
    }

    [ButtonMethod]
    public void ClearComboBuffer()
    {
        Charanimation charanimation = Player.GetComponent<Charanimation>();
        Debug.Log($"Clearing {charanimation.comboBuffer.Count} combos from combo buffer");
        charanimation.ClearComboBuffer();
    }

    [ButtonMethod]
    public void ResetEmptyComboFamilyNames()
    {
        foreach (var item in Player.GetComponent<Charattacks>().currentPossibleCombos)
        {
            if (item.myComboFamilyName == "")
            {
                item.myComboFamilyName = "No Family";
            }
        }
    }
}
