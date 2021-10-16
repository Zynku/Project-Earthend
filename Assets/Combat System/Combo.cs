using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Combo
{
    public string comboName = "null";
    public string moveName;
    public string animationName;
    public float comboChainTimeLocation;    //The time at which, if there is another combo to perform, it will cut this animation at to play the next.
    public bool endOfComboChain;
    public List<Attack> attackList; //Contains the list of attacks required to execute this combo
}
