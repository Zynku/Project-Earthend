using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbyplayer : MonoBehaviour
{
    Rigidbody2D rb2d;
    [Header ("Checks")]
    public bool destroyable = false;
    public bool applyInitialForce = false;
    public bool useParent = true;

    [Header ("Variables")]
    public bool hit;
    public float playerDir = 0;
    public float xForce = 0;
    public float yForce = 0;
    public float torqueForce = 0;
    public AudioClip hitGround, hitByPlayer, destroyed;
    AudioSource audiosource;
    public GameObject brokeObj;

    public int maxHealth;
    public int currentHealth;
    private int damageDoneToMeMax;
    private int damageDoneToMeMin;
    public int damageDoneToMe;
    [HideInInspector] public float dmgCooldown;
    [HideInInspector] public float dmgCooldownTargetTime = 0.1f;
    [HideInInspector] public float collisionDir = 1f;

    public int initialForce;

    // Start is called before the first frame update
    void Start()
    {
        if (useParent)
        {
            rb2d = GetComponentInParent<Rigidbody2D>();
            audiosource = GetComponentInParent<AudioSource>();
        }
        else
        {
            rb2d = GetComponent<Rigidbody2D>();
            audiosource = GetComponent<AudioSource>();
        }
        

        if (applyInitialForce)
        {
            rb2d.AddForce(new Vector2 (Random.Range(initialForce, -initialForce), 0f), ForceMode2D.Impulse);
        }
    }

    // Update is called once per frame
    void Update()
    {
        dmgCooldown -= Time.deltaTime;
        if (dmgCooldown < 0) { dmgCooldown = 0; }

        //Stops health from overflowing or underflowing
        if (currentHealth > maxHealth) { currentHealth = maxHealth; }
        if (currentHealth < 0) { currentHealth = 0; }

        if (currentHealth <= 0 && destroyable)
        {
            onDeactivate();
        }
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
            playerDir = Char_control.facingDir;
            hit = true;

            damageDoneToMeMax = Mathf.FloorToInt(collision.gameObject.GetComponentInParent<Charcontrol>().attackdamageMax);
            damageDoneToMeMin = Mathf.FloorToInt(collision.gameObject.GetComponentInParent<Charcontrol>().attackdamageMin);
            damageDoneToMe = (Random.Range(damageDoneToMeMax, damageDoneToMeMin));
            TakeDamage(damageDoneToMe);

            PlayPlayerHit();

            //Loads hit effect from resources folder
            Instantiate(Resources.Load<GameObject>("Sprites/Hit effects/Hit effect 1"), new Vector3(transform.position.x, transform.position.y, -1.33f), transform.rotation);
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

    private void onDeactivate()
    {
        GameObject clone;
        clone = Instantiate(brokeObj, transform.position, transform.rotation);

        audiosource.pitch = 1f;
        AudioSource.PlayClipAtPoint(destroyed, transform.position, 0.2f);

        Destroy(transform.parent.gameObject);

    }
}
