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

    [Header("Resource Info")]
    public ResourceType ResourceType;
    public int ResourceMax;
    public float ResourceCooldown;
    public float ResourceReset;

    Ability ability;
    public Ability GetAbility() {  return ability; }
    int index;

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

        if(!ReadyToCast())
            UpdateCooldown();
    }

    public void Setup(Unit owner, Ability ability, int index = -1)
    {
        this.owner = owner;
        this.ability = ability;

        AbilityType = ability.Info().AbilityType;
        CastType = ability.Info().CastType;
        Cooldown = ability.Info().Cooldown;
        ResourceType = ability.Info().ResourceType;
        ResourceMax = ability.Info().ResourceMax;

        if(ResourceType == ResourceType.CHARGES)
        {
            ResourceCurrent = ResourceMax;
        }

        this.index = index;
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

        UpdateInterface();
    }

    void UpdateInterface()
    {
        if (index == -1) return;

        GameManager.instance.UpdateAbilityCooldown(index, !CanCast, CooldownRemaining(), Cooldown);
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

    public float CooldownRemaining()
    {
        return Mathf.Max(CooldownStart + Cooldown - Time.time, 0);
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

    public override string ToString()
    {
        string output = ability.Info().AbilityName;

        if (ResourceMax > 0)
            output += " Charges: " + ResourceCurrent;

        if (!ReadyToCast() || (ResourceMax > 0 && !HasMaxCharges()))
            output += " CD: " + CooldownRemaining();

        return output;
    }
}
