using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using MyBox;
using JetBrains.Annotations;
using UnityEditor.PackageManager.Requests;

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
    public string DialogueTreeName;
    public string DialogueTreeId;
    public string dialogueSpeaker;          //The main speaker in the conversation. Note that this can be overidden by the lineOwner variable.
    public bool pauseGameOnOpen;            
    [SerializeField] public List<DialogueLine> dialogueLines;
}

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 10)]
    public string lineString;               //Actual string of words to be said
    [Separator("Dynamic Variables")]
    public bool canChangeAboveHeadDialogue;
    [ConditionalField(nameof(canChangeAboveHeadDialogue))]
    public int AHDialogueIndexToSwitchTo;
    public bool hasChoice;
    [ConditionalField(nameof(hasChoice))]
    public string choiceTreeID;             //The ID of the choice tree it'll show if it has a choice
    public bool canChangeDefaultTreeId;     //Can reading this line change default tree ID to another?
    [ConditionalField(nameof(canChangeDefaultTreeId))]
    public string treeIdToSwitchTo;         //If it can, which tree?
    public bool canTriggerQuest;
    [ConditionalField(nameof(canTriggerQuest))]
    public Quest myQuest;                   //Quest that can be triggered by reading this line

    [Header("Default Variables")]
    public string lineOwner;                //Who said the line?
    public int typeSpeed = 0;               //How fast is the text said?
    public List<AudioClip> audio;           //List of Audio that says the line
    [Range(0f, 1f)]
    public float audioVol = 1;
}
[System.Serializable]
public class CharacterDialogueSprite
{
    public Sprite characterSprite;
    public string spriteOwner;
    public string spriteMood = "default";
    public bool flipSpriteX;
}

[System.Serializable]
public class CharSpriteBox  //Contains a reference to the UI Elements on screen, as well as who is speaking and other sprite references.
{
    public VisualElement characterSpriteBox;
    public GameObject speaker;
    public string speakerName;
    public int spriteBoxIndex;
    public bool active;
    public string name;

    public CharSpriteBox(VisualElement characterSpriteBox, GameObject speaker, int spriteBoxIndex, string speakerName, bool active)
    {
        this.characterSpriteBox = characterSpriteBox;
        this.speaker = speaker;
        this.speakerName = speakerName;
        this.spriteBoxIndex = spriteBoxIndex;
        this.active = active;
    }

    public void Reset()
    {
        speaker = null;
        speakerName = null;
        active = false;
    }
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
    public string treeIdToSwitchTo;         //The next line after this choice will be from this tree
    public string newDefaultTreeId;         //What the NPC will say as default after a choice a made, and the conversation is exited and reopened
    public bool canTriggerQuest;
    [ConditionalField(nameof(canTriggerQuest))] public Quest myQuest;                   //Quest that can be triggered by reading this line
}