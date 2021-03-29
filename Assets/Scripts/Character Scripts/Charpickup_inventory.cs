using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Charpickup_inventory : MonoBehaviour
{
    public float money;
    public List<GameObject> weapons;
    
    // Start is called before the first frame update
    void Start()
    {
        weapons = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

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
        //If you come across a weapon and you interact, add weapon to weapons list, destroy weapon
        if (collision.CompareTag("dropped_weapon"))
        {
            //if (Input.GetAxis("Interact") > 0)
            if (Input.GetKeyDown(KeyCode.S))
            {
                weapons.Add(collision.transform.parent.gameObject);
                Destroy(collision.transform.parent.gameObject);
            }
            
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
    }
}
