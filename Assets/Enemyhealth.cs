using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemyhealth : MonoBehaviour
{
    
    public int maxHealth;
    public int currentHealth;
    private int damageDoneToMeMax;
    private int damageDoneToMeMin;
    public int damageDoneToMe;
    [HideInInspector] public bool stunnedcheck;
    private bool stunnedShown;
    [HideInInspector] public float dmgCooldown;
    [HideInInspector] public float dmgCooldownTargetTime = 0.1f;
    [HideInInspector] public float collisionDir = 1f;
    public GameObject floatingDmgTextPrefab;
    public GameObject floatingHealthTextPrefab;
    public Vector3 dmgTextOffset;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        dmgCooldown -= Time.deltaTime;
        if (dmgCooldown < 0) { dmgCooldown = 0; }

        //Stops health from overflowing or underflowing
        if (currentHealth > maxHealth) { currentHealth = maxHealth; }
        if (currentHealth < 0) { currentHealth = 0; }

        if (Input.GetKeyDown(KeyCode.K))
        {
            ResetHealth();
        }
    }

    private void FixedUpdate()
    {
        //Shows the text stunned! if enemy enters stunned state. Makes sure it only shows once
        if (stunnedShown == false)
        {
            if (GetComponent<enemy_controller>().currentState == enemy_controller.State.Stunned)
            {
                var stunnedtext = Instantiate(floatingDmgTextPrefab, transform.position + dmgTextOffset, Quaternion.identity);
                stunnedtext.GetComponent<TMPro.TextMeshPro>().text = "STUNNED!";
                stunnedShown = true;
            }
        }
        //Resets ability to show stun if enemy moves out of stunned state
        if (GetComponent<enemy_controller>().currentState != enemy_controller.State.Stunned)
        {
            stunnedShown = false;
        }
    }

    //Checks for collisions from enemy hitboxes
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("player_attackhitbox"))
        {
            if (collision.transform.position.x > transform.position.x)
            {
                collisionDir = -1;
            }
            else if (collision.transform.position.x < transform.position.x)
            {
                collisionDir = 1;
            }

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

    //Adds health, healthbar reacts to show this.
    //Instantiates health text, gives it health value to display
    public void AddHealth(int health)
    {
        currentHealth += health;

        var floattext = Instantiate(floatingHealthTextPrefab, transform.position + dmgTextOffset, Quaternion.identity);
        floattext.GetComponent<TMPro.TextMeshPro>().text = health.ToString();
    }


    //Subtracts damage calculated above from health, healthbar reacts to show this
    public void TakeDamage(int damage)
    {
        if (dmgCooldown <= 0 && currentHealth > 0)
        {
            currentHealth -= damage;

            var floattext = Instantiate(floatingDmgTextPrefab, transform.position + dmgTextOffset, Quaternion.identity);
            floattext.GetComponent<TMPro.TextMeshPro>().text = damage.ToString();
            floattext.GetComponent<TMPro.TextMeshPro>().faceColor = new Color(255, 18, 37);


            //Applies force to show direction hit from. Current bug where direction only updates on hit, so last direction may be incorrect if it is changed
            floattext.GetComponent<Rigidbody2D>().AddForce(new Vector2(collisionDir, 0), ForceMode2D.Impulse);
            dmgCooldown = dmgCooldownTargetTime;
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}

