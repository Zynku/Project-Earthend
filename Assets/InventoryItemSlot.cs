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

    public void AddItem(ItemScriptable newItem)
    {
        item = newItem;

        icon.enabled = true;
        icon.sprite = item.Icon;
        amountText.text = newItem.amount.ToString("n0");
    }

    public void ClearSlot()
    {
        item = null;

        icon.sprite = null;
        icon.enabled = false;
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
        }
    }
}
