using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MyBox;

public class AboveHeadDialogue : MonoBehaviour  //This is the script attached to the AHD game object that is instantiated from DialogueManager
{
    [ReadOnly] public GameObject myNPC;
    public TextMeshPro myText;

    private void Start()
    {
        myNPC.GetComponent<Npcscript>().showingAHD = true;
        //Debug.Log($"Showing an AHD for {myNPC}");
    }

    private void OnDisable()
    {
        myNPC.GetComponent<Npcscript>().showingAHD = false;
        //Debug.Log($"Hiding an AHD for {myNPC}");
    }
}
