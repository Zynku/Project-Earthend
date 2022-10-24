using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    //This script holds all the player data specified within. At the end of runtime it formats it to binary and saves it.
    //More complicated Unity specific data types can be broken down into strings or floats to allow for Serialization.
    //Whenever you add a new variable to be saved, make sure it is added to SavegameManager under LoadPlayer() as well

    public float money;
    public int level;
    public float[] position;
    public int health;


    public PlayerData (Charcontrol charcontrol, Charpickup_inventory inventory, Charhealth charhealth)
    {
        money = GameManager.instance.Player.GetComponent<Charpickup_inventory>().money;

        level = charhealth.level;

        position = new float[3];
        position[0] = charcontrol.transform.position.x;
        position[1] = charcontrol.transform.position.y;
        position[2] = charcontrol.transform.position.z;

        health = charhealth.currentHealth;
    }

}
