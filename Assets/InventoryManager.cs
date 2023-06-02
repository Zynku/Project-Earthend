using MyBox.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour
{
    Charpickup_inventory inventory;

    List<VisualElement> inventorySlots;     //The visual elements found in the IHUI UI document inventory
    public List<SlotInformation> slotInformations = new(); //A readable version of the info the scriptable items have on them. Class is defined in this document at the bottom
    int slotNumber; //Keeps track of how many slots were created at start. Also is used as the slotNumber;

    private void Start()
    {
        inventory = GameManager.instance.Player.GetComponent<Charpickup_inventory>();

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        inventorySlots = root.Query<VisualElement>("IHUI-sprite").ToList();

        foreach (var invSlot in inventorySlots)
        {
            slotNumber++;
            VisualElement sprite = invSlot.Q<VisualElement>("IHUI-sprite");
            TextElement itemCounter = invSlot.Q<TextElement>("item-counter");
            CreateInventorySlots(slotNumber, invSlot, sprite, itemCounter);
        }
    }

    public void CreateInventorySlots(int slotNumber, VisualElement inventorySlot, VisualElement sprite, TextElement amountCounter)
    {
        SlotInformation newSlotInfo = new(slotNumber, inventorySlot, sprite, amountCounter, "No invSlot", 0);
        slotInformations.Add(newSlotInfo);
    }

    public void FillNewSlot(Item_in_inventory inventoryItem) //Finds the first empty slot and fills out its information
    {
        SlotInformation slotToAssign = ReturnFirstEmptySlotInfo();
        slotToAssign.myUIItemSprite.style.backgroundImage = new StyleBackground(inventoryItem.Icon);
        slotToAssign.myUIText.text = inventoryItem.amountHas.ToString();
        slotToAssign.myItem = inventoryItem;
        slotToAssign.itemName = inventoryItem.name;
        slotToAssign.itemAmount = inventoryItem.amount;
        slotToAssign.containsItem = true;
    }

    public void UpdateSlot(Item_in_inventory inventoryItem) //Updates the appropriate inventory slot visually under InventoryManager in the inspector
    {
        for (int i = 0; i < slotInformations.Count; i++)
        {
            if (slotInformations[i].myItem.name == inventoryItem.name)
            {
                //This slot information contains the slot that's holding this invSlot
                slotInformations[i].itemAmount = inventoryItem.amountHas;
                slotInformations[i].myUIText.text = inventoryItem.amountHas.ToString();
                return;
            }
        }
        Debug.LogWarning("Could not find an empty slot to assign data");
    }

    public SlotInformation ReturnFirstEmptySlotInfo()
    {
        for (int i = 0; i < slotInformations.Count; i++)
        {
            if (!slotInformations[i].containsItem)  //Returns the first slot that doesnt contain an invSlot
            {
                return slotInformations[i];
            }
        }
        Debug.LogWarning("Could not find an empty slot to assign data");
        return null;
    }
}

[System.Serializable]
public class SlotInformation    //This class is very similar to Item_in_inventory, except that it has fields for the UI components and the slot number it represents
{
    //---------UI Components-----------------------------------------------------------------------
    public VisualElement myUISlot;
    public VisualElement myUIItemSprite;
    public TextElement myUIText;
    //---------Variables---------------------------------------------------------------------------
    [HideInInspector] public string name;
    [HideInInspector] public int slotNumber = -1; 
    public string itemName;
    public int itemAmount;
    public Item_in_inventory myItem;
    public bool containsItem;

    public SlotInformation(int invSlot, VisualElement inventoryUISlot, VisualElement sprite, TextElement amountCounter, string nameOfItem, int amountOfItem)
    {
        slotNumber = invSlot;
        myUISlot = inventoryUISlot;
        myUIItemSprite = sprite;
        myUIText = amountCounter;
        itemName = nameOfItem;
        itemAmount = amountOfItem;

        name = $"Slot number {slotNumber}";
    }
}