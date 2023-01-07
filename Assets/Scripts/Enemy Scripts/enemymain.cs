using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemymain : MonoBehaviour  //This class is reponsible for everything any enemy might need. This should be able to be attached to anything and not affect it negatively
{
    GameManager gameManager;
    CombatEncounter combatEncounter;   //Every enemy won't have a combatencounter, so this sometimes return null;

    public bool activated;                      //Is the enemy fully ready to take dmg / engage with player?

    [Foldout("Variables", true)]
    public State currentState;
    public bool facePlayer;

    [Foldout("Ranges", true)]
    public float playerRadius;
    public float senseRadius;
    public float attackRadius;
    public float playerTooCloseRadius;
    public float wallCheckLength;
    public float groundCheckDistance;

    [Foldout("Checks", true)]
    [ReadOnly] public bool wallInfront;
    [ReadOnly] public bool isGrounded;
    [ReadOnly] public bool canFollowPlayer;
    [ReadOnly] public bool canAttackPlayer;
    [ReadOnly] public bool playerTooClose;
    [ReadOnly] public bool playerInsideRadius;
    [ReadOnly] public float insideRadiusDir;
    [ReadOnly] public float playerDist;    //How far the player is from this enemy
    [ReadOnly] public float facingDir;
    [ReadOnly] public Vector2 playerPos;

    [Foldout("Health Variables", true)]
    public bool canTakeDamage;
    [ReadOnly] public bool dead;
    public int maxHealth;
    [ReadOnly] public int currentHealth;
    public bool canHeal;
    public int healAmount;
    public float healDelay;                 //Time between individual heals
    public float healCooldownTargetTime;    //Time until healing kicks in
    [ReadOnly] public float healCooldownTime;


    [Foldout("Damage Variables", true)]
    public int damageDoneToMeMax;
    public int damageDoneToMeMin;
    public int damageDoneToMe;
    [ReadOnly] public float dmgCooldown;
    public float dmgCooldownTargetTime = 0.1f;                  //How long should invulnerability frames be after taking damage
    [ReadOnly] public GameObject lastDmgSource;
    [HideInInspector] public float collisionDir = 1f;
    [ReadOnly] public float atkCoolDownTimer = 0f;
    public float atkCoolDownTargetTime = 0.2f;                  //Cooldown between attacks
    public float attackdamageMax = 0;                           //Max amount of damage this enemy can do
    public float attackdamageMin = 0;                           //Min amount of damage this enemy can do

    [Foldout("Damage Numbers", true)]
    public GameObject floatingDmgTextPrefab;
    public GameObject floatingHealthTextPrefab;
    public Vector3 dmgTextOffset;

    [Header("Melee Variables")]
    public GameObject meleeHitbox1;   //The hitbox for attacks

    [Separator("Death Stuff")]
    public float deadKnockBX;
    public float deadKnockBY;
    public float despawnTime;

    public delegate void SpawnOrActivateEnemy();
    public SpawnOrActivateEnemy spawnOrActivate;
    public delegate void EnemyGotHit();
    public EnemyGotHit enemyBeenHit;
    public delegate void EnemyDefeated();
    public static EnemyDefeated defeated;
    public bool enemyDefeated;

    private Vector3 scale;  //Records this enemy's initial scale
    private bool scaleSet = false;

    GameObject Player;
    Rigidbody2D rb2d;
    Enemyhealth enemyHealth;

    public enum State
    {
        Spawning,
        Idle,
        Attacking,
        MovingToPlayer,
        MovingAwayFromPlayer,
        Stunned,
        Dodging,
        Waiting,
        Dead
    }

    // Start is called before the first frame update
    void Start()
    {
        Player = GameManager.instance.Player;
        gameManager = GameManager.instance;
        combatEncounter = GetComponentInParent<CombatEncounter>();
        currentHealth = maxHealth;

        rb2d = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponent<Enemyhealth>();
        if (meleeHitbox1 != null) { meleeHitbox1.SetActive(false); }
        currentState = State.Idle;
        atkCoolDownTimer = atkCoolDownTargetTime;

        if (scaleSet == false) { scale = transform.localScale; scaleSet = true; }
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = Player.transform.position;
        facingDir = Mathf.Sign(transform.localScale.x);
        insideRadiusDir = Mathf.Sign(-transform.position.x + playerPos.x);
        playerDist = Mathf.Abs(playerPos.x - transform.position.x);
        healCooldownTime -= Time.deltaTime;



        if (healCooldownTime <= 0)
        {
            healCooldownTime = 0;
            if (canHeal && currentHealth != maxHealth)
            {
                //AddHealth(healAmount);
                AddHealth(Random.Range((int)(healAmount / 0.5), (int)(healAmount * 1.5)));
                healCooldownTime = healDelay;
            }
        }

        if (playerDist < playerRadius) { playerInsideRadius = true; }
        else { playerInsideRadius = false; }

        //Manages damage cooldown timer
        dmgCooldown -= Time.deltaTime;
        if (dmgCooldown < 0) { dmgCooldown = 0; }

        //Stops health from overflowing or underflowing
        if (currentHealth > maxHealth) { currentHealth = maxHealth; }
        if (currentHealth < 0) { currentHealth = 0; }

        //In Sense Radius?
        if (Mathf.Abs(playerDist) < senseRadius) { canFollowPlayer = true; }
        else { canFollowPlayer = false; }

        //In attack radius
        if (Mathf.Abs(playerDist) < attackRadius) { canAttackPlayer = true; }
        else { canAttackPlayer = false; }

        //In too close radius
        if (Mathf.Abs(playerDist) < playerTooCloseRadius) { playerTooClose = true; }
        else { playerTooClose = false; }

        //Face the player
        if (facePlayer) { transform.localScale = new Vector2(insideRadiusDir * scale.x, scale.y); } //Transforms the enemy scale by whichever direction the player is


        if (enemyHealth?.currentHealth <= 0)
        {
            currentState = State.Dead;
        }

        switch (currentState)
        {
            case State.Spawning:
                break;
            case State.Idle:
                break;
            case State.Attacking:
                break;
            case State.MovingToPlayer:
                break;
            case State.MovingAwayFromPlayer:
                break;
            case State.Stunned:
                break;
            case State.Dodging:
                break;
            case State.Waiting:
                break;
            case State.Dead:
                break;
        }

        EnvironmentChecks();
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
    
    public void TakeDamage(int damage)  //Is called from Charattacks. Subtracts damage calculated above from health, healthbar reacts to show this
    {
        if (currentHealth > 0)
        {
            if(dmgCooldown > 0) { Debug.Log($"{gameObject} is on damage cooldown, attack not registered"); return; }//Enemy is still on cooldown, so don't apply attack.
            if (!activated) { Debug.LogWarning("Enemy is not ready! Please set activated to true in Enemymain script!"); return; }

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

            healCooldownTime = healCooldownTargetTime;
            dmgCooldown = dmgCooldownTargetTime;
        }
    }

    public void SetAsReady()
    {
        activated = true;
    }

    public void DefeatEnemy()
    {
        //activated = false;
        defeated?.Invoke();
        enemyDefeated = true;
        damageDoneToMe = 0;
        damageDoneToMeMax = 0;
        damageDoneToMeMin = 0;
        combatEncounter?.ChildEnemyDefeated(this);
    }

    public void CreateFloatingText(string text, Color color)
    {
        var floattext = Instantiate(floatingDmgTextPrefab, transform.position + dmgTextOffset, Quaternion.identity);
        floattext.GetComponent<TMPro.TextMeshPro>().text = text;
        floattext.GetComponent<TMPro.TextMeshPro>().faceColor = color;
    }

    public void EnemyDead()
    {

    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        enemyDefeated = false;
    }

    public void EnvironmentChecks()
    {
        //Front Wallcheck
        if (Physics2D.Raycast(new Vector2((transform.position.x), (transform.position.y + 0.15f)), (Vector2.right * facingDir), wallCheckLength, 1 << LayerMask.NameToLayer("Ground")))
        {
            wallInfront = true;
        }
        else
        {
            wallInfront = false;
        }

        //Grounded Check
        if (Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, 1 << LayerMask.NameToLayer("Ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void onMelee1Start()
    {
        meleeHitbox1.SetActive(true);
    }

    public void onMelee1End()
    {
        meleeHitbox1.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        //Sense player radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerRadius);

        //Ground Check Ray
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector2.down * groundCheckDistance);
/*
        //Wall Check Ray
        Gizmos.color = Color.green;
        Gizmos.DrawRay(new Vector2(transform.position.x, transform.position.y + 0.15f), Vector2.right * wallCheckLength * movingDir);
*/
        //Sense player radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, senseRadius);

        //Attack player radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        //Player too close radius
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, playerTooCloseRadius);

        //Line to player position
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, playerPos);
    }
}
