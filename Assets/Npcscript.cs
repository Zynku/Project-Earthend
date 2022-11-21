using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System.Linq;

public class Npcscript : MonoBehaviour
{
    [Header("Set these in Inspector")]
    public bool facePlayer;
    public bool hasSpecialDialogue;
    public int talkedToTimesForSpecialDialogue;
    [Range(0f, 1f)] public float talkingVolume = 1;
    public AudioClip talkingClip;

    public bool beenTalkedTo = false;   
    private int talkedToTimes = 0;
    public bool inConversation = false;
    public bool playerInRange;                                  //Is controlled by Npccollisionhandler script on child

    [Header("Dialogue & Conversation Variables")]
    [SerializeField] public Dialogue myDialogue;                         //Dialogue is a function of the dialogue class. Check Systems folder in Assets
    [SerializeField] public AboveHeadDialogueLine[] ahDialogueList;
    [SerializeField] public int currentAHD;                     //The Above Head Dialogue that will show currently above the NPC
    [HideInInspector] public bool showingAHD;                    //Has an AHD been instantiated from the dialogue manager and is currently live?
    [SerializeField] public Dialogue specialDialogue;
    [SerializeField] public List<CharacterDialogueSprite> dialogueSprites;


    DialogueManager dialogueManager;
    [HideInInspector] public AudioSource audiosource;
    [HideInInspector] public GameObject Player;
    [HideInInspector] public Animator animator;

    public bool canTalk = true;


    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        audiosource = GetComponent<AudioSource>();
        dialogueManager = GameManager.instance.dialogueManager;

    }


    [ButtonMethod]
    public void PlayCurrentClip()
    {
        audiosource.PlayOneShot(audiosource.clip);
    }

    // Update is called once per frame
    void Update()
    {
        if (facePlayer) { alwaysFacePlayer(); }

        //If player is in range
        if (playerInRange && !inConversation)
        {
            animator.SetBool("Player In Range", true);
            if( ahDialogueList.Length > 0 && !showingAHD)
            {
                for (int i = 0; i < ahDialogueList.Length; i++)     //Look through AHDialogue List for the AHDialogue that has the corresponding ID
                {
                    if (ahDialogueList[i].ahID == currentAHD)
                    {
                        dialogueManager.ShowAboveHeadDialogue(ahDialogueList[i], gameObject);   //If found, display it
                        return;
                    }

                    if (ahDialogueList[i].ahID != currentAHD && i == ahDialogueList.Length - 1)     //If we come to the last item, and there's still no match, something's wrong
                    {
                        Debug.LogWarning($"{gameObject} tried to show an Above Head Dialogue with an ID that doesn't exist! Please check AHIDs in Dialogue");
                    }
                }
            }
        }

        //Out of range and this is the closest NPC
        if (!playerInRange && Player.GetComponent<Charcontrol>().closestNPC == this.gameObject && showingAHD)
        {
            dialogueManager.HideDialogue();
            StartCoroutine(dialogueManager.HideAboveHeadDialogue(gameObject));
            audiosource.Stop();
        }

        //Initiates dialogue
        if (playerInRange && Input.GetButtonDown("Interact"))
        {
            BeginConversation();
        }

        //Checks dialoguemanager to know when typing has stopped
        if (!dialogueManager.isTyping)
        {
            animator.SetBool("Talking", false);
            if (dialogueManager.currentDialogueLine.audio.Count == 0) { audiosource.Stop(); }
        }

        //Stops talking if player isnt in range
        if (!playerInRange && inConversation)
        {
            dialogueManager.HideDialogue();
            dialogueManager.currentLineArray = 0;
            dialogueManager.playerInConversation = false;
            inConversation = false;
            animator.SetBool("Talking", false);
            animator.SetBool("Player In Range", false);
            audiosource.Stop(); //Has no voice lines, and is using voice beeps
            ++talkedToTimes;
        }

        if (inConversation && showingAHD)
        {
            StartCoroutine(dialogueManager.HideAboveHeadDialogue(gameObject));
        }

        //If the end of the conversation is reached
        if (dialogueManager.endOfConversation)
        {
            inConversation = false;
            ++talkedToTimes;
        }
    }

    public void BeginConversation()
    {
        if (canTalk && !dialogueManager.playerInConversation)
        {
            if (!myDialogue)
            {
                Debug.Log($"NPC has no dialogue! Check {this.transform.name}'s NPC script");
                return;
            }
            else if (hasSpecialDialogue) //Sends special dialogue to dialogue manager if the correct amount of times has been reached, other than that sends regular dialogue
            {
                StartSpecialDialogue();
            }
            else
            {
                StartDialogue();
            }
        }
        else if (!dialogueManager.isTyping && dialogueManager.playerInConversation)
        {
            dialogueManager.ContinueConversation();
        }
        else if (dialogueManager.currentDialogueLine.hasChoice)
        {
            dialogueManager.ContinueConversation();
        }
    }

    //Cooldown timer between talks
    public IEnumerator TalkCoolDown()
    {
        canTalk = false;
        yield return new WaitForSeconds(0.1f);
        canTalk = true;
    }

    //Faces player by changing transform x variable
    public void alwaysFacePlayer()
    {
        if (Player.transform.position.x > transform.position.x)
        {
            //Player to the right
            transform.localScale = new Vector2(1, 1);
        }

        if (Player.transform.position.x < transform.position.x)
        {
            //Player to the left
            transform.localScale = new Vector2(-1, 1);
        }
    }

    public void StartDialogue()
    {
        dialogueManager.StartConversation(myDialogue, myDialogue.defaultTreeId, gameObject);                      //Passes this dialogue instance to the manager
        //dialogueManager.ShowDialogueCharacter(dialogueanimator);
        StartCoroutine(TalkCoolDown());
        beenTalkedTo = true;
        inConversation = true;

        animator.SetBool("Talking", true);
        animator.SetBool("Been Talked To", true);

        if (dialogueManager.lineHasAudio)
        {
            return;
        }
        else
        {
            audiosource.loop = true;
            audiosource.volume = talkingVolume;
            audiosource.clip = talkingClip;
            audiosource.Play();
        }
    }

    public void StartSpecialDialogue()
    {
        dialogueManager.StartConversation(specialDialogue, myDialogue.defaultTreeId, gameObject);
        //dialogueManager.ShowDialogueCharacter(dialogueanimator);
        StartCoroutine(TalkCoolDown());
        beenTalkedTo = true;
        inConversation = true;

        animator.SetBool("Talking", true);
        animator.SetBool("Been Talked To", true);

        audiosource.loop = true;
        audiosource.volume = talkingVolume;
        audiosource.clip = talkingClip;
        audiosource.Play();
    }

    public bool CheckForAudio()
    {
        foreach (DialogueTree DT in myDialogue.dialogueTrees)
        {
            foreach (DialogueLine DL in DT.dialogueLines)
            {
                if (DL.audio != null)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
