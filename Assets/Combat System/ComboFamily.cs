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
[System.Serializable]
public class ComboFamilyType
{
    public string cFamilyTypeName;
    public List<Combo> familyTypeList;

    public ComboFamilyType(string name)
    {
        cFamilyTypeName = name;
        familyTypeList = new List<Combo>();
    }
}
