using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npcscript : MonoBehaviour
{
    public bool facePlayer;
    [HideInInspector] public bool playerInRange;
    [SerializeField] Dialogue dialogue;

    [Range(0f, 1f)]
    public float talkingVolume = 1;
    public AudioClip talkingClip;

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

        //Initiates dialogue
        if (playerInRange && Input.GetAxisRaw("Interact") > 0)
        {
            if (canTalk)
            {
                DialogueManager.Instance.ShowDialogue(dialogue);
                StartCoroutine(TalkCoolDown());
                animator.SetBool("Talking", true);

                audiosource.loop = true;
                audiosource.volume = talkingVolume;
                audiosource.clip = talkingClip;
                audiosource.Play();
            }
        }

        //Checks dialoguemanager to know when typing has stopped
        if (!DialogueManager.Instance.isTyping)
        {
            animator.SetBool("Talking", false);
            audiosource.Stop();
        }

        //Stops talking if player isnt in range
        if (!playerInRange)
        {
            DialogueManager.Instance.HideDialogue();
            DialogueManager.Instance.currentLine = 0;
            animator.SetBool("Talking", false);
            audiosource.Stop();
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
}
