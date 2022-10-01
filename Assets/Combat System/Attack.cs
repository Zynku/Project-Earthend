using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack
{
    public string attackName;
    public int attackDamage;
    public AttackType attackType;
    string name;

    public enum AttackType { LIGHT, LIGHT_HELD, HEAVY, HEAVY_HELD, RANGED, RANGED_HELD}


    public Attack(string atkname, int damage, AttackType type)
    {
        name = atkname;
        attackName = atkname;
        attackDamage = damage;
        attackType = type;
    }

}
