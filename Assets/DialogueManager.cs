using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class DialogueManager : MonoBehaviour
{
    [Header("Variables to be Assgined")]
    public static DialogueManager instance;
    public GameObject aboveHeadTextPrefab;
    public Vector3 AboveHeadDialogueOffset;
    public float AHDFadeOutTime;
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
    }

    public void LateUpdate()
    {
        if (GameManager.instance.Player != null)
        {
            endOfConversation = false;

            //Finds closest NPC position from Charcontrol.
            if (charcontrol.closestNPC.gameObject != null)
            {
                GameObject closestNPC = charcontrol.closestNPC.gameObject;
                //NPCPos = new Vector2(closestNPC.transform.position.x, closestNPC.transform.position.y);
            }
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

    public void ShowAboveHeadDialogue(AboveHeadDialogueLine npcahd, GameObject origin)       
    {
        if (aboveHeadTexts.Count == 0)  //If there are no above head texts, we show a new one corresponding to this NPC
        {
            Vector3 spawnPos = origin.transform.position + AboveHeadDialogueOffset;
            GameObject aHDialogue = Instantiate(aboveHeadTextPrefab, spawnPos, Quaternion.identity);
            aboveHeadTexts.Add(aHDialogue);
            aHDialogue.GetComponent<AboveHeadDialogue>().myNPC = origin;
            aHDialogue.GetComponent<AboveHeadDialogue>().myText.text = npcahd.lineString;

            Color transpcolor = new Color32(255, 255, 255, 0);
            Color opaquecolor = new Color32(255, 255, 255, 255);

            aHDialogue.GetComponentInChildren<TextMeshPro>().color = transpcolor;
            StartCoroutine(FadeAHD(transpcolor, opaquecolor, AHDFadeOutTime, aHDialogue));
        }
        else            //If there are above head texts, we check to make sure the one we're trying to show isnt already shown (by checking its NPC). If not, we show it.
        {
            foreach (var item in aboveHeadTexts)
            {
                if (item.GetComponent<AboveHeadDialogue>().myNPC != origin)
                {
                    Vector3 spawnPos = origin.transform.position + AboveHeadDialogueOffset;
                    GameObject aHDialogue = Instantiate(aboveHeadTextPrefab, spawnPos, Quaternion.identity);
                    aboveHeadTexts.Add(aHDialogue);
                    aHDialogue.GetComponent<AboveHeadDialogue>().myNPC = origin;

                    aHDialogue.GetComponent<AboveHeadDialogue>().myText.text = npcahd.lineString;

                    Color transpcolor = new Color32(255, 255, 255, 0);
                    Color opaquecolor = new Color32(255, 255, 255, 255);

                    aHDialogue.GetComponentInChildren<TextMeshPro>().color = transpcolor;
                    StartCoroutine(FadeAHD(transpcolor, opaquecolor, AHDFadeOutTime, aHDialogue));
                }
            }
        }
    }

    public IEnumerator HideAboveHeadDialogue(GameObject origin)
    {
        origin.GetComponent<Npcscript>().showingAHD = false;
        if (aboveHeadTexts.Count != 0)
        {
            foreach (var item in aboveHeadTexts)
            {
                if (item.GetComponent<AboveHeadDialogue>().myNPC == origin)
                {
                    Color transpcolor = new Color32(255, 255, 255, 0);
                    Color opaquecolor = new Color32(255, 255, 255, 255);

                    StartCoroutine(FadeAHD(opaquecolor, transpcolor, AHDFadeOutTime, item));
                    yield return new WaitForSeconds(0.7f);
                    aboveHeadTexts.Remove(item);
                    Destroy(item);
                    break;
                }
            }
        }
    }

    public IEnumerator FadeAHD(Color start, Color end, float duration, GameObject target)
    {
        TextMeshPro TMComponent = target.GetComponentInChildren<TextMeshPro>();
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            //right here, you can now use normalizedTime as the third parameter in any Lerp from start to end
            TMComponent.color = Color.Lerp(start, end, normalizedTime);
            yield return null;
        }
        TMComponent.color = end; //without this, the value will end at something like 0.9992367
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
                //StopCoroutine(TypeDialogue(firstLine.lineString, firstLine.typeSpeed));
                if (TypeCO != null) { StopCoroutine(TypeCO); }
                TypeCO = StartCoroutine(TypeDialogue(firstLine.lineString, firstLine.typeSpeed));

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
        if (audio != null)
        {
            //Debug.Log($"Playing {audio}");
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

    private Coroutine TypeCO;
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

        //If the line can change what dialogue the NPC displays above its head when not in conversation, do it
        if (currentDialogueLine.canChangeAboveHeadDialogue)
        {
            Npcscript currentNPCScript = dialogueSource.GetComponent<Npcscript>();
            currentNPCScript.currentAHD = currentDialogueLine.AHDialogueIndexToSwitchTo;
        }


        //If the line can give the player a quest, do it
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
            //Debug.Log($"Full line is {line}");
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
            if (isTyping && Input.GetButton("Interact"))
            {
                isTyping = false;
                if (TypeCO != null) { StopCoroutine(TypeCO); }
                dialogueText.text = "";
                dialogueText.text = line.ToString();
                yield return new WaitForSecondsRealtime(0.2f);
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
