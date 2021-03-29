using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class death_plane : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponentInParent<Charhealth>().currentHealth = 0;
        }
    }
}
