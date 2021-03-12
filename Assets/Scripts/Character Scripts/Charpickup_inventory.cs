using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charpickup_inventory : MonoBehaviour
{
    public float coins;
    
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
            coins++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
    }
}
