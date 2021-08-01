using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject InventoryUIObject;
    public Transform itemsParent;

    public TextMeshProUGUI itemPickedUpText;
    private Animator itemPickedUpAnim;

    private float itemCoalesceTime;
    public float itemCoalesceTargetTime = 3f;

    private float pickedUpOnScreenTime;
    public float pickedUpOnScreenTargetTime = 3f;

    Charpickup_inventory inventory;
    InventoryItemSlot[] slots;
    List<ItemScriptable> allitems;
    public static InventoryUI instance;

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
        inventory.onItemChangedCallback += UpdateUI;
        inventory.onClearInventoryCallback += ClearInventory;

        itemPickedUpAnim = itemPickedUpText.GetComponent<Animator>();
        slots = itemsParent.GetComponentsInChildren<InventoryItemSlot>();

        InventoryUIObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            InventoryUIObject.SetActive(!InventoryUIObject.activeSelf);
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

    public void ShowPickedUpText(ItemScriptable item)
    {
        float itemamount = item.amount;
        string itemname = item.name.ToString();

        //Pickedup text is on screen for PickedUpOnScreenTargetTime


        /*if (itemCoalesceTime <= 0) //If it has been recent enough after picking up the same item
        {
            //Just add the item number to what's already on screen
            itemPickedUpText.text = (itemamount + " x " + itemname);
            itemCoalesceTime = itemCoalesceTargetTime;
        }
        else
        {
            //Otherwise create a new name + number
            itemPickedUpText.text = (itemamount + " x " + itemname);
        }
        */
    }
}
