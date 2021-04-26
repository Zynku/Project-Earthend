using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Charpickup_inventory : MonoBehaviour
{
    public InventoryObject inventory;
    public float money;
    public List<GameObject> weapons;
    
    public Char_control char_control;
    
    // Start is called before the first frame update
    void Start()
    {
        weapons = new List<GameObject>();
        char_control = GetComponent<Char_control>();
    }

    // Update is called once per frame
    void Update()
    {
    //Get damage values from dropped weapon script on collision object, apply to charhealth
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

        var item = collision.GetComponent<Item>();
        if (item)
        {
            inventory.AddItem(item.item, 1);
            Destroy(collision.gameObject);
        }
    }

        private void OnTriggerStay2D(Collider2D collision)
    {
        //If you come across a weapon and you interact, add weapon to weapons list, destroy weapon
        if (collision.CompareTag("dropped_weapon"))
        {
            if (Input.GetAxisRaw("Interact")>0)
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

    private void OnApplicationQuit()
    {
        inventory.Container.Clear();
    }
}
