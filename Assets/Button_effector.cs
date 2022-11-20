using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_effector : MonoBehaviour
{
    Animator animator;
    AudioSource audiosource;
    GameObject Player;
    public float interactionRange = 0.4f;
    public bool playerInRange = false;
    public AudioClip Press;
    public AudioClip Spawn;
    private float coolDownTimer;
    public float coolDownTargetTime = 5f;
    public GameObject[] Spawnable;
    public bool toggleable;
    private GameObject Spawnableclone;
    public float Spawnamount;
    public float forceX;
    public float forceY;
    public float forceRotation;
    
    // Start is called before the first frame update
    void Start()
    {
        coolDownTimer = coolDownTargetTime;
        animator = GetComponent<Animator>();
        audiosource = GetComponent<AudioSource>();
        Spawnableclone = Spawnable[0];
        Player = GameManager.instance.Player;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //coolDownTimer counts down rounded to two decimal places. At 0 resets to coolDown
        coolDownTimer = ((Mathf.Round(coolDownTimer * 100f)) / 100.0f)-Time.fixedDeltaTime;
        if (coolDownTimer < 0)
        {
            coolDownTimer = 0;
        }

        if (Vector3.Distance(Player.transform.position, transform.position) < interactionRange)
        {
            playerInRange = true;

            //if they interact and button not on cooldown...
            if (Input.GetAxisRaw("Interact") > 0 && coolDownTimer == 0)
            {
                StopAllCoroutines();
                if (toggleable) { StartCoroutine(ContinuousSpawn()); }
                //restarts cooldown, presses button, plays audio, spawns objects
                coolDownTimer = coolDownTargetTime;
                animator.SetTrigger("Pressed");

                if (Spawn != null) audiosource.PlayOneShot(Spawn);
                if (Press != null) audiosource.PlayOneShot(Press);

                //loops spawning *spawnamount* amount of times. Second line applies random velocity based on params
                for (int i = 0; i < Spawnamount; i++)
                {
                    GameObject randomSpawnable = Spawnable[Random.Range(0, Spawnable.Length)];
                    Spawnableclone = Instantiate(randomSpawnable, new Vector2(transform.position.x, transform.position.y + 0.2f), Quaternion.identity);
                    Spawnableclone.GetComponent<Rigidbody2D>().velocity = new Vector2(forceX / 2 + Random.Range(-0.5f, 0.5f), (forceY / 2 + Random.Range(-0.5f, 0.5f) + 0.2f));
                    Spawnableclone.GetComponent<Rigidbody2D>().AddTorque(Random.Range(forceRotation, -forceRotation));
                }
            }
        }
        else
        { 
            playerInRange = false; 
        }

    }

    public IEnumerator ContinuousSpawn()
    {
        while (toggleable)
        {
            yield return new WaitForSeconds(coolDownTargetTime);
            coolDownTimer = coolDownTargetTime;
            animator.SetTrigger("Pressed");

            if (Spawn != null) audiosource.PlayOneShot(Spawn);
            if (Press != null) audiosource.PlayOneShot(Press);

            //loops spawning *spawnamount* amount of times. Second line applies random velocity based on params
            for (int i = 0; i < Spawnamount; i++)
            {
                GameObject randomSpawnable = Spawnable[Random.Range(0, Spawnable.Length)];
                Spawnableclone = Instantiate(randomSpawnable, new Vector2(transform.position.x, transform.position.y + 0.2f), Quaternion.identity);
                Spawnableclone.GetComponent<Rigidbody2D>().velocity = new Vector2(forceX / 2 + Random.Range(-0.5f, 0.5f), (forceY / 2 + Random.Range(-0.5f, 0.5f) + 0.2f));
                Spawnableclone.GetComponent<Rigidbody2D>().AddTorque(Random.Range(forceRotation, -forceRotation));
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}

