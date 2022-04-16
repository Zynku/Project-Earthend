using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchTriggerStayZone : MonoBehaviour
{
    public CrouchArea crouchArea;

    private void Start()
    {
        crouchArea = GetComponentInParent<CrouchArea>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
             crouchArea.playerInTriggerStayZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            crouchArea.playerInTriggerStayZone = false;
        }
    }
}
