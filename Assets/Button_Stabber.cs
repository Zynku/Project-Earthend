using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Stabber : MonoBehaviour
{
    Animator animator;
    AudioSource audiosource;
    public AudioClip Press;
    public AudioClip Spawn;
    public GameObject Melee1;
    public float coolDownTimer;
    public float coolDownTargetTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        coolDownTimer = coolDownTargetTime;
        animator = GetComponent<Animator>();
        audiosource = GetComponent<AudioSource>();
        Melee1.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        //coolDownTimer counts down rounded to two decimal places. At 0 resets to coolDown
        coolDownTimer = ((Mathf.Round(coolDownTimer * 100f)) / 100.0f) - Time.fixedDeltaTime;
        if (coolDownTimer < 0)
        {
            coolDownTimer = 0;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //If the player enters...
        if (collision.CompareTag("Player"))
        {
            //if they interact and button not on cooldown...
            if (Input.GetAxisRaw("Interact") > 0 && coolDownTimer == 0)
            {
                //restarts cooldown, presses button, plays audio, does the thing
                coolDownTimer = coolDownTargetTime;
                animator.SetTrigger("Pressed");
                audiosource.PlayOneShot(Press);
            }
        }
    }

    public void onStabStart()
    {
        Melee1.SetActive(true);
    }

    public void onStabEnd()
    {
        Melee1.SetActive(false);
    }
}
