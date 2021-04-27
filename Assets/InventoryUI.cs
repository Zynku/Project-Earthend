using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject InventoryUIObject;
    public Transform itemsParent;
    Charpickup_inventory inventory;
    InventoryItemSlot[] slots;
    // Start is called before the first frame update
    void Start()
    {
        inventory = Charpickup_inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
        slots = itemsParent.GetComponentsInChildren<InventoryItemSlot>();
        InventoryUIObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            InventoryUIObject.SetActive(!InventoryUIObject.activeSelf);
        }
    }

    // Update is called once per frame
    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
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
