using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doorscript : MonoBehaviour
{
    public GameObject Player;
    public openDirection Direction;
    public bool isDoorOpen = false;
    public float interactionRange = 0.4f;
    public bool playerInRange = false;
    private float doorcooldown = 0;
    private float doorcooldowntargettime = 0.5f;

    [Range(0f, 1f)]
    public float OpenVolume = 1;
    public AudioClip Open;

    [Range(0f, 1f)]
    public float CloseVolume = 1;
    public AudioClip Close;

    [Range(0f, 1f)]
    public float DestroyedVolume = 1;
    public AudioClip Destroyed;

    Animator animator;
    BoxCollider2D boxCol;
    AudioSource audiosource;
    
    
    public enum openDirection
    {
        Left,
        Right
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        boxCol = GetComponent<BoxCollider2D>();
        audiosource = GetComponent<AudioSource>();
        Player = GameManager.instance.Player;
    }

    // Update is called once per frame
    void Update()
    {
        openDirection direction = Direction;
        if (direction == openDirection.Left)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (direction == openDirection.Right)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        doorcooldown -= Time.deltaTime;
        if (doorcooldown < 0) { doorcooldown = 0; }


        if (isDoorOpen == true)
        {
            openDoor();
        }

        if (isDoorOpen == false)
        {
            closeDoor();
        }

        if (playerInRange && Charinputs.instance.interact.WasPressedThisFrame() && doorcooldown == 0)
        {
            if (isDoorOpen == false)
            {
                isDoorOpen = true;
                doorcooldown = doorcooldowntargettime;
            }
            else
            {
                isDoorOpen = false;
                doorcooldown = doorcooldowntargettime;
            }
        }

        if (Vector3.Distance(Player.transform.position, transform.position) < interactionRange)
        { playerInRange = true; }
        else { playerInRange = false; }
    }

    void openDoor()
    {
        boxCol.enabled = false;
        animator.SetBool("Door Open", true);
    }

    void closeDoor()
    {
        boxCol.enabled = true;
        animator.SetBool("Door Open", false);
    }

    public void Audio(float audioclip)
    {
        switch (audioclip)
        {
            case 1:
                audiosource.volume = OpenVolume;
                audiosource.PlayOneShot(Open);
                break;

            case 2:
                audiosource.volume = CloseVolume;
                audiosource.PlayOneShot(Close);
                break;

            case 3:
                audiosource.volume = DestroyedVolume;
                audiosource.PlayOneShot(Destroyed);
                break;
        }
    }
}
