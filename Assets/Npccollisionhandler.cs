using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npccollisionhandler : MonoBehaviour
{
    Npcscript Npcscript;

    // Start is called before the first frame update
    void Start()
    {
        Npcscript = GetComponentInParent<Npcscript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Npcscript.playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Npcscript.playerInRange = false;
        }
    }
}
