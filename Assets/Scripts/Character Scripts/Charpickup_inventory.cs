using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Charpickup_inventory : MonoBehaviour
{
    public List<ItemScriptable> items = new List<ItemScriptable>();
    public int inventorySpace = 40;
    public float money;
    public static Charpickup_inventory instance;
    private Char_control char_control;

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

    public delegate void onItemChanged();
    public onItemChanged onItemChangedCallback;

    // Start is called before the first frame update
    void Start()
    {
        char_control = GetComponent<Char_control>();
    }

    // Update is called once per frame
    void Update()
    {
    //Get damage values from dropped weapon script on collision object, apply to charhealth
    }

    public bool AddItem(ItemScriptable item)
    {
        if (items.Count >= inventorySpace)
        {
            Debug.Log("Not enough room...");
            return false;
        }

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].name == item.name)
            {
                ++items[i].amount;
                return true;
            }
        }
        items.Add(item);
        return true;
    }

    public void RemoveItem(ItemScriptable item)
    {
        items.Remove(item);
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //If you come across a coin, pick it up, destroy the coin, add coinvalue to player inv
        if (collision.CompareTag("coin_collectable"))
        {
            Destroy(collision.gameObject);
            Destroy(collision.transform.parent.gameObject);
            money += collision.GetComponentInParent<coinscript>().coinValue;
        }
        //If you come across a heart, pick it up, destroy the heart, add health value to health
        if (collision.CompareTag("heart_collectable"))
        {
            Destroy(collision.gameObject);
            Destroy(collision.transform.parent.gameObject);
            var heartValue = collision.gameObject.GetComponent<Heartscript>().heartValue;
            GetComponentInParent<Charhealth>().AddHealth(Mathf.FloorToInt(heartValue));
        }
        if (collision.CompareTag("item_collectable"))
        {
            Interactable interactable = collision.gameObject.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactable.Interact();
                //This is how an item is added to the inventory. Interact() is called here, from Interactable script, but is overidden in ItemPickup by Pickup()
                //Pickup destroys the gameObject and calls the AddItem() function from this script, which, if there is enough room, invokes onItemChangedCallback 
                //to add the item to the inventory UI, to which UpdateUI is subscribed in InventoryUI. UpdateUI adds the item to the [i]th place via 
                //AddItem from InventoryItemSlot script.
            }
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //If you come across a weapon and you interact, add weapon to weapons list, destroy weapon
        if (collision.CompareTag("dropped_weapon"))
        {
            if (Input.GetAxisRaw("Interact") > 0)
            {
                //weapons.Add(collision.transform.parent.gameObject);
                char_control.attackdamageMax = collision.GetComponentInParent<dropped_weapon>().damageMax;
                char_control.attackdamageMin = collision.GetComponentInParent<dropped_weapon>().damageMin;
                char_control.SetMeleeSprite(collision.GetComponentInParent<SpriteRenderer>().sprite);
                Destroy(collision.transform.parent.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
    }

}
