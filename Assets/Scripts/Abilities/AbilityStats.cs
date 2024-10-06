using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AbilityStats : ScriptableObject
{
    [Header("Ability Info")]
    public string AbilityName;
    public string AbilityDescription;
    public int AbilityID;
    public int AbilityFlag;
    public ElementType ElementType;
    public AbilityType AbilityType;
    public CastType CastType;
    public float Cooldown;

    [Header("Effect Info")]
    public EffectType EffectType;
    public EffectStatusType StatusType;
    public EffectAttributeFlags AttributeFlags;
    public EffectElementFlags ElementFlags;
    public EffectTriggerFlags TriggerFlags;
    public EffectModifierType ModifierType;
    public float EffectAmount;
    public float EffectTriggerChance;
    public int EffectAbilityID;
    public int EffectAbilityFlag;
    public int EffectStackMax;
    public float EffectDuration;
    public float EffectTickSpeed;

    [Header("Resource Info")]
    public ResourceType ResourceType;
    public int ResourceMax;
    public float ResourceCooldown;
    public float ResourceReset;
}
