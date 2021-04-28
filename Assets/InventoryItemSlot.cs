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

    private void Start()
    {
        amountText.enabled = false;
        nameText.enabled = false;
        amountText.text = "WHAT ARE YA DOING JIMBO";
        nameText.text = "WHAT ARE YA DOING JIMBO";
    }

    public void AddItem(ItemScriptable newItem)
    {
        item = newItem;

        icon.enabled = true;
        icon.sprite = item.Icon;
        amountText.text = newItem.amountHas.ToString("n0");
        nameText.text = newItem.name.ToString();
        amountText.enabled = true;
        nameText.enabled = true;
    }

    public void ClearSlot()
    {
        item = null;

        icon.sprite = null;
        icon.enabled = false;
        amountText.enabled = false;
        nameText.enabled = false;
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
        }
    }
}
