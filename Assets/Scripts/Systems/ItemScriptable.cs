using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory V2/Item" )]
public class ItemScriptable : ScriptableObject
{
    new public string name = "New Item";
    public int amount = 1;                  //In the future, there could be implementation for varying amounts, but for now we sticking with 1
    public int amountHas = 0;
    public int amountInStorage = 0;
    public Sprite Icon = null;
    [TextArea(15, 20)]
    public string description;

    public virtual void Use()
    {
        //Should be overwritten by each different type of item
        Debug.Log("Using " + name);
    }

}
