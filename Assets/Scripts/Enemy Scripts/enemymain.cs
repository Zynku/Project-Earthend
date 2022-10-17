using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemymain : MonoBehaviour  //This class is reponsible for everything any enemy might need. This should be able to be attached to anything and not affect it negatively
{
    [Foldout("Ranges", true)]
    public float playerRadius;

    [Foldout("Static Variables", true)]
    public Vector2 playerPos;
    public float insideRadiusDir;
    public bool playerInsideRadius;
    public float playerDist;    //How far the player is from this enemy

    [Foldout("Health Variables", true)]
    public bool canTakeDamage;
    public int maxHealth;
    public int currentHealth;
    public int damageDoneToMeMax;
    public int damageDoneToMeMin;
    public int damageDoneToMe;
    [ReadOnly] public float dmgCooldown;
    public float dmgCooldownTargetTime = 0.1f;
    public GameObject lastDmgSource;
    [HideInInspector] public float collisionDir = 1f;
    public GameObject floatingDmgTextPrefab;
    public GameObject floatingHealthTextPrefab;
    public Vector3 dmgTextOffset;

    public delegate void SpawnOrActivateEnemy();
    public SpawnOrActivateEnemy spawnOrActivate;
    public delegate void EnemyGotHit();
    public EnemyGotHit enemyBeenHit;
    public delegate void EnemyDefeated();
    public static EnemyDefeated defeated;
    public bool enemyDefeated;



    GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameManager.instance.Player;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInRadius();
        playerPos = Player.transform.position;

        if (playerDist < playerRadius) { playerInsideRadius = true; }
        else { playerInsideRadius = false; }

        //Manages damage cooldown timer
        dmgCooldown -= Time.deltaTime;
        if (dmgCooldown < 0) { dmgCooldown = 0; }

        //Stops health from overflowing or underflowing
        if (currentHealth > maxHealth) { currentHealth = maxHealth; }
        if (currentHealth < 0) { currentHealth = 0; }
    }

    public void PlayerInRadius()
    {
        insideRadiusDir = Mathf.Sign(-transform.position.x + playerPos.x);
        playerDist = Mathf.Abs(playerPos.x - transform.position.x);
    }

    //Checks for collisions from enemy hitboxes
    public void OnTriggerEnter2D(Collider2D collision)
    {
        //Charattacks is responsible for letting the enemy know it has been hit. Make sure tags and layers are correct.
    }

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

            if (lastDmgSource.transform.position.x < transform.position.x) //Been hit from left
            {
                collisionDir = -1;
            }

            if (lastDmgSource.transform.position.x > transform.position.x) //Been hit from right
            {
                collisionDir = 1;
            }
            enemyBeenHit?.Invoke();

            dmgCooldown = dmgCooldownTargetTime;
        }
    }

    public void CreateFloatingText(string text, Color color)
    {
        var floattext = Instantiate(floatingDmgTextPrefab, transform.position + dmgTextOffset, Quaternion.identity);
        floattext.GetComponent<TMPro.TextMeshPro>().text = text;
        floattext.GetComponent<TMPro.TextMeshPro>().faceColor = color;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    private void OnDrawGizmos()
    {
        //Sense player radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerRadius);
    }
}
