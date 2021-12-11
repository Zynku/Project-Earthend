using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("Variables to be Assgined")]
    public static DialogueManager Instance;
    public GameObject aboveheaddialogueBox;
    public TextMeshPro aboveheaddialogue;
    public GameObject dialogueBox;
    public GameObject choicesBox;
    //public GameObject dialogueCharacter;          //Sprite / Animation of character doing the talking
    public TextMeshProUGUI continueText;            //Text that says press [Interactor] to continue

    private Vector2 NPCPos;
    public Vector2 AboveHeadDialogueOffset;
    public Vector2 AboveHeadDialogueBoxOffset;

    [Header("Typing Variables")]
    public int defaultLettersPerSecond = 100;

    public bool playerInConversation;
    public bool isTyping = false;
    public bool endOfConversation = false;

    [Header("Current Dialogue Variables")]
    public TextMeshProUGUI dialogueText;            //TextMesh component of the text that is shown on screen
    public GameObject dialogueSource;
    public int currentTreeNumber = 0;
    public string currentTreeId;
    public int currentLine = 0;                     //What number line are we on?(Used for visual purposes)
    public int currentLineArray = 0;                //What number line are we on?(used for array references)                
    public Dialogue dialogue;
    public DialogueTree currentDialogueTree;
    public DialogueLine currentDialogueLine;

    [Header("Dialogue Choice Variables")]
    public TextMeshProUGUI dialogueOnChoiceText;    //TextMesh component of the text that is shown on screen before a choice
    public TextMeshProUGUI choiceOneText;
    public TextMeshProUGUI choiceTwoText;

    [Header("Dialogue Quest Variables")]
    public List<Quest> questsToGive;                //Quests that will be given to the player after this conversation is done

    Animator dialogueCharAnim;                      //Animator component of the dialogue character
    Charcontrol charcontrol;
    QuestManager questManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        dialogueBox.SetActive(false);
        choicesBox.SetActive(false);
        continueText.enabled = false;
        currentLineArray = 0;
        currentLine = 0;

        aboveheaddialogueBox.SetActive(false);
        aboveheaddialogue.enabled = false;

        //Makes sure text renders infront everything else
        aboveheaddialogue.GetComponent<MeshRenderer>().sortingOrder = 69;

        //dialogueCharAnim = dialogueCharacter.GetComponent<Animator>();


        charcontrol = GameManager.instance.Player.GetComponent<Charcontrol>();
        questManager = GameManager.instance.questManager;
    }


    public void Update()
    {
        if (isTyping) { continueText.enabled = false; }
        else { continueText.enabled = true; }

        ManageQuestsToGive();
    }

    public void LateUpdate()
    {
        endOfConversation = false;

        //Finds closest NPC position from Charcontrol.
        if (charcontrol.closestNPC.gameObject != null)
        {
            GameObject closestNPC = charcontrol.closestNPC.gameObject;
            NPCPos = new Vector2(closestNPC.transform.position.x, closestNPC.transform.position.y);
        }
    }

    public void ManageQuestsToGive()
    {
        if (!playerInConversation && questsToGive.Count > 0)
        {
            Debug.Log("There's a quest to give, let's do it");
            //QuestGiver questScript = dialogueSource.GetComponent<QuestGiver>();
            //StartCoroutine(questScript.AcceptQuest(questsToGive[0]));
            Quest questToGive = questsToGive[0];
            questsToGive.Remove(questToGive);
            questManager.SetupNewQuest(questToGive);
        }
    }

    public void ShowAboveHeadDialogue(Dialogue dialogue)            //TODO: fix this
    {
        aboveheaddialogue.enabled = true;
        aboveheaddialogueBox.SetActive(true);

        aboveheaddialogue.transform.position = NPCPos + AboveHeadDialogueOffset;
        aboveheaddialogueBox.transform.position = NPCPos + AboveHeadDialogueBoxOffset;

        //aboveheaddialogueBox.transform.localScale = new Vector2((dialogue.Lines[0].Length * 0.21f), 1f);

        //aboveheaddialogue.text = dialogue.Lines[0].ToString();
    }

    //Takes dialogue and passes it to coroutine, also sets text box to be active. Is only called at the START of every conversation
    public void ShowDialogue(Dialogue dialogue, string treeId, GameObject dialogueSource)
    {
        this.dialogue = dialogue;
        this.dialogueSource = dialogueSource;

        var dialogueTree = dialogue.dialogueTrees.Find(i => i.treeID == treeId);    //Finds the dialogue tree in the list by ID
        if (dialogueTree == null)
        {
            Debug.LogWarning("Cannot find correct Dialogue Tree via ID! Are your IDs correct?");
            return;
        }
        else
        {
            currentTreeId = dialogueTree.treeID;
            this.currentDialogueTree = dialogueTree;
            this.currentDialogueLine = dialogueTree.dialogueLines[currentLineArray];
        }

        dialogueBox.SetActive(true);                //Enables all the shit
        choicesBox.SetActive(false);
        aboveheaddialogueBox.SetActive(false);
        aboveheaddialogue.enabled = false;
        

        if (!isTyping)
        {
            //If the character somehow has no lines, say so
            if (currentDialogueTree.dialogueLines.Count == 0)
            {
                Debug.LogWarning("Character has no dialogue lines!");
            }
            //Otherwise, passes one line at a time to coroutine
            else
            {
                currentLineArray = 0;
                currentLine = 1;

                //Passes the first line to the Coroutine so its starts typing
                StartCoroutine(TypeDialogue(currentDialogueTree.dialogueLines[0].lineString, this.currentDialogueTree.dialogueLines[0].lettersPerSecond));
                endOfConversation = false;
                playerInConversation = true;
            }
        }
    }

    public void ShowDialogueCharacterAnim(Animation animation)
    {

    }

    //Is called at the end of a dialogue. Ends everything and disables what it needs to so it's ready for the next conversation
    public void HideDialogue()
    {
        if (currentDialogueLine.hasChoice)
        {
            Debug.LogWarning("Can't exit out of a line that has a choice!");
        }
        else
        {
            dialogueBox.SetActive(false);
            choicesBox.SetActive(false);
            aboveheaddialogueBox.SetActive(false);
            aboveheaddialogue.enabled = false;
            endOfConversation = true;
            currentLineArray = 0;
            currentLine = 1;
            playerInConversation = false;
            isTyping = false;
        }
    }

    //Is called while inside a conversation, and pressing [Interact]. Pulls the next line for the Coroutine to use
    public void ContinueConversation()
    {
        if (currentDialogueLine.hasChoice)
        {
            Debug.LogWarning("Can't exit out of a line that has a choice!");
            return;
        }

        if (currentLine == currentDialogueTree.dialogueLines.Count)  //The last line in the dialogueLines list, end of conversation
        {
            HideDialogue();
        }
        else
        {
            ++currentLine;
            ++currentLineArray;

            DialogueTree dialogueTree = this.currentDialogueTree;
            this.currentDialogueLine = dialogueTree.dialogueLines[currentLineArray];
            StartCoroutine(TypeDialogue(currentDialogueTree.dialogueLines[currentLineArray].lineString, currentDialogueTree.dialogueLines[currentLineArray].lettersPerSecond));
        }
    }

    //Takes dialogue and shows it letter by letter
    public IEnumerator TypeDialogue(string line, int lettersPerSecond)
    {        
        isTyping = true;
        dialogueText.text = "";

        if(lettersPerSecond == 0)
        {
            lettersPerSecond = defaultLettersPerSecond; //Only changes lettersPerSecond if it's changed in the Dialogue Scriptable Object
        }

        //If the line has choices, switch to the choices layout so player can choose
        if (currentDialogueTree.dialogueLines[currentLineArray].hasChoice)
        {
            StartCoroutine(GivePlayerChoices(line, lettersPerSecond));  
            yield break;
        }

        if (currentDialogueTree.dialogueLines[currentLineArray].canTriggerQuest)
        {
            QuestGiver questScript = dialogueSource.GetComponent<QuestGiver>();
            questsToGive.Add(questScript.myQuest);
        }

        //If the choice can change the default tree to a different one, let it, and set the new tree ID
        if (currentDialogueTree.dialogueLines[currentLineArray].canChangeDefaultTreeId)
        {
            dialogue.defaultTreeId = currentDialogueTree.dialogueLines[currentLineArray].treeIdToSwitchTo;
        }

        yield return new WaitForSeconds(0.2f);

        //Actually types the line, letter by letter
        foreach (var letter in line.ToCharArray())
        {
            dialogueText.text += letter;

            //Allows player to interrupt if they press interact key during typing. Prints whole line and stops letter by letter typing
            if (isTyping && Input.GetButtonDown("Interact"))
            {
                dialogueText.text = "";
                dialogueText.text = line.ToString();
                yield return new WaitForSecondsRealtime(0.2f);
                isTyping = false;
                yield break;
            }

            yield return new WaitForSecondsRealtime(1f / lettersPerSecond);
        }
        isTyping = false;
    }

    //Choices are given here. The regular dialogue box is disabled and the choices box (which is slightly different) is shown in its place
    //The player doesn't notice because they have the same dimensions
    public IEnumerator GivePlayerChoices(string line, int lettersPerSecond)
    {
        DialogueLine currentDialogueLine = this.dialogue.dialogueTrees[currentTreeNumber].dialogueLines[currentLineArray];
        ChoiceTree choiceTree = dialogue.choiceTrees.Find(i => i.choiceTreeID == currentDialogueLine.choiceTreeID);
        ChoiceLine choiceOne = choiceTree.choices[0];
        ChoiceLine choiceTwo = choiceTree.choices[1];

        if (choiceTree.choices.Count == 0)
        {
            Debug.LogWarning("NPC Dialogue has no lines for its choice options!");
            HideDialogue();
            yield break;
        }

        dialogueBox.SetActive(false);
        choicesBox.SetActive(true);
        choiceOneText.text = "";
        choiceTwoText.text = "";

        isTyping = true;
        dialogueOnChoiceText.text = "";

        yield return new WaitForSeconds(0.1f);

        if (lettersPerSecond == 0)
        {
            lettersPerSecond = defaultLettersPerSecond;
        }

        foreach (var letter in line.ToCharArray())
        {
            dialogueOnChoiceText.text += letter;
            yield return new WaitForSecondsRealtime(1f / lettersPerSecond);
        }

        choiceOneText.text = choiceOne.choiceText; //Finds which choice to display by ARRAY REFERENCE (not by choice #)
        choiceTwoText.text = choiceTwo.choiceText;
        isTyping = false;
    }

    public void ChoiceOneSelected()
    {
        DialogueLine currentDialogueLine = this.dialogue.dialogueTrees[currentTreeNumber].dialogueLines[currentLineArray];
        ChoiceTree choiceTree = dialogue.choiceTrees.Find(i => i.choiceTreeID == currentDialogueLine.choiceTreeID);
        ChoiceLine choiceOne = choiceTree.choices[0];
        ChoiceLine choiceTwo = choiceTree.choices[1];

        currentTreeId = choiceOne.treeIdToSwitchTo;             //Switches tree ID so the character can "respond"
        dialogue.defaultTreeId = choiceOne.newDefaultTreeId;    //Switches default tree ID so character says different things based on what you chose
        currentLineArray = 0;
        currentLine = 1;
        ShowDialogue(this.dialogue, choiceOne.treeIdToSwitchTo, dialogueSource);
        dialogueSource.GetComponent<Npcscript>().MakeConversationBeeps();

        if (choiceOne.canTriggerQuest)
        {
            //QuestGiver questScript = dialogueSource.GetComponent<QuestGiver>();
            //Quest questToGive = questScript.myQuest;
            Quest questToGive = choiceOne.myQuest;
            questsToGive.Add(questToGive);
        }
    }

    public void ChoiceTwoSelected()
    {
        DialogueLine currentDialogueLine = this.dialogue.dialogueTrees[currentTreeNumber].dialogueLines[currentLineArray];
        ChoiceTree choiceTree = dialogue.choiceTrees.Find(i => i.choiceTreeID == currentDialogueLine.choiceTreeID);
        ChoiceLine choiceOne = choiceTree.choices[0];
        ChoiceLine choiceTwo = choiceTree.choices[1];

        currentTreeId = choiceTwo.treeIdToSwitchTo;
        dialogue.defaultTreeId = choiceTwo.newDefaultTreeId;
        currentLineArray = 0;
        currentLine = 1;
        ShowDialogue(this.dialogue, choiceTwo.treeIdToSwitchTo, dialogueSource);
        dialogueSource.GetComponent<Npcscript>().MakeConversationBeeps();

        if (choiceTwo.canTriggerQuest)
        {
            //QuestGiver questScript = dialogueSource.GetComponent<QuestGiver>();
            //Quest questToGive = questScript.myQuest;
            Quest questToGive = choiceTwo.myQuest;
            questsToGive.Add(questToGive);
        }
    }
}
