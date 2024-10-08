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
    public float AbilitySpeed;
    public float AbilityRange;
    public ElementType ElementType;
    public AbilityType AbilityType;
    public CastType CastType;
    public float Cooldown;

    [Header("Effect Info")]
    public EffectType EffectType;
    public EffectTargetType EffectTargetType;
    public EffectStatusType StatusType;
    public EffectAttributeFlags AttributeFlags;
    public EffectElementFlags ElementFlags;
    public EffectTriggerFlags TriggerFlags;
    public EffectModifierType ModifierType;
    public AbilityStats EffectAbility;
    public float EffectAmount;
    public float DamageCoefficent;
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
    public AbilityStats[] ComboAbilities;

    [Header("Visuals")]
    public Texture2D AbilityIcon;
    public GameObject AbilityVisual;

    //Replace string tokens with values of the Ability
    private readonly string damageColor = ColorUtility.ToHtmlStringRGB(Color.red);
    private readonly string rangeColor = ColorUtility.ToHtmlStringRGB(Color.yellow);

    public string GetDescription(bool showAdanced = false)
    {
        if (showAdanced) return GetDescriptionAdvanced();

        string output = AbilityDescription;

        output.Replace("{$d}", Colorize(EffectAmount.ToString("000"), damageColor));
        output.Replace("{$r}", Colorize(AbilityRange.ToString("00.0"), rangeColor));

        return output;
    }

    private string GetDescriptionAdvanced()
    {
        string output = AbilityDescription;

        output.Replace("{$d}", Colorize(EffectAmount.ToString("000") + " + (" + (DamageCoefficent * 100.0f).ToString("000") + "% Damage Bonus)", damageColor));
        output.Replace("{$r}", Colorize(AbilityRange.ToString("00.0"), rangeColor));

        return output;
    }

    public string GetDescription(Unit unit)
    {
        string output = AbilityDescription;

        output.Replace("{$d}", Colorize(unit.CalculateDamage(EffectAmount, DamageCoefficent, false).Amount.ToString("000"), damageColor));
        output.Replace("{$r}", Colorize(AbilityRange.ToString("00.0"), rangeColor));

        return output;
    }

    private string Colorize(string value, string color)
    {
        value = color + value + "</color>";
        return value;
    }
}
