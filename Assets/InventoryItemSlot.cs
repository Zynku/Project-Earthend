using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemSlot : MonoBehaviour
{
    public Image icon;
    public ItemScriptable item;
    public TextMeshProUGUI amountText;
    public TextMeshProUGUI nameText;
    public Charpickup_inventory inventory;

    private void Start()
    {
        amountText.enabled = false;
        nameText.enabled = false;
        amountText.text = "You shouldn't be able to see this";
        nameText.text = "You shouldn't be able to see this";
        inventory = Charpickup_inventory.instance;
    }

    public void AddItem(ItemScriptable newItem)
    {
        item = newItem;

        icon.enabled = true;
        amountText.enabled = true;
        nameText.enabled = true;
        icon.sprite = item.Icon;
        amountText.text = newItem.amountHas.ToString();
        nameText.text = newItem.name.ToString();
        //Debug.Log("Item added");
    }

    public void ClearSlot()
    {
        if (item != null)
        {
            item.amountHas = 0;
            inventory.items.Remove(item);

            item = null;
            icon.sprite = null;
            icon.enabled = false;
            amountText.enabled = false;
            nameText.enabled = false;
            Debug.Log("Item cleared");
        }
    }

    public void SplitSlot()
    {
        if (item != null)
        {
            //Make two more items, set the amount of each to half of what you have, delete current item
            inventory.AddItem(item);
        }
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
        }
    }
}
