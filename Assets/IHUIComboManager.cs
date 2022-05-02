using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using TMPro;

public class IHUIComboManager : MonoBehaviour
{
    GameObject Player;
    Charattacks charattacks;
    GameObject contentScreen;

    public GameObject comboListPrefab;
    public List<GameObject> comboListsList;

    public TextMeshProUGUI whatKindOfCombosTEXT;

    [ReadOnly] public List<Combo> allLightCombosEver;
    [ReadOnly] public List<Combo> allHeavyCombosEver;
    [ReadOnly] public List<Combo> allRangedCombosEver;
    [ReadOnly] public List<Combo> currentPossibleCombos;

    public enum comboPageType
    {
        lightCombos,
        heavyCombos,
        rangedCombos,
        currentPossibleCombos
    };
    public comboPageType cPType;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameManager.instance.Player;
        charattacks = Player.GetComponent<Charattacks>();
        contentScreen = GameObject.Find("Combolist Content");

        

        cPType = comboPageType.lightCombos;
        StartCoroutine(AssignAllCombos());
    }

    // Update is called once per frame
    void Update()
    {
        allLightCombosEver = charattacks.allLightCombosEver;
        allHeavyCombosEver = charattacks.allHeavyCombosEver;
        allRangedCombosEver = charattacks.allRangedCombosEver;
        currentPossibleCombos = charattacks.currentPossibleCombos;
    }

    public void SwitchToLightCombosPage()
    {
        if (cPType == comboPageType.lightCombos)
        {
            return;
        }
        else
        {
            cPType = comboPageType.lightCombos;
            whatKindOfCombosTEXT.text = "Light Combos";
            StartCoroutine(AssignAllCombos());
        }
    }

    public void SwitchToHeavyCombosPage()
    {
        if (cPType == comboPageType.heavyCombos)
        {
            return;
        }
        else
        {
            cPType = comboPageType.heavyCombos;
            whatKindOfCombosTEXT.text = "Heavy Combos";
            StartCoroutine(AssignAllCombos());
        }
    }

    public void SwitchToRangedCombosPage()
    {
        if (cPType == comboPageType.rangedCombos)
        {
            return;
        }
        else
        {
            cPType = comboPageType.rangedCombos;
            whatKindOfCombosTEXT.text = "Ranged Combos";
            StartCoroutine(AssignAllCombos());
        }
    }

    public void SwitchToCurrentPossibleCombosPage()
    {

        cPType = comboPageType.currentPossibleCombos;
        whatKindOfCombosTEXT.text = "Current Combos";
        StartCoroutine(AssignAllCombos());

    }

    public IEnumerator AssignAllCombos()
    {
        if (comboListsList.Count > 0)   //If there are left over combos being shown from the last time this function was called, clear them first
        {
            foreach (var item in comboListsList)
            {
                Destroy(item);
            }
            comboListsList.Clear();
            //yield return new WaitForSeconds(0.5f);
        }

        
         switch (cPType)
         {
             case comboPageType.lightCombos:
                 foreach (var item in allLightCombosEver)
                 {
                     GameObject comboList = Instantiate(comboListPrefab, contentScreen.transform);
                     comboList.GetComponent<IHUIComboListDesc>().myCombo = item;
                     comboList.GetComponent<IHUIComboListDesc>().AssignMyCombo();
                     comboListsList.Add(comboList);
                     //yield return new WaitForSecondsRealtime(0.1f);
                 }
                 break;
             case comboPageType.heavyCombos:
                 foreach (var item in allHeavyCombosEver)
                 {
                     GameObject comboList = Instantiate(comboListPrefab, contentScreen.transform);
                     comboList.GetComponent<IHUIComboListDesc>().myCombo = item;
                     comboList.GetComponent<IHUIComboListDesc>().AssignMyCombo();
                     comboListsList.Add(comboList);
                     //yield return new WaitForSeconds(0.2f);
                 }
                 break;
             case comboPageType.rangedCombos:
                 foreach (var item in allRangedCombosEver)
                 {
                     GameObject comboList = Instantiate(comboListPrefab, contentScreen.transform);
                     comboList.GetComponent<IHUIComboListDesc>().myCombo = item;
                     comboList.GetComponent<IHUIComboListDesc>().AssignMyCombo();
                     comboListsList.Add(comboList);
                     //yield return new WaitForSeconds(0.3f);
                 }
                 break;
             case comboPageType.currentPossibleCombos:
                 foreach (var item in currentPossibleCombos)
                 {
                     GameObject comboList = Instantiate(comboListPrefab, contentScreen.transform);
                     comboList.GetComponent<IHUIComboListDesc>().myCombo = item;
                     comboList.GetComponent<IHUIComboListDesc>().AssignMyCombo();
                     comboListsList.Add(comboList);
                     //yield return new WaitForSeconds(0.05f);
                 }
                 break;
         }
        yield return new WaitForSeconds(0.1f);
    }
}
