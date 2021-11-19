using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
[CreateAssetMenu(fileName = "New Character Dialogue", menuName = "Dialogue/New Character Dialogue")]
public class Dialogue : ScriptableObject
{
    public string defaultTreeId = "1";
    [SerializeField] public List<DialogueTree> dialogueTrees;
    [SerializeField] public List<ChoiceTree> choiceTrees;
}

[System.Serializable]
public class DialogueTree
{
    public string treeName;
    public string treeID;
    public bool pauseGameOnOpen;            //TODO: Pause Game if this is true
    [SerializeField] public List<DialogueLine> dialogueLines;
}

[System.Serializable]
public class DialogueLine
{
    public string lineString;               //Actual string of words to be said
    public bool hasChoice;
    public string choiceTreeID;                 //The ID of the choice tree it'll show if it has a choice
    public bool canChangeDefaultTreeId;     //Can reading this line change default tree ID to another?
    public string treeIdToSwitchTo;         //If it can, which tree?
    public bool canTriggerQuest;
    public Quest myQuest;                   //Quest that can be triggered by reading this line
    public string lineOwner;                //Who said the line?
    public int lettersPerSecond = 100;      //How fast is the text said?
    public AudioClip audio;                 //TODO: Audio that says the line
}

[System.Serializable]
public class ChoiceTree
{
    public string choiceTreeName;
    public string choiceTreeID;
    public List<ChoiceLine> choices;
}

[System.Serializable]
public class ChoiceLine
{
    public string choiceText;               //Displays the text of the options the player has to choose
    public string choiceID;                 //ID that identifies this choice
    public string treeIdToSwitchTo;
    public string newDefaultTreeId;         //What the NPC will say as default after a choice a made, and the conversation is exited and reopened
    public bool canTriggerQuest;
    public Quest myQuest;                   //Quest that can be triggered by reading this line
}