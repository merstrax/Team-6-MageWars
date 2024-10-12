using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class enemyStats : ScriptableObject
{
    [Header("Enemy Stats")]
    public float healthRegen;
    public float healthBase;
    public float damageBase;
    public float defenseBase;
    public float speedBase;
    public float critChanceBase;
    public float critDamageBase;
    public float cooldownBase;

    [Header("Aggro and Roaming")]
    public float aggroRange;
    public float kiteRange;
    public int viewAngle;

    [Header("AI Abilities")]
    public Ability[] abilityPassive;
    public Ability[] abilities;
    public float abilityRate;

    [Range(0f, 1f)] public float dropChance;

}
