using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject InventoryUIObject;
    public Transform itemsParent;
    Charpickup_inventory inventory;
    InventoryItemSlot[] slots;
    List<ItemScriptable> allitems;
    // Start is called before the first frame update
    void Start()
    {
        inventory = Charpickup_inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
        inventory.onClearInventoryCallback += ClearInventory;
        slots = itemsParent.GetComponentsInChildren<InventoryItemSlot>();
        InventoryUIObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            InventoryUIObject.SetActive(!InventoryUIObject.activeSelf);
        }

        allitems = inventory.items;
    }

    void ClearInventory()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
        }

        for (int i = 0; i < allitems.Count; i++)
        {
            allitems[i].amountHas = 0;
        }
    }

    // Update is called once per frame
    void UpdateUI()
    {
        //Loops through all inventory slots
        for (int i = 0; i < slots.Length; i++)
        {
            //If there it finds an empty slot, it adds item to the end of the list of items
            if (i < inventory.items.Count)
            {
                slots[i].AddItem(inventory.items[i]);
            }
            /*else
            {
                Debug.Log("Clear slot");
                slots[i].ClearSlot();
            }*/
        }
    }
}
