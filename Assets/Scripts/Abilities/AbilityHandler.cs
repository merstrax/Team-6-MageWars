using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class AbilityHandler : MonoBehaviour
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

    Ability ability;
    public Ability GetAbility() {  return ability; }

    protected Unit owner;
    protected Unit other;

    bool IsCasting;
    float CastStartTime;
    float CooldownStart;
    bool CanCast = true;
    Vector3 CastTarget;

    int ResourceCurrent;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: Cast Logic

        if (owner == null)
        {
            Destroy(gameObject);
        }

        UpdateCooldown();
    }

    public void Setup(Unit owner, Ability ability)
    {
        this.owner = owner;
        this.ability = ability;

        AbilityType = ability.AbilityType;
        CastType = ability.CastType;
        Cooldown = ability.Cooldown;
        ResourceType = ability.ResourceType;
        ResourceMax = ability.ResourceMax;
    }

    public bool IsMovementAbility()
    {
        return AbilityType == AbilityType.MOVEMENT || AbilityType == AbilityType.TELEPORT;
    }

    void UpdateCooldown()
    {
        if (CooldownRemaining() <= 0)
        {
            if (HasCharges())
            {
                GainCharge();
                if (!HasMaxCharges())
                {
                    CooldownStart = Time.time;
                }
            }
            CanCast = true;
        }
    }

    //Cooldown Handling
    public bool ReadyToCast()
    {
        return CanCast && HasChargesRemaining();
    }

    public void StartCooldown()
    {
        if (HasCharges())
        {
            if (ResourceCurrent == ResourceMax)
            {
                CooldownStart = Time.time;
            }
            ConsumeCharge();
        }
        else
        {
            CooldownStart = Time.time;
            CanCast = false;
        }
    }

    public int CooldownRemaining()
    {
        return Mathf.RoundToInt(CooldownStart + Cooldown - Time.time);
    }

    //Charges Handling
    bool HasCharges()
    {
        return ResourceMax > 0;
    }

    bool HasChargesRemaining()
    {
        return ResourceCurrent > 0 || !HasCharges();
    }

    void ConsumeCharge()
    {
        ResourceCurrent = Mathf.Max(ResourceCurrent - 1, 0);
    }

    bool HasMaxCharges()
    {
        return ResourceCurrent == ResourceMax;
    }

    public void GainCharge(int amount = 1)
    {
        ResourceCurrent = Mathf.Min(ResourceCurrent + amount, ResourceMax);
    }
}
