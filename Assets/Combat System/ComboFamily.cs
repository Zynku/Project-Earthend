using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[System.Serializable]
public class ComboFamily
{
    public string cFamilyName;
    public List<Combo> familyList;

    public ComboFamily(string name)
    {
        cFamilyName = name;
        familyList = new List<Combo>();
    }
}
