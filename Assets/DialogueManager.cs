using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class DialogueManager : MonoBehaviour
{
    public float timeConstant;

    [Header("Variables to be Assgined")]
    public static DialogueManager instance;
    public GameObject aboveHeadTextPrefab;
    public Vector3 AboveHeadDialogueOffset;
    public List<GameObject> aboveHeadTexts;
    public GameObject aboveheaddialogueBox;
    public TextMeshPro aboveheaddialogue;
    public GameObject dialogueBox;
    public GameObject choicesBox;
    //public GameObject dialogueCharacter;          //Sprite / Animation of character doing the talking
    public TextMeshProUGUI continueText;            //Text that says press [Interactor] to continue

    [Header("Typing Variables")]
    public int defaultTypeSpeed = 10000;

    public bool playerInConversation;
    public bool playerInChoice;
    public bool isTyping = false;
    public bool isParsingTag = false;
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
    public bool lineHasAudio = false;

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
        /*        if (instance != null)
                {
                    Debug.LogWarning("More than one instance of Dialogue Manager found!");

                    DialogueManager old_dm = instance;
                    instance = this;
                    Destroy(old_dm.gameObject);
                }
                else
                {
                    instance = this;
                }*/
        instance = this;
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

        timeConstant = Time.deltaTime;
    }

    public void LateUpdate()
    {
        endOfConversation = false;

        //Finds closest NPC position from Charcontrol.
        if (charcontrol.closestNPC.gameObject != null)
        {
            GameObject closestNPC = charcontrol.closestNPC.gameObject;
            //NPCPos = new Vector2(closestNPC.transform.position.x, closestNPC.transform.position.y);
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

    public void ShowAboveHeadDialogue(AboveHeadDialogueLine npcahd, GameObject origin)            //TODO: fix this
    {
        if (aboveHeadTexts.Count == 0)
        {
            Vector3 spawnPos = origin.transform.position + AboveHeadDialogueOffset;
            GameObject aHDialogue = Instantiate(aboveHeadTextPrefab, spawnPos, Quaternion.identity);
            aboveHeadTexts.Add(aHDialogue);
            aHDialogue.GetComponent<AboveHeadDialogue>().myNPC = origin;
            aHDialogue.GetComponent<AboveHeadDialogue>().myText.text = npcahd.lineString;
        }
        else
        {
            foreach (var item in aboveHeadTexts)
            {
                if (!item.GetComponent<AboveHeadDialogue>().myNPC == origin)
                {
                    Vector3 spawnPos = origin.transform.position + AboveHeadDialogueOffset;
                    GameObject aHDialogue = Instantiate(aboveHeadTextPrefab, spawnPos, Quaternion.identity);
                    aboveHeadTexts.Add(aHDialogue);
                    aHDialogue.GetComponent<AboveHeadDialogue>().myNPC = origin;
                    aHDialogue.GetComponentInChildren<TextMeshPro>().text = dialogue.ToString();
                }
            }
        }
        
       /* aboveheaddialogue.enabled = true;
        aboveheaddialogueBox.SetActive(true);

        aboveheaddialogue.transform.position = NPCPos + AboveHeadDialogueOffset;
        aboveheaddialogueBox.transform.position = NPCPos + AboveHeadDialogueBoxOffset;*/

        //aboveheaddialogueBox.transform.localScale = new Vector2((dialogue.Lines[0].Length * 0.21f), 1f);

        //aboveheaddialogue.text = dialogue.Lines[0].ToString();
    }

    public void HideAboveHeadDialogue(GameObject origin)
    {
        if (aboveHeadTexts.Count != 0)
        {
            foreach (var item in aboveHeadTexts)
            {
                if (item.GetComponent<AboveHeadDialogue>().myNPC == origin)
                {
                    aboveHeadTexts.Remove(item);
                    Destroy(item);
                    return;
                }
            }
        }
    }

    //Takes dialogue and passes it to coroutine, also sets text box to be active. Is only called at the START of every conversation
    public void StartConversation(Dialogue dialogue, string treeId, GameObject dialogueSource)
    {
        this.dialogue = dialogue;
        this.dialogueSource = dialogueSource;

        var dialogueTree = dialogue.dialogueTrees.Find(i => i.DialogueTreeId == treeId);    //Finds the dialogue tree in the list by ID
        if (dialogueTree == null)
        {
            Debug.LogWarning("Cannot find correct Dialogue Tree via ID! Are your IDs correct?");
            return;
        }
        else
        {
            currentTreeId = dialogueTree.DialogueTreeId;
            this.currentDialogueTree = dialogueTree;
            this.currentDialogueLine = dialogueTree.dialogueLines[currentLineArray];
        }

        dialogueBox.SetActive(true);                //Enables all the shit
        choicesBox.SetActive(false);
        aboveheaddialogueBox.SetActive(false);
        aboveheaddialogue.enabled = false;

        DialogueLine firstLine = currentDialogueTree.dialogueLines[0];
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
                StopCoroutine(TypeDialogue(firstLine.lineString, firstLine.typeSpeed));
                StartCoroutine(TypeDialogue(firstLine.lineString, firstLine.typeSpeed));

                //Passes the audio to the below function to play audio clip
                endOfConversation = false;
                playerInConversation = true;
            }
        }

        if (currentDialogueLine.audio.Count > 0)
        {
            lineHasAudio = true;
            int randomNum = Random.Range(0, currentDialogueLine.audio.Count);
            AudioClip clip = currentDialogueLine.audio[randomNum];
            PlayDialogueLine(clip, currentDialogueLine.audioVol, dialogueSource.GetComponent<AudioSource>());
        }
        else
        {
            lineHasAudio = false;
            MakeConversationBeeps();
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
            StartCoroutine(TypeDialogue(currentDialogueLine.lineString, currentDialogueLine.typeSpeed));

            //Passes the audio to the below function to play audio clip
            if (currentDialogueLine.audio.Count > 0)
            {
                lineHasAudio = true;
                int randomNum = Random.Range(0, currentDialogueLine.audio.Count);
                AudioClip clip = currentDialogueLine.audio[randomNum];
                PlayDialogueLine(clip, currentDialogueLine.audioVol, dialogueSource.GetComponent<AudioSource>());
            }
            else
            {
                lineHasAudio = false;
                MakeConversationBeeps();
            }
            endOfConversation = false;
            playerInConversation = true;
        }
    }

    public void PlayDialogueLine(AudioClip audio, float audioVol, AudioSource audioSource)
    {
        Debug.Log($"Playing {audio}");
        if (audio != null)
        {
            audioSource.loop = false;
            audioSource.volume = audioVol;
            audioSource.pitch = 1;
            audioSource.clip = audio;
            audioSource.Play();
        }
    }

    public void MakeConversationBeeps() //This is a mess but I am lazy
    {
        Npcscript npcScript = dialogueSource.GetComponent<Npcscript>();
        StartCoroutine(npcScript.TalkCoolDown());
        npcScript.beenTalkedTo = true;
        npcScript.inConversation = true;

        npcScript.animator.SetBool("Talking", true);
        npcScript.animator.SetBool("Been Talked To", true);

        npcScript.audiosource.loop = true;
        npcScript.audiosource.volume = npcScript.talkingVolume;
        npcScript.audiosource.clip = npcScript.talkingClip;
        npcScript.audiosource.Play();
    }

    //Is called at the end of a dialogue. Ends everything and disables what it needs to so it's ready for the next conversation
    public void HideDialogue()
    {
        if (currentDialogueLine.hasChoice)
        {
            //Debug.LogWarning("Can't exit out of a line that has a choice!");
            dialogueBox.SetActive(false);
            choicesBox.SetActive(false);
            aboveheaddialogueBox.SetActive(false);
            aboveheaddialogue.enabled = false;
            //endOfConversation = true;
            //currentLineArray = 0;
            //currentLine = 1;
            playerInConversation = false;
            isTyping = false;

            Npcscript npcScript = dialogueSource.GetComponent<Npcscript>();
            npcScript.audiosource.Stop();

            StopAllCoroutines();
        }
        else if (dialogueBox.activeInHierarchy)
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

            Npcscript npcScript = dialogueSource.GetComponent<Npcscript>();
            npcScript.audiosource.Stop();

            StopAllCoroutines();
        }
    }
    //Takes dialogue and shows it letter by letter
    public IEnumerator TypeDialogue(string line, int lettersPerSecond)
    {
        isTyping = true;
        dialogueText.text = "";
        int currentLetterIndex = 0;

        if (lettersPerSecond == 0)
        {
            lettersPerSecond = defaultTypeSpeed; //Only changes typeSpeed if it's changed in the Dialogue Scriptable Object
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

        //TODO: Finish Tag Parser
        foreach (var letter in line.ToCharArray())
        {
            if (letter.ToString() == "<")   //If an opening tag is found, send it to the tag parser (Currently broken lol)
            {
                int openTagIndex = dialogueText.text.Length;
                Debug.Log($"In tag at {openTagIndex}");
                isParsingTag = true;
                ParseTags(line.ToCharArray());
            }
            else if(letter.ToString() == ">")
            {
                Debug.Log($"Exiting tag at {dialogueText.text.Length}");
                isParsingTag = false;
            }
            else
            {
                dialogueText.text += line[currentLetterIndex];  //Actually types the line, letter by letter
                currentLetterIndex++;
                //dialogueText.text += letter;
            }

            //Allows player to interrupt if they press interact key during typing. Prints whole line and stops letter by letter typing
            if (isTyping && Input.GetButtonDown("Interact"))
            {
                dialogueText.text = "";
                dialogueText.text = line.ToString();
                yield return new WaitForSecondsRealtime(0.2f);
                isTyping = false;
                yield break;
            }

            float yieldTime = (1 / (lettersPerSecond * Time.deltaTime))/10;
            //Debug.Log($"Yielding for {yieldTime} seconds");
            //yield return new WaitForSecondsRealtime(1 / typeSpeed);
            yield return new WaitForSeconds(yieldTime);
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

        playerInChoice = true;
        isTyping = true;
        dialogueOnChoiceText.text = "";

        yield return new WaitForSeconds(0.1f);

        if (lettersPerSecond == 0)
        {
            lettersPerSecond = defaultTypeSpeed;
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
        StartConversation(this.dialogue, choiceOne.treeIdToSwitchTo, dialogueSource);
        /*        if (currentDialogueLine.audio != null)
                {
                    lineHasAudio = true;
                    PlayDialogueLine(currentDialogueLine.audio, currentDialogueLine.audioVol, dialogueSource.GetComponent<AudioSource>());
                }
                else
                {
                    lineHasAudio = false;
                    MakeConversationBeeps();
                }
        */
        if (choiceOne.canTriggerQuest)
        {
            //QuestGiver questScript = dialogueSource.GetComponent<QuestGiver>();
            //Quest questToGive = questScript.myQuest;
            Quest questToGive = choiceOne.myQuest;
            questsToGive.Add(questToGive);
        }
        playerInChoice = false;
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
        StartConversation(this.dialogue, choiceTwo.treeIdToSwitchTo, dialogueSource);
        /*        if (currentDialogueLine.audio != null)
                {
                    lineHasAudio = true;
                    PlayDialogueLine(currentDialogueLine.audio, currentDialogueLine.audioVol, dialogueSource.GetComponent<AudioSource>());
                }
                else
                {
                    lineHasAudio = false;
                    MakeConversationBeeps();
                }*/

        if (choiceTwo.canTriggerQuest)
        {
            //QuestGiver questScript = dialogueSource.GetComponent<QuestGiver>();
            //Quest questToGive = questScript.myQuest;
            Quest questToGive = choiceTwo.myQuest;
            questsToGive.Add(questToGive);
        }
        playerInChoice = false;
    }

    public void ShowDialogueCharacterAnim(Animation animation)
    {

    }

    public void ParseTags(char[] lineArray) //Man fuck this
    {
        int openTagIndex = dialogueText.text.Length;
        Debug.Log(openTagIndex);

        char nextChar = lineArray[openTagIndex + 1];
        char nextChar2 = lineArray[openTagIndex + 2];
        char nextChar3 = lineArray[openTagIndex + 3];

        if (nextChar == 'l' && nextChar2 == 'p' && nextChar3 == 's')
        {
            //Tag for Letters per second
            Debug.Log("$Found a tag for a change in LPS");
            foreach (char letter in lineArray)
            {
            }
        }
    }
}
