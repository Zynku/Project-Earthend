using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavegameManager : MonoBehaviour
{
    GameObject Player;
    Charcontrol Charcontrolscript;
    Charpickup_inventory Charpickup_Inventoryscript;
    Charhealth Charhealthscript;

    private void Awake()
    {
        Player = GameObject.FindWithTag("Player");
        Charcontrolscript = Player.GetComponent<Charcontrol>();
        Charpickup_Inventoryscript = Player.GetComponent<Charpickup_inventory>();
        Charhealthscript = Player.GetComponent<Charhealth>();
    }

    //Public method to be called every time:
    //player exits to Main Menu
    //player selects save game
    //player hits a checkpoint
    public void SavePlayer()
    {
        SaveSystem.SavePlayer(Charcontrolscript, Charpickup_Inventoryscript, Charhealthscript);
    }

    public void LoadPlayer()
    {
        //Whenever you add a new variable to be saved, make sure it is added to PlayerData as well
        PlayerData data = SaveSystem.LoadPlayer();

        Charpickup_inventory.instance.money = data.money;

        Charhealthscript.level = data.level;

        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];
        Charcontrolscript.transform.position = position;

        Charhealthscript.currentHealth = data.health;
    }

    //Saves game every time the game closes
    private void OnApplicationQuit()
    {
        SavePlayer();
    }

    //Saves game every time the game pauses (for mobile)
    private void OnApplicationPause(bool pause)
    {
        SavePlayer();
    }

    //Loads game every time the player hits continue on Main Menu
    //Call from button

    //Loads game every time player selects Load game in Menu
    //Call from button

}
