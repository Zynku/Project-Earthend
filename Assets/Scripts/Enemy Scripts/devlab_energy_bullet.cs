using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class devlab_energy_bullet : MonoBehaviour
{
    SpriteRenderer spriterenderer;
    ParticleSystem hitparticles;

    public GameObject firedFrom;
    public GameObject target;

    public bool travelling = true;
    private bool particlesTriggered;
    public float despawnTime = 5f;
    public float minDamage;
    public float maxDamage;

    public float speed;

    private void Start()
    {
        spriterenderer = GetComponent<SpriteRenderer>();
        hitparticles = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (travelling) { transform.Translate(new Vector3(-1 * speed * Time.deltaTime, 0, 0)); }
        Destroy(gameObject, despawnTime);
    }

    public void CreateEffects()
    { 
        travelling = false;
        spriterenderer.enabled = false;
        hitparticles.Play();
        Destroy(gameObject, 3f);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"{collision} on {collision.gameObject}");
        switch (collision.tag)
        {
            case "Walls":
                //Destroy(gameObject);
                if (!particlesTriggered) { particlesTriggered = true; CreateEffects(); }
                break;

            case "Ground":
                //Destroy(gameObject);
                if (!particlesTriggered) { particlesTriggered = true; CreateEffects(); }
                break;

            case "Player":
                int randomDamage = Mathf.FloorToInt(Random.Range(minDamage, maxDamage));
                collision.GetComponent<Charhealth>().TakeDamage(randomDamage, true);
                break;

            case "enemy":
                int randomDamage2 = Mathf.FloorToInt(Random.Range(minDamage, maxDamage));
                Enemymain enemyScript = collision.GetComponentInParent<Enemymain>();
                enemyScript.damageDoneToMeMax = randomDamage2;
                enemyScript.damageDoneToMeMin = randomDamage2;
                enemyScript.damageDoneToMe = randomDamage2;
                enemyScript.lastDmgSource = gameObject;
                enemyScript.TakeDamage(randomDamage2);
                break;

            default:
                break;
        }
    }
}
