using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Dialogue
{
    public int defaultTreeNumber = 0;           //The tree number the NPC defaults to. Can be changed
    [SerializeField] List<string> lines;
    [SerializeField] public List<DialogueTree> dialogueTrees;
    [SerializeField] public List<ChoiceLine> choiceLines;

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
    public int lettersPerSecond = 100;            //How fast is the text said?
    public AudioClip audio;                 //TODO: Audio that says the line
}

[System.Serializable]
public class ChoiceLine
{
    public string choiceText;               //Displays the text of the options the player has to choose
    public int choiceNumber;                //Which # is this choice? (First or second)
    public int treeNumberToSwitchTo;
    public int newDefaultTreeNumber;        //What the NPC will say as default after a choice a made
}