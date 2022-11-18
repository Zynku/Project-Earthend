using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item_in_inventory
{
    public string name = "New Item";
    public int amount = 1;                  //In the future, there could be implementation for varying amounts, but for now we sticking with 1
    public int amountHas = 0;
    public int amountInStorage = 0;
    public Sprite Icon = null;
    [TextArea(2,10)]
    public string description;

    public Item_in_inventory(string i_name, int i_amountHas, int i_amountInStorage, Sprite i_icon, string i_description)
    {
        name = i_name;
        amountHas = i_amountHas;
        amountInStorage = i_amountInStorage;
        Icon = i_icon;
        description = i_description;
    }
}
