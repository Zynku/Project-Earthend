using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charhealth : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    private int damageDoneToMeMax;
    private int damageDoneToMeMin;
    public int damageDoneToMe;
    public GameObject floatingTextPrefab;
    public Vector3 dmgTextOffset;
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
        //Stops health from overflowing or underflowing
        if (currentHealth > maxHealth) {currentHealth = maxHealth;}
        if (currentHealth < 0) { currentHealth = 0;}

        if (Input.GetKeyDown(KeyCode.J))
        {
            ResetHealth();
        }
    }
    //Checks for collisions from enemy hitboxes
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("enemy_attackhitbox"))
        {
            //Gets max and min attack values from enemy script, returns random value between them, applies damage
            damageDoneToMeMax = Mathf.FloorToInt(collision.gameObject.GetComponentInParent<enemy_controller>().attackdamageMax);
            damageDoneToMeMin = Mathf.FloorToInt(collision.gameObject.GetComponentInParent<enemy_controller>().attackdamageMin);
            damageDoneToMe = (Random.Range(damageDoneToMeMax, damageDoneToMeMin));
            TakeDamage(damageDoneToMe);
        }
    }
    //Subtracts damage calculated above from health, healthbar reacts to show this
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthbar.SetHealth(currentHealth);
        var floattext = Instantiate(floatingTextPrefab, transform.position + dmgTextOffset, Quaternion.identity);
        floattext.GetComponent<TMPro.TextMeshPro>().text = damage.ToString();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        healthbar.SetHealth(maxHealth * 1);
    }
}
