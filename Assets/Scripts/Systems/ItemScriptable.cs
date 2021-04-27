using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory V2/Item" )]
public class ItemScriptable : ScriptableObject
{
    new public string name = "New Item";
    public int amount = 1;                  //In the future, there could be implementation for varying amounts, but for now we sticking with 1
    public Sprite Icon = null;
    public bool isDefaultItem = false;

    public virtual void Use()
    {
        //Should be overwritten by each different type of item
        Debug.Log("Using " + name);
    }

}
