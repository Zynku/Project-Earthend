using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemInteractable : MonoBehaviour
{
    public ItemScriptable item;
    Charpickup_inventory inventory;

    private void Start()
    {
        inventory = GameManager.instance.Player.GetComponent<Charpickup_inventory>();
    }
    public void Interact()
    {

        if (GetComponentInChildren<ItemPickupable>().canBePickedUp == true)
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
        bool wasPickedUp = inventory.AddItem(item);

        if (wasPickedUp)
        {
            Destroy(gameObject);
        }
    }
}
