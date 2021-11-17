using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] Dialogue myDialogue;                         //Dialogue is a function of the dialogue class. Check Systems folder in Assets
    [SerializeField] Dialogue aboveHeadDialogue;
    [SerializeField] Dialogue specialDialogue;
    public int defaultDialogueTreeNumber = 0;
    public int currentDialogueTreeNumber = 0;

    

    AudioSource audiosource;
    GameObject Player;
    Animator animator;

    public bool canTalk = true;


    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        audiosource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (facePlayer) { alwaysFacePlayer(); }

        //If player is in range
        if (playerInRange && !inConversation)
        {
            animator.SetBool("Player In Range", true);
            DialogueManager.Instance.ShowAboveHeadDialogue(aboveHeadDialogue);
        }

        //Out of range and this is the closest NPC
        if (!playerInRange && Player.GetComponent<Charcontrol>().closestNPC == this.gameObject)
        {
            DialogueManager.Instance.HideDialogue();
        }

        //Initiates dialogue
        if (playerInRange && Input.GetButtonDown("Interact"))
        {
            if (canTalk)
            {
                //Sends special dialogue to dialogue manager if the correct amount of times has been reached, other than that sends regular dialogue
                if (hasSpecialDialogue && (talkedToTimes == talkedToTimesForSpecialDialogue))
                {
                    StartSpecialDialogue();
                }
                else
                {
                    StartDialogue();
                }
                
            }
        }

        //Checks dialoguemanager to know when typing has stopped
        if (!DialogueManager.Instance.isTyping)
        {
            animator.SetBool("Talking", false);
            audiosource.Stop();
        }

        //Stops talking if player isnt in range
        if (!playerInRange && inConversation)
        {
            DialogueManager.Instance.HideDialogue();
            DialogueManager.Instance.currentLine = 0;
            inConversation = false;
            animator.SetBool("Talking", false);
            animator.SetBool("Player In Range", false);
            audiosource.Stop();
            ++talkedToTimes;
        }

        //If the end of the conversation is reached
        if (DialogueManager.Instance.endOfConversation)
        {
            inConversation = false;
            ++talkedToTimes;
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
        DialogueManager.Instance.ShowDialogue(myDialogue, currentDialogueTreeNumber);                      //Passes this dialogue instance to the manager
        //DialogueManager.Instance.ShowDialogueCharacter(dialogueanimator);
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

    public void StartSpecialDialogue()
    {
        DialogueManager.Instance.ShowDialogue(specialDialogue, currentDialogueTreeNumber);
        //DialogueManager.Instance.ShowDialogueCharacter(dialogueanimator);
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
}
