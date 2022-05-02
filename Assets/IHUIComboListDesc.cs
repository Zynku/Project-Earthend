using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MyBox;

public class IHUIComboListDesc : MonoBehaviour
{
    public GameObject Player;
    public Charattacks charattacks;
    IHUIComboManager comboManager;

    [ReadOnly] public List<Combo> allLightCombosEver;
    [ReadOnly] public List<Combo> allHeavyCombosEver;
    [ReadOnly] public List<Combo> allRangedCombosEver;
    [ReadOnly] public List<Combo> currentPossibleCombos;

    public TextMeshProUGUI comboNameTEXT;
    public GameObject comboBlockPrefab;
    public GameObject comboContinuePrefab;
    public Combo myCombo;
    public int amountOfAttacks = -1;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameManager.instance.Player;
        charattacks = Player.GetComponent<Charattacks>();
        comboManager = GetComponentInParent<IHUIComboManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (myCombo != null)
        {
            amountOfAttacks = myCombo.attackList.Count;
        }
        else
        {
            amountOfAttacks = -1;    
        }
        AssignRefs();
    }

    public void AssignRefs()
    {
        allLightCombosEver = charattacks.allLightCombosEver;
        allHeavyCombosEver = charattacks.allHeavyCombosEver;
        allRangedCombosEver = charattacks.allRangedCombosEver;
        currentPossibleCombos = charattacks.currentPossibleCombos; 
    }

    public void AssignMyCombo() //Instantiates names and arrows as needed for this combo
    {
        comboNameTEXT.text = myCombo.comboName;
        StartCoroutine(InstantiateStuff());
    }

    public IEnumerator InstantiateStuff()
    {
        List<Attack> thisAttackList = myCombo.attackList;
        for (int i = 0; i < thisAttackList.Count; i++)  //Loops through each attack and makes a name for each one
        {
            GameObject block = Instantiate(comboBlockPrefab, transform);
            block.GetComponentInChildren<TextMeshProUGUI>().text = ConvertAttackType(thisAttackList[i]);
            Debug.Log($"Instantiated a block for a {ConvertAttackType(thisAttackList[i])}");
            yield return new WaitForSeconds(0.01f);

            if (i != thisAttackList.Count - 1)  //If there's more attacks to come after this one, make an arrow so they flow
            {
                Instantiate(comboContinuePrefab, transform);
                Debug.Log("$Instantiated an arrow");
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    public string ConvertAttackType(Attack attack)  //Converts attack type to a string
    {
        string name = null;
        switch (attack.attackType)
        {
            case Attack.AttackType.LIGHT:
                name = "Light";
                break;
            case Attack.AttackType.LIGHT_HELD:
                name = "Hold Light";
                break;
            case Attack.AttackType.HEAVY:
                name = "Heavy";
                break;
            case Attack.AttackType.HEAVY_HELD:
                name = "Hold Heavy";
                break;
            case Attack.AttackType.RANGED:
                name = "Ranged";
                break;
            case Attack.AttackType.RANGED_HELD:
                name = "Hold Ranged";
                break;
            default:
                break;
        }
        return name;
    }
}
