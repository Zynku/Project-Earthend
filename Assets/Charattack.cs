using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charattack : MonoBehaviour
{
    public GameObject Melee1;
    public bool Attacking;

    // Start is called before the first frame update
    void Start()
    {
        Melee1.SetActive(false);
    }

    // Update is called once per frame
    // If D is pressed, sets Attacing to true
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        { 
            Attacking = true;
        }
    }

    //Used by Animation Events in Melee Animation. On Event 1 activates hitbox, on Event 2 deactivates hitbox
    public void OnMelee1Start()
    {
        Melee1.SetActive(true);
    }

    public void OnMelee1End()
    {
        Attacking = false;
        Melee1.SetActive(false);
    }
}
