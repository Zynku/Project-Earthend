using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;           //Dialogue that is actually shown on screen at any given point
    public int lettersPerSecond;
    [HideInInspector] public int currentLine = 0;
    public Dialogue dialogue;
    public Dialogue line;
    public bool isTyping = false;

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
    }


    //Takes dialogue and passes it to coroutine, also sets text box to be active
    public void ShowDialogue(Dialogue dialogue)
    {
        this.dialogue = dialogue;
        dialogueBox.SetActive(true);

        if (!isTyping)
        {
            if (dialogue.Lines.Count != 0 && currentLine == dialogue.Lines.Count)
            {
                currentLine = 0;
                HideDialogue();
            }
            else
            {
                StartCoroutine(TypeDialogue(dialogue.Lines[currentLine]));
            }
        }
    }

    //Shows next line and shit
    public void Update()
    {
        
    }

    public void HideDialogue()
    {
        dialogueBox.SetActive(false);
    }

    //Takes dialogue and shows it letter by letter
    public IEnumerator TypeDialogue(string line)
    {
        /*isTyping = true;
        dialogueText.text = line;
        ++currentLine;
        yield return new WaitForSeconds(1);
        isTyping = false;*/
        
        isTyping = true;
        dialogueText.text = "";
        ++currentLine;
        yield return new WaitForSeconds(0.2f);

        

        foreach (var letter in line.ToCharArray())
        {
            dialogueText.text += letter;

            /*if (isTyping && Input.GetAxisRaw("Interact") != 0)
            {
                dialogueText.text = "";
                dialogueText.text = line.ToString();
                yield return new WaitForSeconds(0.4f);
                isTyping = false;
                yield break;
            }*/
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }
}
