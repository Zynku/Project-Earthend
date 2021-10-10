using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Combo
{
    public string comboName;
    public string moveName;
    public List<Attack> attackList; //Contains the list of attacks required to execute this combo
}
