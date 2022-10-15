using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterHitbox : MonoBehaviour
{
    CombatEncounter parentScript;

    private void Start()
    {
        parentScript = GetComponentInParent<CombatEncounter>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && gameObject.name == "Start Collider")
        {
            parentScript.playerInStartCollider = true;
        }
        else if (collision.CompareTag("Player") && gameObject.name == "End Collider")
        {
            parentScript.playerInExitCollider = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && gameObject.name == "Start Collider")
        {
            parentScript.playerInStartCollider = false;
        }
        else if (collision.CompareTag("Player") && gameObject.name == "End Collider")
        {
            parentScript.playerInExitCollider = false;
        }
    }
}
