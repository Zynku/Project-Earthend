using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Interactable
{
    public ItemScriptable item;
    
    public override void Interact()
    {
        base.Interact();

        Pickup();
    }

    public void Pickup()
    {
        bool wasPickedUp = Charpickup_inventory.instance.AddItem(item);

        if (wasPickedUp)
        {
            Destroy(gameObject);
        }
    }
}
