using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject aboveheaddialogueBox;
    public TextMeshPro aboveheaddialogue;

    public GameObject dialogueBox;
    //public GameObject dialogueCharacter;          //Sprite / Animation of character doing the talking
    Animator dialogueCharAnim;                      //Animator component of the dialogue character
   
    public TextMeshProUGUI continueText;            //Text that says press [Interactor] to continue
    public int lettersPerSecond;
    private Vector2 NPCPos;
    public Vector2 AboveHeadDialogueOffset;
    public Vector2 AboveHeadDialogueBoxOffset;
    
    
    public bool isTyping = false;
    public bool endOfConversation = false;

    Charcontrol charcontrol;

    [Header("Current Dialogue Variables")]
    public TextMeshProUGUI dialogueText;            //Dialogue that is actually shown on screen at any given point
    [HideInInspector] public int currentLine = 0;
    public Dialogue dialogue;
    public Dialogue line;
    public int currentTreeNumber;


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

        charcontrol = gamemanager.instance.Player.GetComponent<Charcontrol>();
    }

    // Start is called before the first frame update
    void Start()
    {
        dialogueBox.SetActive(false);
        continueText.enabled = false;

        aboveheaddialogueBox.SetActive(false);
        aboveheaddialogue.enabled = false;

        //Makes sure text renders infront everything else
        aboveheaddialogue.GetComponent<MeshRenderer>().sortingOrder = 69;

        //dialogueCharAnim = dialogueCharacter.GetComponent<Animator>();
    }


    public void Update()
    {
        if (isTyping)
        {
            continueText.enabled = false;
        }
        else
        {
            continueText.enabled = true;
        }
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

    public void ShowAboveHeadDialogue(Dialogue dialogue)
    {
        aboveheaddialogue.enabled = true;
        aboveheaddialogueBox.SetActive(true);

        aboveheaddialogue.transform.position = NPCPos + AboveHeadDialogueOffset;
        aboveheaddialogueBox.transform.position = NPCPos + AboveHeadDialogueBoxOffset;

        aboveheaddialogueBox.transform.localScale = new Vector2((dialogue.Lines[0].Length * 0.21f), 1f);

        aboveheaddialogue.text = dialogue.Lines[0].ToString();
    }

    //Takes dialogue and passes it to coroutine, also sets text box to be active
    public void ShowDialogue(Dialogue dialogue, int treeNumber)
    {
        Debug.Log("Showing Dialogue...");
        this.dialogue = dialogue;
        dialogueBox.SetActive(true);
        aboveheaddialogueBox.SetActive(false);
        aboveheaddialogue.enabled = false;
        currentTreeNumber = treeNumber;

        if (!isTyping)
        {
            //If at the end of dialogue lines, hide dialogue
            if (dialogue.dialogueTrees[treeNumber].dialogueLines.Count != 0 && dialogue.dialogueTrees[currentTreeNumber].dialogueLines[currentLine].endOfConversation)   
            {
                currentLine = 0;
                HideDialogue();
                endOfConversation = true;
            }
            //If the character somehow has no lines, say so
            else if (dialogue.dialogueTrees[treeNumber].dialogueLines.Count == 0)
            {
                Debug.LogWarning("Character has no dialogue lines!");
            }
            //Otherwise, passes one line at a time to coroutine
            else
            {
                StartCoroutine(TypeDialogue(dialogue.dialogueTrees[treeNumber].dialogueLines[currentLine].lineString));       
                endOfConversation = false;

                if (dialogue.dialogueTrees[treeNumber].dialogueLines[currentLine].hasChoice)
                {
                    GivePlayerChoices();
                }
            }
        }
    }

    public void ShowDialogueCharacterAnim(Animation animation)
    {
        
    }

    public void GivePlayerChoices()
    {
        Debug.Log("Here's your choices fucko!");
    }

    public void HideDialogue()
    {
        dialogueBox.SetActive(false);
        aboveheaddialogueBox.SetActive(false);
        aboveheaddialogue.enabled = false;
    }

    //Takes dialogue and shows it letter by letter
    public IEnumerator TypeDialogue(string line)
    {        
        isTyping = true;
        dialogueText.text = "";
        ++currentLine;

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
}

//TODO: TypeDialogue is currently very loose, meaning, the only way the next way the next line is called is by effectively "talking" with the NPC again,
//Instead of it being a built in functionality here. The only reason it works is that the dialogue box isnt hidden between lines, and the lines are incremented,
//Giving the illusion of the conversation continuing, when it reality its starting over, but at a different line. FIX THAT