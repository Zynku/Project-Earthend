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

    private void OnTriggerStay2D(Collider2D collision)
    {
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
                collision.GetComponent<Charhealth>().TakeDamage(randomDamage);
                break;

            default:
                break;
        }
    }
}
