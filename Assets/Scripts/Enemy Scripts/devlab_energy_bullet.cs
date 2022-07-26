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
        if (collision.CompareTag("Walls") || collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
