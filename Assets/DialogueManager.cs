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

    public bool playerInConversation;
    public bool isTyping = false;
    public bool endOfConversation = false;

    [Header("Current Dialogue Variables")]
    public TextMeshProUGUI dialogueText;            //TextMesh component of the text that is shown on screen
    public GameObject dialogueSource;
    public int currentTreeNumber = 0;
    public int currentLine = 0;                     //What number line are we on?(Used for visual purposes)
    public int currentLineArray = 0;                //What number line are we on?(used for array references)                
    public Dialogue dialogue;
    public Dialogue line;

    [Header("Dialogue Choice Variables")]
    public TextMeshProUGUI dialogueOnChoiceText;    //TextMesh component of the text that is shown on screen before a choice
    public TextMeshProUGUI choiceOneText;
    public TextMeshProUGUI choiceTwoText;

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


        charcontrol = gamemanager.instance.Player.GetComponent<Charcontrol>();
        questManager = gamemanager.instance.questManager;
    }


    public void Update()
    {
        if (isTyping) { continueText.enabled = false; }
        else { continueText.enabled = true; }
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

    public void ShowAboveHeadDialogue(Dialogue dialogue)            //TODO: fix this
    {
        aboveheaddialogue.enabled = true;
        aboveheaddialogueBox.SetActive(true);

        aboveheaddialogue.transform.position = NPCPos + AboveHeadDialogueOffset;
        aboveheaddialogueBox.transform.position = NPCPos + AboveHeadDialogueBoxOffset;

        aboveheaddialogueBox.transform.localScale = new Vector2((dialogue.Lines[0].Length * 0.21f), 1f);

        aboveheaddialogue.text = dialogue.Lines[0].ToString();
    }

    //Takes dialogue and passes it to coroutine, also sets text box to be active
    public void ShowDialogue(Dialogue dialogue, int treeNumber, GameObject dialogueSource)
    {
        Debug.Log("Starting dialogue for the first time");
        this.dialogue = dialogue;
        this.dialogueSource = dialogueSource;
        dialogueBox.SetActive(true);
        choicesBox.SetActive(false);
        aboveheaddialogueBox.SetActive(false);
        aboveheaddialogue.enabled = false;
        currentTreeNumber = treeNumber;
        

        if (!isTyping)
        {
            //If the character somehow has no lines, say so
            if (dialogue.dialogueTrees[treeNumber].dialogueLines.Count == 0)
            {
                Debug.LogWarning("Character has no dialogue lines!");
            }
            //Otherwise, passes one line at a time to coroutine
            else
            {
                currentLineArray = 0;
                currentLine = 1;
                StartCoroutine(TypeDialogue(this.dialogue.dialogueTrees[treeNumber].dialogueLines[0].lineString, this.dialogue.dialogueTrees[treeNumber].dialogueLines[0].lettersPerSecond));
/*              ++currentLine;
                ++currentLineArray;*/
                endOfConversation = false;
                playerInConversation = true;
            }
        }
    }

    public void ShowDialogueCharacterAnim(Animation animation)
    {

    }


    public void HideDialogue()
    {
        dialogueBox.SetActive(false);
        choicesBox.SetActive(false);
        aboveheaddialogueBox.SetActive(false);
        aboveheaddialogue.enabled = false;
        endOfConversation = true;
        playerInConversation = false;
        isTyping = false;
    }

    public void ContinueConversation()
    {
        if (this.dialogue.dialogueTrees[currentTreeNumber].dialogueLines[currentLineArray].endOfConversation)
        {
            Debug.Log("Line #" + currentLine + " is the end of the dialogue Tree.");
            HideDialogue();
        }
        ++currentLine;
        ++currentLineArray;
        if (currentLine > this.dialogue.dialogueTrees[currentTreeNumber].dialogueLines.Count)  //The last line in the dialogueLines list, end of conversation
        {
            HideDialogue();
        }
        else
        {
            StartCoroutine(TypeDialogue(this.dialogue.dialogueTrees[currentTreeNumber].dialogueLines[currentLineArray].lineString, this.dialogue.dialogueTrees[currentTreeNumber].dialogueLines[currentLineArray].lettersPerSecond));
        }
    }

    //Takes dialogue and shows it letter by letter
    public IEnumerator TypeDialogue(string line, int lettersPerSecond)
    {        
        isTyping = true;
        dialogueText.text = "";
        if (this.dialogue.dialogueTrees[currentTreeNumber].dialogueLines[currentLineArray].hasChoice)
        {
            Debug.Log("Line #" + currentLine + " has a choice.");
            StartCoroutine(GivePlayerChoices(line, lettersPerSecond));
            yield break;
        }

        yield return new WaitForSeconds(0.1f);


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

    public IEnumerator GivePlayerChoices(string line, int lettersPerSecond)
    {
        if (this.dialogue.choiceLines.Count == 0)
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


        foreach (var letter in line.ToCharArray())
        {
            dialogueOnChoiceText.text += letter;
            yield return new WaitForSecondsRealtime(1f / lettersPerSecond);
        }
        choiceOneText.text = this.dialogue.choiceLines[0].choiceText;
        choiceTwoText.text = this.dialogue.choiceLines[1].choiceText;
        isTyping = false;
    }

    public void ChoiceOneSelected()
    {
        currentTreeNumber = this.dialogue.choiceLines[0].treeNumberToSwitchTo;
        dialogue.defaultTreeNumber = dialogue.choiceLines[0].newDefaultTreeNumber;
        currentLineArray = 0;
        currentLine = 1;
        ShowDialogue(this.dialogue, currentTreeNumber, dialogueSource);
        
    }

    public void ChoiceTwoSelected()
    {
        currentTreeNumber = this.dialogue.choiceLines[1].treeNumberToSwitchTo;
        dialogue.defaultTreeNumber = dialogue.choiceLines[1].newDefaultTreeNumber;
        currentLineArray = 0;
        currentLine = 1;
        ShowDialogue(this.dialogue, currentTreeNumber, dialogueSource);
    }

}
