using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropped_weapon : MonoBehaviour
{
    Vector3 spawnLoc;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {

        
    }
}
