using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemInteractable : MonoBehaviour
{
    public ItemScriptable item;

    public void Interact()
    {

        if (GetComponent<ItemPickupable>().canBePickedUp == true)
        {
            Pickup();
        }
    }

    /*public override void Interact()
    {
        base.Interact();

        if (GetComponent<ItemPickupable>().canBePickedUp == true)
        {
            Debug.Log("Interacting...");
            Pickup();
        }
    }*/

    
    public void Pickup()
    {
        bool wasPickedUp = Charpickup_inventory.instance.AddItem(item);

        if (wasPickedUp)
        {
            Destroy(gameObject);
        }
    }
}
