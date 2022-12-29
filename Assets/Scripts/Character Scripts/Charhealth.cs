using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.InputSystem;

public class Charhealth : MonoBehaviour
{
    Charcontrol charcontrol;
    Rigidbody2D rb2d;
    SpriteRenderer spriterenderer;
    ParticleSystem poison;
    public int maxHealth;
    public int currentHealth = 1;
    private int damageDoneToMeMax;
    private int damageDoneToMeMin;
    public int damageDoneToMe;
    
    public bool onDamageCoolDown;
    [HideInInspector] public float collisionDir = 1f;
    [ReadOnly] public float dmgCooldownTime;
    public float dmgCooldownTargetTime = 0.1f;
    public GameObject floatingDmgTextPrefab;
    public GameObject floatingHealthTextPrefab;
    public Vector3 dmgTextOffset;
    public Healthbar healthbarOver;
    public Healthbar healthbarUnder;
    public int level = 1;
    
    [ReadOnly] public bool playerDead;

    [Header("StatusEffects")]
    public bool poisoned;
    public int poisonDamage = 5;
    public float poisonTargetTime;
    private float poisonTimer;
    public float poisonTickTargetTime;
    private float poisonTickTimer;
    public bool frozen;
    public bool onFire;

    public delegate void gotHit();
    public static event gotHit Hit;

    private void Awake()
    {
        Debug.Log("Player has been spawned, whats up bitches");
    }

    // Start is called before the first frame update
    public void Start()
    {
        charcontrol = GetComponent<Charcontrol>();
        healthbarOver = GameManager.instance.ingame_UI.healthBarOver;
        healthbarUnder = GameManager.instance.ingame_UI.healthBarUnder;
        rb2d = GetComponent<Rigidbody2D>();
        spriterenderer = GetComponent<SpriteRenderer>();
        //poison = GetComponent<ParticleSystem>();

        //Invoke("SetHealthIntially", 0.2f);
        currentHealth = maxHealth;
        healthbarOver.SetHealth(maxHealth * 1);
        healthbarUnder.SetMaxHealth(maxHealth * 1);
        poisonTimer = poisonTargetTime;
    }

    public void SetHealthIntially()
    {
        Start();
        playerDead = false;
        healthbarOver.SetMaxHealth(maxHealth);
        healthbarUnder.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (dmgCooldownTime > 0) { dmgCooldownTime -= Time.deltaTime; onDamageCoolDown = true; }
        else { onDamageCoolDown = false; }

        //Stops health from overflowing or underflowing
        if (currentHealth > maxHealth) {currentHealth = maxHealth;}
        if (currentHealth < 0) { currentHealth = 0;}

        //healthbarOver.SetHealth(currentHealth);

        if (Keyboard.current.jKey.wasPressedThisFrame) { ResetHealth(true);}
        if (Keyboard.current.hKey.wasPressedThisFrame) { AddHealth(20); }
        if (Keyboard.current.kKey.wasPressedThisFrame) { TakeDamage(maxHealth); }
        if (Keyboard.current.lKey.wasPressedThisFrame) { TakeDamage(19); }

        if (poisoned) { Poisoned(poisonTargetTime, poisonDamage); }
        else { poisonTimer = poisonTargetTime; /*Reassign sprite color to initial here*/; }
    }

