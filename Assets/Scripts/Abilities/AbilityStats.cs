using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu]
public class AbilityStats : ScriptableObject
{
    [Header("Ability Info")]
    public string AbilityName;
    public string AbilityDescription;
    public int AbilityID;
    public float AbilitySpeed;
    public float AbilityRange;
    public ElementType ElementType;
    public AbilityType AbilityType;
    public CastType CastType;
    public float Cooldown;
    public float CastTime;
    public bool IsRooted;
    public bool IsTarget;

    [Header("Effect Info")]
    public EffectType EffectType;
    public EffectTargetType EffectTargetType;
    public EffectStatusType StatusType;
    public AttributeFlags AttributeFlags;
    public EffectElementFlags ElementFlags;
    public TriggerFlags TriggerFlags;
    public EffectModifierType ModifierType;
    public AbilityStats EffectAbility;
    public float EffectAmount;
    public float DamageCoefficent;
    public float EffectTriggerChance;
    public int EffectStackAmount;
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
    public Status StatusIcon;
    public GameObject AbilityVisual;
    public GameObject AbilityImpact;
    public GameObject AbilityCast;
    public uint CastPosition;
    public AnimationType AnimationType;

    //Replace string tokens with values of the Ability
    private readonly string damageColor = ColorUtility.ToHtmlStringRGB(Color.red);
    private readonly string rangeColor = ColorUtility.ToHtmlStringRGB(Color.yellow);
    private readonly string timeColor = ColorUtility.ToHtmlStringRGB(Color.green);

    public string GetDescription(bool showAdanced = false)
    {
        if (showAdanced) return GetDescriptionAdvanced();

        string output = AbilityDescription;

        output = output.Replace("{$d}", Colorize(EffectAmount.ToString("0.00"), damageColor));
        output = output.Replace("{$r}", Colorize(AbilityRange.ToString("0"), rangeColor));
        output = output.Replace("{$ts}", Colorize(EffectTickSpeed.ToString("0.0") + "s", timeColor));
        output = output.Replace("{$ct}", Colorize(CastTime.ToString("0.0") + "s", timeColor));

        return output;
    }

    private string GetDescriptionAdvanced()
    {
        string output = AbilityDescription;

        output = output.Replace("{$d}", Colorize(EffectAmount.ToString() + " + (" + (DamageCoefficent * 100.0f).ToString("0.00") + "% Damage Bonus)", damageColor));
        output = output.Replace("{$r}", Colorize(AbilityRange.ToString("0"), rangeColor));
        output = output.Replace("{$ts}", Colorize(EffectTickSpeed.ToString("0.0") + "s", timeColor));
        output = output.Replace("{$ct}", Colorize(CastTime.ToString("0.0") + "s", timeColor));

        return output;
    }

    public string GetDescription(Unit unit)
    {
        string output = AbilityDescription;

        output = output.Replace("{$d}", Colorize(unit.CalculateDamage(EffectAmount, DamageCoefficent, false).Amount.ToString("0.00"), damageColor));
        output = output.Replace("{$r}", Colorize(AbilityRange.ToString("0"), rangeColor));
        output = output.Replace("{$ts}", Colorize(EffectTickSpeed.ToString("0.0") + "s", timeColor));
        output = output.Replace("{$ct}", Colorize(CastTime.ToString("0.0") + "s", timeColor));

        return output;
    }

    private string Colorize(string value, string color)
    {
        value = "<color=#" + color + ">" + value + "</color>";
        return value;
    }
}
