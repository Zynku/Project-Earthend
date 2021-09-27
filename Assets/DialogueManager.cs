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
    //public GameObject dialogueCharacter;           //Sprite / Animation of character doing the talking
    Animator dialogueCharAnim;                     //Animator component of the dialogue character
    public TextMeshProUGUI dialogueText;           //Dialogue that is actually shown on screen at any given point
    public TextMeshProUGUI continueText;            //Text that says press [Interactor] to continue
    public int lettersPerSecond;
    private Vector2 NPCPos;
    public Vector2 AboveHeadDialogueOffset;
    public Vector2 AboveHeadDialogueBoxOffset;
    [HideInInspector] public int currentLine = 0;
    public Dialogue dialogue;
    public Dialogue line;
    public bool isTyping = false;
    public bool endOfConversation = false;

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
        if (Charcontrol.closestNPC.gameObject != null)
        {
            GameObject closestNPC = Charcontrol.closestNPC.gameObject;
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
    public void ShowDialogue(Dialogue dialogue)
    {
        this.dialogue = dialogue;
        dialogueBox.SetActive(true);
        aboveheaddialogueBox.SetActive(false);
        aboveheaddialogue.enabled = false;

        if (!isTyping)
        {
            if (dialogue.Lines.Count != 0 && currentLine == dialogue.Lines.Count)
            {
                currentLine = 0;
                HideDialogue();
                endOfConversation = true;
            }
            else
            {
                StartCoroutine(TypeDialogue(dialogue.Lines[currentLine]));
                endOfConversation = false;
            }
        }
    }

    public void ShowDialogueCharacterAnim(Animation animation)
    {
        
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
        //Code to show line instantly, for debugging purposes
        /*isTyping = true;
        dialogueText.text = line;
        ++currentLine;
        yield return new WaitForSeconds(1);
        isTyping = false;*/
        
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
