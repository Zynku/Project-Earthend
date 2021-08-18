using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Transform interactionTransform;

    public virtual void Interact()
    {
        //This method is meant to be overridden
    }

    public void OnDrawGizsmosSelected()
    {
        if (interactionTransform == null)
        {
            interactionTransform = transform;
        }

        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(interactionTransform.position, radius);
    }
}
