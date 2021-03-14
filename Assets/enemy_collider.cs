using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_collider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("truuuu");
            return true;
        }
        else
        {
            Debug.Log("false");
            return false;
        }
    }
}
