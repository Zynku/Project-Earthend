using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharMeleeHitBox : MonoBehaviour
{
    public GameObject Player;
    public Collider2D myCollider;

    private void Start()
    {
        Player = GetComponentInParent<Charcontrol>().gameObject;
        myCollider = GetComponent<BoxCollider2D>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("target_dummy"))
        {
            target_dummy_controller dummyScript = collision.GetComponent<target_dummy_controller>();
            if (collision.transform.position.x > transform.position.x)
            {
                //Collision to the left
            }
            else if (collision.transform.position.x < transform.position.x)
            {
                //Collision to the right
            }

            //Gets max and min attack values from enemy script, returns random value between them, applies damage
            dummyScript.damageDoneToMeMax = Mathf.FloorToInt(Player.GetComponentInParent<Charcontrol>().attackdamageMax);
            dummyScript.damageDoneToMeMin = Mathf.FloorToInt(Player.GetComponentInParent<Charcontrol>().attackdamageMin);
            dummyScript.damageDoneToMe = (Random.Range(dummyScript.damageDoneToMeMax, dummyScript.damageDoneToMeMin));
            dummyScript.TakeDamage(dummyScript.damageDoneToMe);
            dummyScript.BeenHit();

            GameManager.instance.Particle_Manager.PlayHitParticles(collision.gameObject.GetComponent<Collider2D>().ClosestPoint(transform.position));
            StartCoroutine(GameManager.instance.MeleeHitStop());
        }
    }
}
