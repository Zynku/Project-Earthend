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
    private float dmgCooldown;
    [HideInInspector] public float collisionDir = 1f;
    [HideInInspector] public float dmgCooldownTargetTime = 0.1f;
    public GameObject floatingDmgTextPrefab;
    public GameObject floatingHealthTextPrefab;
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
        dmgCooldown -= Time.deltaTime;
        if (dmgCooldown < 0) { dmgCooldown = 0;}

        //Stops health from overflowing or underflowing
        if (currentHealth > maxHealth) {currentHealth = maxHealth;}
        if (currentHealth < 0) { currentHealth = 0;}

        if (Input.GetKeyDown(KeyCode.J)) { ResetHealth();}
        if (Input.GetKeyDown(KeyCode.H)) { AddHealth(20); }
    }
    //Checks for collisions from enemy hitboxes
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("enemy_attackhitbox"))
        {
            //Calculates collision direction based on transform x values
            if (collision.transform.position.x > transform.position.x)
            {
                collisionDir = -1;
            }
            else if (collision.transform.position.x < transform.position.x)
            {
                collisionDir = 1;
            }

            //Gets max and min attack values from enemy script, returns random value between them, calls damage function, passing calculated damage to it
            damageDoneToMeMax = Mathf.FloorToInt(collision.gameObject.GetComponentInParent<enemy_controller>().attackdamageMax);
            damageDoneToMeMin = Mathf.FloorToInt(collision.gameObject.GetComponentInParent<enemy_controller>().attackdamageMin);
            damageDoneToMe = (Random.Range(damageDoneToMeMax, damageDoneToMeMin));
            TakeDamage(damageDoneToMe);
        }

        if (collision.gameObject.CompareTag("damage_object"))
        {
            //Calculates collision direction based on transform x values
            if (collision.transform.position.x > transform.position.x)
            {
                collisionDir = -1;
            }
            else if (collision.transform.position.x < transform.position.x)
            {
                collisionDir = 1;
            }

            damageDoneToMe = collision.gameObject.GetComponent<damageObject>().damage;
            TakeDamage(damageDoneToMe);
        }
    }
    //Subtracts damage calculated above from health, healthbar reacts to show this. Only executes when damage cooldown timer is 0 or less
    //Instantiates dmg text, gives it dmg value to display
    public void TakeDamage(int damage)
    {
        if (dmgCooldown <= 0)
        {
            currentHealth -= damage;
            healthbar.SetHealth(currentHealth);
        
            var floattext = Instantiate(floatingDmgTextPrefab, transform.position + dmgTextOffset, Quaternion.identity);
            floattext.GetComponent<TMPro.TextMeshPro>().text = damage.ToString();
            //Applies force to show direction hit from.
            floattext.GetComponent<Rigidbody2D>().AddForce(new Vector2(collisionDir, 0), ForceMode2D.Impulse);
            dmgCooldown = dmgCooldownTargetTime;
        }
    }

    //Adds health, healthbar reacts to show this.
    //Instantiates health text, gives it health value to display
    public void AddHealth(int health)
    {
        currentHealth += health;
        healthbar.SetHealth(currentHealth);

        var floattext = Instantiate(floatingHealthTextPrefab, transform.position + dmgTextOffset, Quaternion.identity);
        floattext.GetComponent<TMPro.TextMeshPro>().text = health.ToString();
    }

    //Adds max health, healthbar reacts to show this.
    //Instantiates health text, gives it max health value to display
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        healthbar.SetHealth(maxHealth * 1);

        var floattext = Instantiate(floatingHealthTextPrefab, transform.position + dmgTextOffset, Quaternion.identity);
        floattext.GetComponent<TMPro.TextMeshPro>().text = maxHealth.ToString();
    }
}
