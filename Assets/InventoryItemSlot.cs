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
        amountText.gameObject.SetActive(false);
        nameText.gameObject.SetActive(false);
        amountText.text = "You shouldn't be able to see this";
        nameText.text = "You shouldn't be able to see this";
        inventory = Charpickup_inventory.instance;
    }

    public void AddItem(ItemScriptable newItem)
    {
        item = newItem;

        Debug.Log("Activating and setting everything else");
        ActivateAmountandName();
        amountText.text = newItem.amountHas.ToString();
        nameText.text = newItem.name.ToString();
        icon.enabled = true;
        icon.sprite = item.Icon;
        //Debug.Log("Item added");
    }

    void ActivateAmountandName()
    {
        Debug.Log("Activating name and amount");
        amountText.gameObject.SetActive(true);
        nameText.gameObject.SetActive(true);
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
            amountText.gameObject.SetActive(false);
            nameText.gameObject.SetActive(false);
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
