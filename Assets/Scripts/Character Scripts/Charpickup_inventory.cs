using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Charpickup_inventory : MonoBehaviour
{
    public float money;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("coin_collectable"))
        {
            Destroy(collision.gameObject);
            Destroy(collision.transform.parent.gameObject);
            money += collision.GetComponentInParent<coinscript>().coinValue;
        }

        if (collision.CompareTag("heart_collectable"))
        {
            Destroy(collision.gameObject);
            Destroy(collision.transform.parent.gameObject);
            var heartValue = collision.gameObject.GetComponent<Heartscript>().heartValue;
            GetComponentInParent<Charhealth>().AddHealth(Mathf.FloorToInt(heartValue));
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
    }
}
