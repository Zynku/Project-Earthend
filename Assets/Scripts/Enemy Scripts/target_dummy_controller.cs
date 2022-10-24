using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class target_dummy_controller : MonoBehaviour
{
    public Animator animator;
    public AudioSource audiosource;
    Enemymain enemymainscript;

    public float collisionDir;
    private float healCooldown;
    public float healCooldownTargetTime = 2f;
    public Vector3 dmgTextOffset;
    public GameObject floatingDmgTextPrefab;
    public GameObject floatingHealthTextPrefab;

    [SerializeField] AudioClip[] Hit;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        audiosource = GetComponent<AudioSource>();
        enemymainscript = GetComponent<Enemymain>();
        enemymainscript.enemyBeenHit += BeenHit ;
    }

    private void Update()
    {
        healCooldown -= Time.deltaTime;
        if (healCooldown < 0) { healCooldown = 0; }
        enemymainscript.currentHealth = enemymainscript.maxHealth;
    }

    void FixedUpdate()
    {
        if (healCooldown <= 0 && enemymainscript.currentHealth < enemymainscript.maxHealth) //Automatically starts regenerating health if the time has been reached
        {
            enemymainscript.AddHealth(30);
        }

        if (enemymainscript.currentHealth == enemymainscript.maxHealth)
        {
            //healCooldown = healCooldownTargetTime;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
            animator.SetBool("BeenHit", false);
    }

    public void BeenHit()
    {
        animator.SetBool("BeenHit", true);
        AudioClip hitclip = Hit[Random.Range(0, Hit.Length)];
        audiosource.PlayOneShot(hitclip);
    }
}

