using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Debugscript : MonoBehaviour
{
    [ExecuteAlways]
    public GameObject Player;

    
    public List<Combo> allLightCombosEver, allHeavyCombosEver, allRangedCombosEver, currentPossibleCombos;
    public comboList theComboList;
    public enum comboList
    {
        allLightCombosEver,
        allHeavyCombosEver,
        allRangedCombosEver,
    }
    
    public int comboArrayNum;
    // Start is called before the first frame update
    public void Start()
    {
        //DOES NOT WORK
    }

    // Update is called once per frame
    public void Update()
    {
        //DOES NOT WORK
    }

    [ButtonMethod]
    public void AddComboToCurrentPossibleCombos()  //Moves a combo from all combos to possible combos
    {
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
        Player = GameObject.FindGameObjectWithTag("Player");
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
}
