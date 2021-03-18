using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charhealth : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public Healthbar healthbar;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            TakeDamage(20);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            ResetHealth();
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthbar.SetHealth(currentHealth);
    }

    void ResetHealth()
    {
        currentHealth = maxHealth;
        healthbar.SetHealth(maxHealth);
    }
}
