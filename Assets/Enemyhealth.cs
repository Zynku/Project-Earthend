using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemyhealth : MonoBehaviour
{
    
    public int maxHealth;
    public int currentHealth;
    public int damageDoneToMeMax;
    public int damageDoneToMeMin;
    public int damageDoneToMe;
    public bool stunnedcheck;
    //public Healthbar healthbar;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        //healthbar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        //Stops health from overflowing or underflowing
        if (currentHealth > maxHealth) { currentHealth = maxHealth; }
        if (currentHealth < 0) { currentHealth = 0; }

        if (Input.GetKeyDown(KeyCode.K))
        {
            ResetHealth();
        }
    }
    //Checks for collisions from enemy hitboxes
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("player_attackhitbox"))
        {
            //Gets max and min attack values from enemy script, returns random value between them, applies damage
            damageDoneToMeMax = Mathf.FloorToInt(collision.gameObject.GetComponentInParent<Char_control>().attackdamageMax);
            damageDoneToMeMin = Mathf.FloorToInt(collision.gameObject.GetComponentInParent<Char_control>().attackdamageMin);
            damageDoneToMe = (Random.Range(damageDoneToMeMax, damageDoneToMeMin));
            TakeDamage(damageDoneToMe);

            stunnedcheck = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("player_attackhitbox"))
        {
            stunnedcheck = false;
        }
    }


    //Subtracts damage calculated above from health, healthbar reacts to show this
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
       //healthbar.SetHealth(currentHealth);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        //healthbar.SetHealth(maxHealth * 1);
    }
}

