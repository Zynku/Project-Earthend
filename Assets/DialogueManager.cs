using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.Rendering;
using JetBrains.Annotations;
using Newtonsoft.Json.Serialization;
using UnityEngine.U2D;
using UnityEditor;
using System;

public class DialogueManager : MonoBehaviour
{
    public VisualElement wholeScreen;
    public VisualElement colorBarTop;
    public VisualElement colorBarBottom;
    public TextElement characterNameText;
    public TextElement dialogueText;

    public List<CharacterDialogueSprite> importantDialogueSprites;
    public List<CharSpriteBox> spriteBoxesLeft; //Classes that control Character Sprite Box UI Elements on the left;
    public List<CharSpriteBox> spriteBoxesRight;


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
    //public TextMeshProUGUI dialogueText;            //TextMesh component of the text that is shown on screen
    public GameObject dialogueSource;               //The gameObject that has this dialogue attached to it
    public int currentTreeNumber = 0;
    public string currentTreeId;
    public int currentLine = 0;                     //What number line are we on?(Used for visual purposes)
    public int currentLineArray = 0;                //What number line are we on?(used for array references)                
    public Dialogue currentDialogue;
    public DialogueTree currentDialogueTree;
    public DialogueLine currentDialogueLine;
    public bool lineHasAudio = false;
    public Npcscript currentNpcScript;

    [Header("Dialogue Choice Variables")]
    public TextMeshProUGUI dialogueOnChoiceText;    //TextMesh component of the text that is shown on screen before a choice
    public TextMeshProUGUI choiceOneText;
    public TextMeshProUGUI choiceTwoText;

    [Header("Dialogue Quest Variables")]
    public List<Quest> questsToGive;                //Quests that will be given to the player after this conversation is done

    [Separator("Audio")]
    AudioSource audioSource;
    public AudioClip dialogueEnter;
    public AudioClip dialogueExit;

    Animator dialogueCharAnim;                      //Animator component of the dialogue character
    Charcontrol charcontrol;
    QuestManager questManager;

    private void OnValidate()   //Runs at the start of script loading in edit mode
    {
        EditorApplication.playModeStateChanged += DisableUIInEditMode;
    }

    public void DisableUIInEditMode(PlayModeStateChange state)
    {
        try
        {
            UIDocument UIDoc = GetComponent<UIDocument>();
            UIDoc.enabled = false;
            Debug.Log("Disabling dialogue UI in edit mode");
        }
        catch (MissingReferenceException)
        {
        }
    }

