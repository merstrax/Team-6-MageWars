using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AbilityStats : ScriptableObject
{
    [Header("Ability Info")]
    public string AbilityName;
    public float Cooldown;
    public int ChargesMax;
    public Ability.AbilityType DamageType;
    public Ability.ElementType ElementType;
    public Ability.StatusEffect ControlType;

    public float damageAmount;
    public float tickSpeed;
    public float duration;
}
