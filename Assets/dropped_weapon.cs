using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropped_weapon : MonoBehaviour
{
    public int damageMax = 69;
    public int damageMin = 69;
    public int fireChance = 69;
    public int freezeChance = 69;
    public int poisonChance = 69;

    private void Start()
    {
        //transform.rotation = Quaternion.Euler(Random.Range(20,-20), 0f, 0f);
        //spawnLoc = transform.position;
    }


    private void FixedUpdate()
    {
       //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0,0,45)), Time.deltaTime);
       //transform.position = spawnLoc;
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Input.GetAxisRaw("Interact") > 0)
            {
                
            }
        }
        
    }
}
