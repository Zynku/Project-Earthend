using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class devlab_energy_bullet : MonoBehaviour
{
    public GameObject firedFrom;
    public GameObject target;

    public float despawnTime = 5f;
    public float minDamage;
    public float maxDamage;

    public float speed;

    private void Update()
    {
        transform.Translate(new Vector3(-1 * speed * Time.deltaTime, 0, 0));
        Destroy(gameObject, despawnTime);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Walls":
                Destroy(gameObject);
                break;

            case "Ground":
                Destroy(gameObject);
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