    private void FixedUpdate()
    {
        if (currentHealth <= 0 && !playerDead)
        {
            playerDead = true;
            OnDeath();
        }
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
            damageDoneToMeMax = Mathf.FloorToInt(collision.gameObject.GetComponentInParent<Enemy_controller>().attackdamageMax);
            damageDoneToMeMin = Mathf.FloorToInt(collision.gameObject.GetComponentInParent<Enemy_controller>().attackdamageMin);
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

        if (collision.gameObject.CompareTag("gas_cloud"))
        {
            Gascloudscript gascloudscript = collision.GetComponent<Gascloudscript>();
            switch (collision.GetComponent<Gascloudscript>().CloudType)
            {
                case Gascloudscript.cloudType.Poison:
                    poisonTargetTime = gascloudscript.poisonTime;
                    poisonDamage = gascloudscript.poisonDamage;
                    poisoned = true;
                    break;

                case Gascloudscript.cloudType.Freeze:
                    break;
            }
        }
    }
    //Subtracts damage calculated above from health, healthbar reacts to show this. Only executes when damage cooldown timer is 0 or less
    //Instantiates dmg text, gives it dmg value to display
    public void TakeDamage(int damage)
    {
        if (dmgCooldownTime <= 0 && currentHealth > 0)
        {
            dmgCooldownTime = dmgCooldownTargetTime;
            currentHealth -= damage;
            healthbarOver.SetHealth(currentHealth);
            healthbarUnder.SetHealth(currentHealth);

            var floattext = Instantiate(floatingDmgTextPrefab, transform.position + dmgTextOffset, Quaternion.identity);
            floattext.GetComponent<TMPro.TextMeshPro>().text = damage.ToString();
            //Applies force to show direction hit from.
            floattext.GetComponent<Rigidbody2D>().AddForce(new Vector2(collisionDir, 0), ForceMode2D.Impulse);
            Hit?.Invoke();
        }
    }

    //Adds health, healthbar reacts to show this.
    //Instantiates health text, gives it health value to display
    public void AddHealth(int health)
    {
        currentHealth += health;
        healthbarOver.SetHealth(currentHealth);
        healthbarUnder.SetHealth(currentHealth);

        var floattext = Instantiate(floatingHealthTextPrefab, transform.position + dmgTextOffset, Quaternion.identity);
        floattext.GetComponent<TMPro.TextMeshPro>().text = health.ToString();
    }

    //Adds max health, healthbar reacts to show this.
    //Instantiates health text, gives it max health value to display
    public void ResetHealth(bool showFloatText)
    {
        currentHealth = maxHealth;
        healthbarOver.SetHealth(maxHealth * 1);
        healthbarUnder.SetHealth(maxHealth * 1);

        if (showFloatText)
        {
            var floattext = Instantiate(floatingHealthTextPrefab, transform.position + dmgTextOffset, Quaternion.identity);
            floattext.GetComponent<TMPro.TextMeshPro>().text = maxHealth.ToString();
        }
    }

    public void Poisoned(float poisonTargetTime, int poisonDamage)
    {
        //End after a certain amount of time
        //var emission = poison.emission;
        poisonTimer -= Time.fixedDeltaTime;
        if (poisonTimer < 0) { poisonTimer = 0; }
        if (poisonTimer == 0) {poisonTimer = poisonTargetTime; spriterenderer.color = new Color(1, 1, 1, 1); poisoned = false; /*emission.enabled = false;*/ }

        poisonTickTimer -= Time.fixedDeltaTime;
        if (poisonTickTimer < 0) { poisonTickTimer = 0; }

        //If poison is active
        if (poisonTimer > 0)
        {
            //On every poison tick....
            if (poisonTickTimer == 0)
            {
                //Takes poison damage from health
                currentHealth -= poisonDamage;
                healthbarOver.SetHealth(currentHealth);
                healthbarUnder.SetHealth(currentHealth);
                poisonTickTimer = poisonTickTargetTime;

                //Instantiate damage numbers text on every tick
                var floattext = Instantiate(floatingDmgTextPrefab, transform.position + dmgTextOffset, Quaternion.identity);
                floattext.GetComponent<TMPro.TextMeshPro>().text = poisonDamage.ToString();
                floattext.GetComponent<TMPro.TextMeshPro>().color = new Color(.43f, .76f, .18f);

                //Changes sprite color hue to a sickly green
                spriterenderer.color = new Color(.58f, .74f, .48f, 1);

                //Start poison particles
                //emission.enabled = true;
                poisonTickTimer = poisonTickTargetTime;
            }
        }
        
        //Sprite effects showing poison
    }

    public void Frozen()
    {

    }

    public void OnFire()
    {

    }

    public void OnDeath()
    {
        charcontrol.currentState = Charcontrol.State.Dead;
        rb2d.velocity = new Vector2(0, 0);
        StartCoroutine(GameManager.instance.respawn_Menu_Manager.ShowRespawnOptions());
    }
}