    private void Awake()
    {
        UIDocument UIDoc = GetComponent<UIDocument>();
        UIDoc.enabled = true;
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        colorBarTop = root.Q<VisualElement>("color-bar-top");
        colorBarBottom = root.Q<VisualElement>("color-bar-bottom");
        //characterSpriteBoxL = root.Q<VisualElement>("character-sprite-box-L");
        //characterSpriteBoxR = root.Q<VisualElement>("character-sprite-box-R");
        List<VisualElement> listOfCharBoxes = root.Query(className: "character-sprite-box").ToList();   //Adds all sprite boxes to a temp list
        for (int i = 0; i < listOfCharBoxes.Count; i++)                                                 //Loops through all sprite boxes, creates a CharacterSpriteBox, and adds them to a left or right list based on its parent name
        {
            CharSpriteBox CSB = new(listOfCharBoxes[i], null, i, "", false);
            CSB.name = listOfCharBoxes[i].name;

            if (listOfCharBoxes[i].parent.name == "boxes-left")
            {
                CSB.characterSpriteBox.AddToClassList("char-sprite-box-offscreenL");
                spriteBoxesLeft.Add(CSB);
            }
            else
            {
                CSB.characterSpriteBox.AddToClassList("char-sprite-box-offscreenR");
                spriteBoxesRight.Add(CSB);
            }
            
        }

        characterNameText = root.Q<TextElement>("name-text");
        dialogueText = root.Q<TextElement>("dialogue-text");

        wholeScreen = root.Q<VisualElement>("whole-screen");
        wholeScreen.style.display = DisplayStyle.None;                              //Disables dialoguebox UI initially

        continueText.enabled = false;
        currentLineArray = 0;
        currentLine = 0;

        aboveheaddialogueBox.SetActive(false);
        aboveheaddialogue.enabled = false;

        //Makes sure text renders infront everything else
        aboveheaddialogue.GetComponent<MeshRenderer>().sortingOrder = 69;

        audioSource = GetComponent<AudioSource>();

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
        try
        {
            GameObject closestNPC = charcontrol.closestNPC.gameObject;
        }
        catch (System.NullReferenceException)
        {
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
                    return;
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
        this.currentDialogue = dialogue;
        this.dialogueSource = dialogueSource;

        var dialogueTree = dialogue.dialogueTrees.Find(i => i.DialogueTreeId == treeId);    //Finds the dialogue tree in the list by ID
        if (dialogueTree == null)
        {
            Debug.LogWarning("Cannot find correct Dialogue Tree via ID! Are your IDs correct? Make sure to assign your first dialogue tree as 1 and subsequent trees as subsequent letters / numbers");
            return;
        }
        else
        {
            currentTreeId = dialogueTree.DialogueTreeId;
            this.currentDialogueTree = dialogueTree;
            this.currentDialogueLine = dialogueTree.dialogueLines[currentLineArray];
            currentNpcScript = dialogueSource.GetComponent<Npcscript>();
        }

        wholeScreen.style.display = DisplayStyle.Flex;          //Enables the whole UI Screen while disabling character sprites.

        foreach (var item in spriteBoxesLeft) { item.characterSpriteBox.style.display = DisplayStyle.None;}
        foreach (var item in spriteBoxesRight) { item.characterSpriteBox.style.display = DisplayStyle.None; }

        choicesBox.SetActive(false);
        aboveheaddialogueBox.SetActive(false);
        aboveheaddialogue.enabled = false;

        //--------Finds Speaker-----------------------------------------------
        if (dialogueTree.dialogueLines[currentLineArray].lineOwner == "")
        {
            characterNameText.text = dialogueTree.dialogueSpeaker; //Assigns the speaker to the whole Dialogue owner if the line doesn't have an individual owner
        } 
        else
        {
            characterNameText.text = dialogueTree.dialogueLines[currentLineArray].lineOwner; //Assigns the speaker as the line owner if this specific line is owned by another
        }
        AssignCharacterSpriteBoxes();
        
        DialogueLine firstLine = currentDialogueTree.dialogueLines[0];
        if (!isTyping)
        {
            //If the character somehow has no lines, say so
            if (currentDialogueTree.dialogueLines.Count == 0)
            {
                Debug.LogWarning("Character has no dialogue lines!");
                return;
            }
            //Otherwise, passes one line at a time to coroutine
            else
            {
                currentLineArray = 0;
                currentLine = 1;

                //Passes the first line to the Coroutine so its starts typing
                if (TypeCO != null) { StopCoroutine(TypeCO); }
                TypeCO = StartCoroutine(TypeDialogue(firstLine.lineString, firstLine.typeSpeed));

                endOfConversation = false;
                playerInConversation = true;
                audioSource.PlayOneShot(dialogueEnter);
            }
        }

        if (currentDialogueLine.audio.Count > 0)    //If the dialogue has audio to use (recorded audio), play it.
        {
            lineHasAudio = true;
            int randomNum = UnityEngine.Random.Range(0, currentDialogueLine.audio.Count);
            AudioClip clip = currentDialogueLine.audio[randomNum];
            PlayDialogueLine(clip, currentDialogueLine.audioVol, dialogueSource.GetComponent<AudioSource>());
        }
        else
        {
            lineHasAudio = false;
            MakeConversationBeeps();
        }

        if (currentDialogueTree.pauseGameOnOpen)
        {
            GameManager.instance.PauseGame();
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

            //--------Finds Speaker-----------------------------------------------
            if (dialogueTree.dialogueLines[currentLineArray].lineOwner == "")
            {
                characterNameText.text = dialogueTree.dialogueSpeaker;
            }
            else
            {
                characterNameText.text = dialogueTree.dialogueLines[currentLineArray].lineOwner;
            }


            StartCoroutine(TypeDialogue(currentDialogueLine.lineString, currentDialogueLine.typeSpeed));

            //Passes the audio to PlayDialogueLine() to play audio clip
            if (currentDialogueLine.audio.Count > 0)
            {
                lineHasAudio = true;
                int randomNum = UnityEngine.Random.Range(0, currentDialogueLine.audio.Count);
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
            wholeScreen.style.display = DisplayStyle.None;
            choicesBox.SetActive(false);
            aboveheaddialogueBox.SetActive(false);
            aboveheaddialogue.enabled = false;
            playerInConversation = false;
            isTyping = false;

            Npcscript npcScript = dialogueSource.GetComponent<Npcscript>();
            npcScript.audiosource.Stop();

            StopAllCoroutines();
        }
        else
        {
            wholeScreen.style.display = DisplayStyle.None;
            foreach (var item in spriteBoxesLeft) 
            {
                item.Reset();
                item.characterSpriteBox.AddToClassList("char-sprite-box-offscreenL");   //Moves characterspriteboxes back offscreen
                VisualElement visualSprite = item.characterSpriteBox.Q<VisualElement>("character-sprite");
                visualSprite.style.scale = new Scale(new Vector2(1, 1));    //Flips all sprites back to default orientation
            }
            foreach (var item in spriteBoxesRight)
            {
                item.Reset();
                item.characterSpriteBox.AddToClassList("char-sprite-box-offscreenR");
                VisualElement visualSprite = item.characterSpriteBox.Q<VisualElement>("character-sprite");
                visualSprite.style.scale = new Scale(new Vector2(1, 1));
            }

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

            audioSource.PlayOneShot(dialogueExit);

            StopAllCoroutines();
        }

        if (GameManager.instance.paused)
        {
            GameManager.instance.ResumeGame();
        }
    }

    public void AssignCharacterSpriteBoxes()
    {
        //Find dialogue owner first and assign that as dialogue box owner 1
        //Create new list keeping track of owners as strings
        //Loop through all dialogue lines looking at dialogue line owners
        //If line owner is different than dialogue owner, check list to make sure we haven't logged this owner before
        //If we have, move to next line
        //If we haven't assign new owner to new dialogue box

        List<string> dialogueLineOwnersInDialogue = new();

        spriteBoxesLeft[0].speakerName = currentDialogueTree.dialogueSpeaker;   //Assigns the first sprite box to the speaker of the dialogue
        spriteBoxesLeft[0].speaker = dialogueSource;                            //Assign gameObject as well
        dialogueLineOwnersInDialogue.Add(spriteBoxesLeft[0].speakerName);

        spriteBoxesRight[0].speakerName = "Nikita";
        spriteBoxesRight[0].speaker = GameManager.instance.Player;
        dialogueLineOwnersInDialogue.Add("Nikita");

        int startingIndex = 1;
        foreach (var item in currentDialogueTree.dialogueLines) //Loop through all dialogue lines...
        {
            if (item.lineOwner != "")                           //If the line owner isnt empty (meaning the line owner is the dialogue tree owner)
            {
                if (!dialogueLineOwnersInDialogue.Contains(item.lineOwner)) //If the line owner isnt one that we've checked before...
                {
                    spriteBoxesLeft[startingIndex].speakerName = item.lineOwner;    //Assign a new sprite box to this new owner that we haven't seen before
                    dialogueLineOwnersInDialogue.Add(item.lineOwner);           //Adds this new name to the list of names we check against for future names
                    startingIndex++;
                }
            }
        }

        //--------Finds Sprites-----------------------------------------------
        if (currentNpcScript.dialogueSprites.Count > 0) //Enables character sprites if there is one to be used.
        {
            //NPC Sprite
            spriteBoxesLeft[0].active = true;
            spriteBoxesLeft[0].characterSpriteBox.RemoveFromClassList("char-sprite-box-offscreenL");
            VisualElement spriteBox = spriteBoxesLeft[0].characterSpriteBox;
            spriteBox.style.display = DisplayStyle.Flex;
            spriteBox.style.backgroundColor = new Color(0, 0, 0, 0);
            for (int i = 0; i < currentNpcScript.dialogueSprites.Count; i++)    //Loops through all the NPC's sprites located in NPC Script looking for the default one
            {
                if (currentNpcScript.dialogueSprites[i].spriteMood == "default" || currentNpcScript.dialogueSprites[i].spriteMood == "")
                {
                    VisualElement visualSprite = spriteBox.Q<VisualElement>("character-sprite");
                    visualSprite.style.backgroundImage = new StyleBackground(currentNpcScript.dialogueSprites[i].characterSprite);
                    visualSprite.style.backgroundColor = new Color(0, 0, 0, 0);
                    if (currentNpcScript.dialogueSprites[i].flipSpriteX) { visualSprite.style.scale = new Scale(new Vector2(-1, 1)); } //Flips the sprite if needed
                    break;
                }
            }

            //Player Sprite
            spriteBoxesRight[0].active = true;
            spriteBoxesRight[0].characterSpriteBox.RemoveFromClassList("char-sprite-box-offscreenR");
            VisualElement spriteBoxP = spriteBoxesRight[0].characterSpriteBox;
            spriteBoxP.style.display = DisplayStyle.Flex;
            spriteBoxP.style.backgroundColor = new Color(0, 0, 0, 0);
            for (int i = 0; i < importantDialogueSprites.Count; i++)    //Loops through all player sprites located on DialogueManager looking for the default one
            {
                if (importantDialogueSprites[i].spriteOwner == "player")
                {
                    if (importantDialogueSprites[i].spriteMood == "default" || importantDialogueSprites[i].spriteMood == "")
                    {
                        VisualElement visualSprite = spriteBoxP.Q<VisualElement>("character-sprite");
                        visualSprite.style.backgroundImage = new StyleBackground(importantDialogueSprites[i].characterSprite);
                        visualSprite.style.backgroundColor = new Color(0, 0, 0, 0);
                        if (importantDialogueSprites[i].flipSpriteX) { visualSprite.style.scale = new Scale(new Vector2(-1, 1)); } //Flips the sprite if needed
                        break;
                    }
                }
            }
        }
        else
        {
            spriteBoxesLeft[0].characterSpriteBox.style.backgroundImage = null;
        }
    }

    public IEnumerator AnimateCharacterSprites()    //I can't figure this out. The class list is added but the transition isnt triggering.
    {
        yield return new WaitForSeconds(0.4f);
        for (int i = 0; i < spriteBoxesLeft.Count; i++)
        {
            if (spriteBoxesLeft[i].speakerName == currentDialogueLine.lineOwner || spriteBoxesLeft[i].speakerName == "")
            {
                spriteBoxesLeft[i].characterSpriteBox.AddToClassList("char-sprite-box-speaking");
                spriteBoxesLeft[i].characterSpriteBox.Q<VisualElement>("character-sprite").AddToClassList("char-sprite-box-speaking");
            }

            if (i == spriteBoxesLeft.Count - 1)
            {
                for (int j = 0; j < spriteBoxesRight.Count; j++)
                {
                    if (spriteBoxesLeft[j].speakerName == currentDialogueLine.lineOwner)
                    {
                        spriteBoxesLeft[j].characterSpriteBox.AddToClassList("char-sprite-box-speaking");
                        spriteBoxesLeft[i].characterSpriteBox.Q<VisualElement>("character-sprite").AddToClassList("char-sprite-box-speaking");
                    }
                }
            }
        }
    }

    private Coroutine TypeCO;
    //Takes dialogue and shows it letter by letter
    public IEnumerator TypeDialogue(string line, int lettersPerSecond)
    {
        //StartCoroutine(AnimateCharacterSprites());
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
            currentDialogue.defaultTreeId = currentDialogueTree.dialogueLines[currentLineArray].treeIdToSwitchTo;
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
            if (isTyping && Charinputs.instance.interact.WasPressedThisFrame())
            {
                isTyping = false;
                if (TypeCO != null) { StopCoroutine(TypeCO); }
                dialogueText.text = "";
                dialogueText.text = line.ToString();
                yield return new WaitForSecondsRealtime(0.2f);
                yield break;
            }

            float yieldTime = (1 / (lettersPerSecond * Time.deltaTime))/10;
            yield return new WaitForSeconds(yieldTime);
        }
        isTyping = false;
    }

    //Choices are given here. The regular dialogue box is disabled and the choices box (which is slightly different) is shown in its place
    //The player doesn't notice because they have the same dimensions
    public IEnumerator GivePlayerChoices(string line, int lettersPerSecond)
    {
        DialogueLine currentDialogueLine = this.currentDialogue.dialogueTrees[currentTreeNumber].dialogueLines[currentLineArray];
        ChoiceTree choiceTree = currentDialogue.choiceTrees.Find(i => i.choiceTreeID == currentDialogueLine.choiceTreeID);
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
        DialogueLine currentDialogueLine = this.currentDialogue.dialogueTrees[currentTreeNumber].dialogueLines[currentLineArray];
        ChoiceTree choiceTree = currentDialogue.choiceTrees.Find(i => i.choiceTreeID == currentDialogueLine.choiceTreeID);
        ChoiceLine choiceOne = choiceTree.choices[0];
        ChoiceLine choiceTwo = choiceTree.choices[1];

        currentTreeId = choiceOne.treeIdToSwitchTo;             //Switches tree ID so the character can "respond"
        currentDialogue.defaultTreeId = choiceOne.newDefaultTreeId;    //Switches default tree ID so character says different things based on what you chose
        currentLineArray = 0;
        currentLine = 1;
        StartConversation(this.currentDialogue, choiceOne.treeIdToSwitchTo, dialogueSource);
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
        DialogueLine currentDialogueLine = this.currentDialogue.dialogueTrees[currentTreeNumber].dialogueLines[currentLineArray];
        ChoiceTree choiceTree = currentDialogue.choiceTrees.Find(i => i.choiceTreeID == currentDialogueLine.choiceTreeID);
        ChoiceLine choiceOne = choiceTree.choices[0];
        ChoiceLine choiceTwo = choiceTree.choices[1];

        currentTreeId = choiceTwo.treeIdToSwitchTo;
        currentDialogue.defaultTreeId = choiceTwo.newDefaultTreeId;
        currentLineArray = 0;
        currentLine = 1;
        StartConversation(this.currentDialogue, choiceTwo.treeIdToSwitchTo, dialogueSource);
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
