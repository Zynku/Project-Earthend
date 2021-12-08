using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject InventoryUIObject;
    public Transform itemsParent;

    public GameObject pickedUpPrefab;
    public List<GameObject> pickedUpTexts;
    public List<GameObject> offScreenTexts;

    public bool isTextLoActive;
    public bool isTextMidActive;
    public bool isTextHiActive;

    private float itemCoalesceTime;
    public float itemCoalesceTargetTime = 3f;

    private float pickedUpOnScreenTime;
    public float pickedUpOnScreenTargetTime = 3f;

    Charpickup_inventory inventory;
    InventoryItemSlot[] slots;
    List<ItemScriptable> allitems;
    public static InventoryUI instance;
    public InventoryUIHelper inventoryUIHelper;

    public bool beenSetup = false;

    #region Singleton
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found!");
            return;
        }
        instance = this;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        inventory = Charpickup_inventory.instance;
        //inventory.onItemChangedCallback += UpdateUI;
        inventory.onClearInventoryCallback += ClearInventory;

        slots = itemsParent.GetComponentsInChildren<InventoryItemSlot>();
        inventoryUIHelper = Gamemanager.instance.inventoryUIHelper;
        //InventoryUIObject.SetActive(false);
        beenSetup = true;
    }

    private void Update()
    {
        //ManagePickedUpTexts();

        if (pickedUpTexts.Count == 0)
        {
            isTextLoActive = false;
            isTextMidActive = false;
            isTextHiActive = false;
        }

        itemCoalesceTime -= Time.deltaTime;
        if (itemCoalesceTime < 0) { itemCoalesceTime = 0; }

        pickedUpOnScreenTime -= Time.deltaTime;
        if (pickedUpOnScreenTime < 0) { pickedUpOnScreenTime = 0; }

        allitems = inventory.items;
    }

    void ClearInventory()
    {

        for (int i = 0; i < allitems.Count; i++)
        {
            allitems[i].amountHas = 0;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
        }
    }


    public void UpdateUI()
    {
        if (!beenSetup) { Start(); }
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
                //Debug.Log("Clear slot");
                slots[i].ClearSlot();
            }*/
        }
    }
}