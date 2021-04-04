﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbyplayer : MonoBehaviour
{
    Rigidbody2D rb2d;
    public bool hit;
    public float playerDir = 0;
    public float xForce = 0;
    public float yForce = 0;
    public float torqueForce = 0;
    public AudioClip hitGround, hitByPlayer, destroyed;
    AudioSource audiosource;

    public int maxHealth;
    public int currentHealth;
    private int damageDoneToMeMax;
    private int damageDoneToMeMin;
    public int damageDoneToMe;
    [HideInInspector] public float dmgCooldown;
    [HideInInspector] public float dmgCooldownTargetTime = 0.1f;
    [HideInInspector] public float collisionDir = 1f;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponentInParent<Rigidbody2D>();
        audiosource = GetComponentInParent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        dmgCooldown -= Time.deltaTime;
        if (dmgCooldown < 0) { dmgCooldown = 0; }

        //Stops health from overflowing or underflowing
        if (currentHealth > maxHealth) { currentHealth = maxHealth; }
        if (currentHealth < 0) { currentHealth = 0; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            PlayGroundHit();
            
        }
        if (collision.transform.tag == ("player_attackhitbox"))
        {
            rb2d.AddForce(new Vector2(xForce * playerDir * 10, yForce * 10));
            rb2d.AddTorque(Random.Range(torqueForce, -torqueForce));
            playerDir = collision.GetComponentInParent<Char_control>().facingDir;
            hit = true;

            damageDoneToMeMax = Mathf.FloorToInt(collision.gameObject.GetComponentInParent<Char_control>().attackdamageMax);
            damageDoneToMeMin = Mathf.FloorToInt(collision.gameObject.GetComponentInParent<Char_control>().attackdamageMin);
            damageDoneToMe = (Random.Range(damageDoneToMeMax, damageDoneToMeMin));
            TakeDamage(damageDoneToMe);

            PlayPlayerHit();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        hit = false;
    }

    public void TakeDamage(int damage)
    {
        if (dmgCooldown <= 0 && currentHealth > 0)
        {
            currentHealth -= damage;
            dmgCooldown = dmgCooldownTargetTime;
        }
    }

    private void PlayGroundHit()
    {
        audiosource.pitch = (Random.Range(0.8f, 1f));
        audiosource.PlayOneShot(hitGround);
    }

    private void PlayPlayerHit()
    {
        audiosource.pitch = (Random.Range(0.8f, 1f));
        audiosource.PlayOneShot(hitByPlayer);
    }

    private void OnDestroy()
    {
        audiosource.pitch = (Random.Range(0.8f, 1f));
        audiosource.PlayOneShot(destroyed);
    }
}
