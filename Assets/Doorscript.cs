using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doorscript : MonoBehaviour
{
    public openDirection Direction;
    public bool isDoorOpen = false;
    private bool playerInRange = false;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        openDirection direction = Direction;
        if (direction == openDirection.Left)
        {
            transform.localScale = new Vector3(-0.7142f, 0.7142f, 0.7142f);
        }

        if (direction == openDirection.Right)
        {
            transform.localScale = new Vector3(0.7142f, 0.7142f, 0.7142f);
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

        if (playerInRange && Input.GetAxisRaw("Interact") > 0 && doorcooldown == 0)
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
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
