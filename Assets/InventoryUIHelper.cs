using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUIHelper : MonoBehaviour
{
    public GameObject pickedUpPrefab;
    public List<GameObject> pickedUpTexts;
    public List<GameObject> offScreenTexts;

    public bool isTextLoActive;
    public bool isTextMidActive;
    public bool isTextHiActive;

    private void Update()
    {
        ManageOffScreenTexts();
    }

    public void ShowPickedUpText(ItemScriptable item)
    {
        //Debug.Log("Starting ShowPickedUpText loop for " + item.name);
        bool textfound = false;

        //Creates a new text if no texts are on screen
        if (pickedUpTexts.Count == 0)
        {
            //Debug.Log("No texts exist! Therefore creating new text out of if statement for " + item.name);
            CreateNewText(item);
        }

        //If there are texts onscreen, but no texts offscreen
        if (pickedUpTexts.Count > 0)
        {
            foreach (GameObject text in pickedUpTexts)
            {
                //Debug.Log("Checking " + pickedUpTexts.Count + " texts in pickedUpTexts array containing " + item.name);
                if (text.GetComponent<TextMeshProUGUI>().text.Contains(item.name.ToString()))
                {
                    //Debug.Log("Text found containing " + item.name + " . Adding " + item.amount + " to it");
                    //Add the amount that was picked up to the amount already being displayed.
                    text.GetComponent<PickedUpTextScript>().myItemAmount += item.amount;
                    text.GetComponent<PickedUpTextScript>().AssignNewNameandAmount();
                    //Debug.Log("New text says contains " + text.GetComponent<PickedUpTextScript>().myItemAmount + " " + item.name);
                    textfound = true;
                    break;
                }
            }
        }
        //If there are texts onscreen AND texts offscreen
        if (offScreenTexts.Count > 0)
        {
            //Check each text object in offscreentexts to see if it contains the same name as the one you just picked up
            foreach (GameObject text in offScreenTexts)
            {
                //If you find a text with the name...
                //Debug.Log("Checking " + offScreenTexts.Count + " texts in offScreenTexts array containing " + item.name);
                if (text.GetComponent<TextMeshProUGUI>().text.Contains(item.name))
                {
                    //Debug.Log("Text found containing " + item.name + " that says " + text.GetComponent<TextMeshProUGUI>().text + " . Adding " + item.amount + " to it") ;
                    //...add the amount that was picked up to the amount already being displayed.
                    text.GetComponent<PickedUpTextScript>().myItemAmount += item.amount;
                    text.GetComponent<PickedUpTextScript>().AssignNewNameandAmount();
                    //Debug.Log("New text says contains " + text.GetComponent<PickedUpTextScript>().myItemAmount + " " + item.name);
                    textfound = true;
                    break;
                }
            }
        }
        //If you don't find one in pickedUpTexts or offscreenTexts
        if (!textfound)
        {
            //Debug.Log("No text in pickedUpTexts or offScreenTexts found containing the same name. Creating new text out of for loop for " + item.name);
            CreateNewText(item);
        }
    }
    public void CreateNewText(ItemScriptable item)
    {
        //If there is no text on screen with the same name as the item you picked up
        {
            GameObject newText = Instantiate(pickedUpPrefab, gameObject.transform);
            newText.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -20, 0);
            newText.GetComponent<PickedUpTextScript>().AssignNameandAmount(item);
            pickedUpTexts.Add(newText);

            AssignTextToPosition(newText);
        }
    }

    public void AssignTextToPosition(GameObject newText)
    {
        if (!isTextLoActive)
        {
            newText.GetComponent<Animator>().Play("Appear at Lo");
            isTextLoActive = true;
        }
        else if (!isTextMidActive)
        {
            newText.GetComponent<Animator>().Play("Appear at Mid");
            isTextMidActive = true;
        }
        else if (!isTextHiActive)
        {
            newText.GetComponent<Animator>().Play("Appear at Hi");
            isTextHiActive = true;
        }
        else
        {
            pickedUpTexts.Remove(newText);
            newText.GetComponent<Animator>().Play("Hold Off Screen");
            //Debug.Log("Assigning text containing " + newText.GetComponent<PickedUpTextScript>().myItemAmount + " " + newText.GetComponent<PickedUpTextScript>().myItemName + " to offscreen");
            offScreenTexts.Add(newText);
            //CheckforDuplicatesOffscreen(newText);
        }
    }


    public void ManageOffScreenTexts()
    {
        if (offScreenTexts.Count > 0)
        {
            if (!isTextLoActive || !isTextMidActive || !isTextHiActive)
            {
                GameObject currentText = offScreenTexts[0];
                offScreenTexts.Remove(currentText);
                pickedUpTexts.Add(currentText);
                AssignTextToPosition(currentText);
            }
        }
    }
}
