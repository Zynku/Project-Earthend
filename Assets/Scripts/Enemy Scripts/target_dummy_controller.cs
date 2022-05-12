﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class target_dummy_controller : MonoBehaviour
{
    public Animator animator;
    public AudioSource audiosource;

    public int currentHealth;
    public int maxHealth;
    public float collisionDir;
    public int damageDoneToMeMax;
    public int damageDoneToMeMin;
    public int damageDoneToMe;
    private float dmgCooldown;
    private float dmgCooldownTargetTime = 0.1f;
    private float healCooldown;
    private float healCooldownTargetTime = 2f;
    public Vector3 dmgTextOffset;
    public GameObject floatingDmgTextPrefab;
    public GameObject floatingHealthTextPrefab;

    [SerializeField] AudioClip[] Hit;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        audiosource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        dmgCooldown -= Time.deltaTime;
        if (dmgCooldown < 0) { dmgCooldown = 0; }

        healCooldown -= Time.deltaTime;
        if (healCooldown < 0) { healCooldown = 0; }
    }

    
    void FixedUpdate()
    {
        //Stops health from overflowing or underflowing
        if (currentHealth > maxHealth) { currentHealth = maxHealth; }
        if (currentHealth < 0) { currentHealth = 0; }

        if (healCooldown <= 0 && currentHealth < maxHealth)
        {
            AddHealth(30);
        }

        if (currentHealth == maxHealth)
        {
            healCooldown = healCooldownTargetTime;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {

    }

    public void OnTriggerExit2D(Collider2D collision)
    {
            animator.SetBool("BeenHit", false);
    }


    public void TakeDamage(int damage)
    {
        if (dmgCooldown <= 0 && currentHealth > 0)
        {
            currentHealth -= damage;

            var floattext = Instantiate(floatingDmgTextPrefab, transform.position + dmgTextOffset, Quaternion.identity);
            floattext.GetComponent<TMPro.TextMeshPro>().text = damage.ToString();
            //Applies force to show direction hit from.
            floattext.GetComponent<Rigidbody2D>().AddForce(new Vector2(collisionDir, 0), ForceMode2D.Impulse);
            dmgCooldown = dmgCooldownTargetTime;
            healCooldown = healCooldownTargetTime;
        }
        BeenHit();
    }

    public void BeenHit()
    {
        animator.SetBool("BeenHit", true);
        AudioClip hitclip = Hit[Random.Range(0, Hit.Length)];
        audiosource.PlayOneShot(hitclip);
    }

    public void AddHealth(int health)
    {
        currentHealth += health;

        //var floattext = Instantiate(floatingHealthTextPrefab, transform.position + dmgTextOffset, Quaternion.identity);
        //floattext.GetComponent<TMPro.TextMeshPro>().text = health.ToString();
    }
}

