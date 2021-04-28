using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearInventory()
    {
        //Calls clear inventory in the Charpickup_Inventory script
        Charpickup_inventory.instance.ClearInventory();
    }
}
