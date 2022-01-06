using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack
{
    public string attackName;
    public int attackDamage;
    public AttackType attackType;

    public enum AttackType { LIGHT, LIGHT_HELD, HEAVY, HEAVY_HELD, RANGED, RANGED_HELD}


    public void SetupAttack(string name, int damage, AttackType type)
    {
        attackName = name;
        attackDamage = damage;
        attackType = type;
    }

}
