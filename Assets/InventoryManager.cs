using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    Charpickup_inventory inventory;

    private void Start()
    {
        inventory = GameManager.instance.Player.GetComponent<Charpickup_inventory>();
    }
    public void ClearInventory()
    {
        //Calls clear inventory in the Charpickup_Inventory script
        inventory.ClearInventory();
    }
}
