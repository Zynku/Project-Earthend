using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject InventoryUIObject;
    public Transform itemsParent;
    Charpickup_inventory inventory;
    InventoryItemSlot[] slots;
    ItemScriptable[] allitems;
    // Start is called before the first frame update
    void Start()
    {
        inventory = Charpickup_inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
        inventory.onClearInventoryCallback += ClearInventory;
        slots = itemsParent.GetComponentsInChildren<InventoryItemSlot>();
        allitems = FindObjectsOfType<ItemScriptable>();
        InventoryUIObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            InventoryUIObject.SetActive(!InventoryUIObject.activeSelf);
        }
        Debug.Log(allitems.Length);
    }

    void ClearInventory()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
        }

        for (int i = 0; i < allitems.Length; i++)
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
            //If there it finds an empty slot, it adds item
            if (i < inventory.items.Count)
            {
                slots[i].AddItem(inventory.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
