using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Dialogue
{
    [SerializeField] List<string> lines;
    [SerializeField] public List<DialogueTree> dialogueTrees;

    public List<string> Lines
    {
        get { return lines; }
    }
}

[System.Serializable]
public class DialogueTree
{
    public string treeName;
    public int treeNumber;
    public bool pauseGameOnOpen;            //TODO: Pause Game if this is true
    [SerializeField] public List<DialogueLine> dialogueLines;
}

[System.Serializable]
public class DialogueLine
{
    public string lineString;               //Actual string of words to be said
    public bool endOfConversation;          //Does pressing Interact on this line close the dialogue popup?
    public bool hasChoice;
    public bool canTriggerQuest;
    public string lineOwner;                //Who said the line?
    public AudioClip audio;                 //TODO: Audio that says the line
}

[System.Serializable]
public class ChoiceLine
{
    public string choiceText;               //Displays the text of the options the player has to choose
    public int treeNumberToSwitchTo;
}