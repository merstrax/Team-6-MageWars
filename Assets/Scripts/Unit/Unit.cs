using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Unit : MonoBehaviour, IDamage
{
    [Header("Unit Scriptable Object - TODO")]

    [Header("Unit Info")]
    [SerializeField] protected string unitName;
    public string GetUnitName() { return unitName; }
    [SerializeField] Transform castPos;
    public Transform GetCastPos() { return castPos; }

    [Header("Unit Stats")]
    [SerializeField] protected float healthRegen;
    [SerializeField] protected float healthBase;
    [SerializeField] protected float damageBase;
    [SerializeField] protected float defenseBase;
    [SerializeField] protected float speedBase;
    [SerializeField] protected float critChanceBase;
    [SerializeField] protected float critDamageBase;

    protected float healthBonus;
    protected float damageBonus;
    protected float defenseBonus;
    protected float speedBonus;
    protected float critChanceBonus;
    protected float critDamageBonus;

    protected float healthModifier;
    protected float damageModifier;
    protected float defenseModifier;
    protected float speedModifier;
    protected float critChanceModifier;
    protected float critDamageModifier;

    public float healthCurrent;
    protected float healthMax;

    public float GetHealthCurrent() { return healthCurrent; }
    public void SetHealthCurrent(float healthCurrent) { this.healthCurrent = healthCurrent; }

    public float GetHealthMax() { return healthMax; }
    public void SetHealthMax(float healthMax) { this.healthMax = healthMax; }

    public float GetHealthRegen() { return healthRegen; }
    public void SetHealthRegen(float healthRegen) { this.healthRegen = healthRegen; }

    public float GetDamageBonus() { return damageBonus; }
    public float GetDamageModifier() { return damageModifier; }
    public void SetDamageModifier(float damageModifier) { this.damageModifier = damageModifier; }

    public float GetDefenseBonus() {  return defenseBonus; }
    public float GetDefenseModifier() { return defenseModifier; }
    public void SetDefenseModifier(float defenseModifier) { this.defenseModifier = defenseModifier; }

    public float GetCritChanceBonus() {  return critChanceBonus; }
    public float GetCritDamageBonus() { return critDamageBonus; }

    private Dictionary<int, Ability> effects = new();
    public Dictionary<int, Ability> GetEffects() { return effects; }

    bool isInvulnerable;
    public void SetInvulnerable(bool isInvulnerable) { this.isInvulnerable = isInvulnerable; }

    //Healthbar
    [Header("Interface")]
    [SerializeField] UnitInterface unitInterface;
    Material outlineMaterial;
    

    #region Stat Handling
    protected virtual void Start()
    {
        UpdateStats();
        healthCurrent = healthMax;

        UpdateInterface();
        outlineMaterial = Resources.Load<Material>("Materials/OutlineMaterial");
        outlineMaterial = Instantiate(outlineMaterial);
        if (outlineMaterial != null)
        {
            List<Renderer> renderers = new List<Renderer>();

            foreach(Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                List<Material> materials = new(){ outlineMaterial};
                materials.AddRange(renderer.materials);
                renderer.SetMaterials(materials);
            }
        }

        outlineMaterial.SetFloat("_OutlineWidth", 0f);
    }

    public void TargetOutline(bool target = true)
    {
        if (target)
        {
            outlineMaterial.SetFloat("_OutlineWidth", 0.075f);
        }else
            outlineMaterial.SetFloat("_OutlineWidth", 0f);
    }

    protected void UpdateStats()
    {
        //Reset Bonus to base
        healthBonus = healthBase;
        damageBonus = damageBase;
        defenseBonus = defenseBase;
        speedBonus = speedBase;
        critChanceBonus = critChanceBase;
        critDamageBonus = critDamageBase;

        //Reset modifiers to 1
        healthModifier = 1;
        damageModifier = 1;
        defenseModifier = 1;
        speedModifier = 1;
        critChanceModifier = 1;
        critDamageModifier = 1;

        foreach (Ability ability in effects.Values)
        {
            if(ability.Info().TriggerFlags.HasFlag(EffectTriggerFlags.ON_UPDATE) && ability.Info().EffectType == EffectType.MODIFIER)
            {
                var modifyFlag = ability.Info().AttributeFlags;

                if(modifyFlag.HasFlag(EffectAttributeFlags.DAMAGE))
                {
                    UpdateDamage(ability.Info().EffectAmount, ability.Info().ModifierType, ability.GetStacks());
                }

                if (modifyFlag.HasFlag(EffectAttributeFlags.DEFENSE))
                {
                    UpdateDefense(ability.Info().EffectAmount, ability.Info().ModifierType, ability.GetStacks());
                }

                if (modifyFlag.HasFlag(EffectAttributeFlags.MOVESPEED))
                {
                    UpdateSpeed(ability.Info().EffectAmount, ability.Info().ModifierType, ability.GetStacks());
                }

                if (modifyFlag.HasFlag(EffectAttributeFlags.CRIT_CHANCE))
                {
                    UpdateCritChance(ability.Info().EffectAmount, ability.Info().ModifierType, ability.GetStacks());
                }

                if (modifyFlag.HasFlag(EffectAttributeFlags.CRIT_DAMAGE))
                {
                    UpdateCritDamage(ability.Info().EffectAmount, ability.Info().ModifierType, ability.GetStacks());
                }
            }
        }

        healthMax = healthBonus * healthModifier;
        healthCurrent = Mathf.Min(healthCurrent, healthMax);

        defenseBonus *= defenseModifier;
        speedBonus *= speedModifier;
        critChanceBonus *= critChanceModifier;
        critDamageBonus *= critDamageModifier;
    }

    protected void UpdateHealthMax(float amount, EffectModifierType modType, int stackcount = 1)
    {
        switch(modType)
        {
            case EffectModifierType.ADD:
                healthBonus += (amount * stackcount);
                break;
            case EffectModifierType.MULTIPLY:
                amount = amount < 1.0f ? 1.0f - ((1.0f - amount) * stackcount) : 1 + ((amount - 1.0f) * stackcount);
                healthModifier *= amount;
                break;
        }
    }

    protected void UpdateDamage(float amount, EffectModifierType modType, int stackcount = 1)
    {
        switch (modType)
        {
            case EffectModifierType.ADD:
                damageBonus += (amount * stackcount);
                break;
            case EffectModifierType.MULTIPLY:
                amount = amount < 1.0f ? 1.0f - ((1.0f - amount) * stackcount) : 1 + ((amount - 1.0f) * stackcount);
                damageModifier *= amount;
                break;
        }
    }

    protected void UpdateDefense(float amount, EffectModifierType modType, int stackcount = 1)
    {
        switch (modType)
        {
            case EffectModifierType.ADD:
                defenseBonus += (amount * stackcount);
                break;
            case EffectModifierType.MULTIPLY:
                amount = amount < 1.0f ? 1.0f - ((1.0f - amount) * stackcount) : 1 + ((amount - 1.0f) * stackcount);
                defenseModifier *= amount;
                break;
        }
    }

    protected void UpdateSpeed(float amount, EffectModifierType modType, int stackcount = 1)
    {
        switch (modType)
        {
            case EffectModifierType.ADD:
                speedBonus += (amount * stackcount);
                break;
            case EffectModifierType.MULTIPLY:
                amount = amount < 1.0f ? 1.0f - ((1.0f - amount) * stackcount) : 1 + ((amount - 1.0f) * stackcount);
                speedModifier *= amount;
                break;
        }
    }

    protected void UpdateCritChance(float amount, EffectModifierType modType, int stackcount = 1)
    {
        switch (modType)
        {
            case EffectModifierType.ADD:
                amount = amount > 1 ? amount / 100 : amount;
                critChanceBonus += (amount * stackcount);
                break;
            case EffectModifierType.MULTIPLY:
                amount = amount < 1.0f ? 1.0f - ((1.0f - amount) * stackcount) : 1 + ((amount - 1.0f) * stackcount);
                critChanceModifier *= amount;
                break;
        }
    }

    protected void UpdateCritDamage(float amount, EffectModifierType modType, int stackcount = 1)
    {
        switch (modType)
        {
            case EffectModifierType.ADD:
                amount = amount > 1 ? amount / 100 : amount;
                critDamageBonus += (amount * stackcount);
                break;
            case EffectModifierType.MULTIPLY:
                amount = amount < 1.0f ? 1.0f - ((1.0f - amount) * stackcount) : 1 + ((amount - 1.0f) * stackcount);
                critDamageModifier *= amount;
                break;
        }
    }
    #endregion

    //Status Effects
    private bool isCasting;
    private bool isSlowed;
    private bool isStunned;
    private bool isRooted;

    public bool IsCasting() { return isCasting; }
    public void SetCasting(bool isCasting) { this.isCasting = isCasting; }

    public bool IsSlowed() { return isSlowed; }
    public void SetSlowed(bool isSlowed) { this.isSlowed = isSlowed; }

    public bool IsStunned() { return isStunned; }
    public void SetStunned(bool isStunned) { this.isStunned = isStunned; }

    public bool IsRooted() { return isRooted; }
    public void SetRooted(bool isRooted) { this.isRooted = isRooted; }

    protected Vector3 moveDir;
    public Vector3 GetMoveDir() { return moveDir; }
    public void SetMoveDir(Vector3 moveDir) { this.moveDir = moveDir; }
    public void UpdateMoveDir(Vector2 move) 
    { 
        moveDir = move.x * transform.right +
                    move.y * transform.forward;
    }
    public float GetSpeed() { return speedBonus; }

    public Damage CalculateDamage(float damage, float coefficient, bool canCrit = true)
    {
        damage += (coefficient * damageBonus);
        damage *= damageModifier;

        bool isCritical = Random.Range(0.0f, 1.0f) < critChanceBonus;
        damage *= isCritical && canCrit ? critDamageBonus : 1.0f;

        return new Damage(damage, isCritical);
    }

    public float CalculateDefense(float damage)
    {
        /*
         * Every 20 defense will reduces the effectiveness of defense by 50%
         * Damage = 100
         * 
         * Defense 0 : Damage 100
         * Defense 20 : Damage 50 (Average 2.5% per defense)
         * Defense 60 : Damage 25 (Average 1.25% per defense)
         * Defense 140 : Damage 12.5 (Average 0.625% per defense)
         */
        float damageReduction = (IDamage.DefenseCoefficient / (IDamage.DefenseCoefficient + defenseBonus)); 
        damage *= damageReduction;

        return damage;
    }

    public void ApplyEffect(Ability ability)
    {
        int _id = ability.GetID();

        if (effects.ContainsKey(_id))
        {
            effects[_id].AddStack();
            ability.CleanUp(true); //Remove ability as one already exists
        }
        else
        {
            effects.Add(_id, ability);
        }
        UpdateStats();

        UpdateInterface();
    }
    
    public void RemoveEffect(Ability ability)
    {
        int _id = ability.GetID();
        if (effects.ContainsKey(_id))
        {
            effects[_id].CleanUp(true);
            effects.Remove(_id);
        }
        UpdateStats();

        UpdateInterface();
    }

    public virtual void InterruptCasting()
    {
        SetCasting(false);
        OnInterrupted();
    }

    public virtual void TakeDamage(float amount, Unit other = null)
    {
        TakeDamage(new Damage(amount), other);
    }

    //Overridable but not recommended
    public virtual void TakeDamage(Damage damage, Unit other = null)
    {
        if (isInvulnerable) return;

        float _reducedDamage = CalculateDefense(damage.Amount);
        healthCurrent -= _reducedDamage;

        if (other != null)
        {
            other.OnDamage(damage, this);
        }
        
        OnDamaged(damage, other);

        if(healthCurrent <= 0)
        {
            OnDeath(other);
        }
        if (unitInterface != null)
        {
            UpdateInterface();
            unitInterface.CreateFloatingNumber((int)damage.Amount);
        }

        string damageOutput = unitName + " took " + _reducedDamage + " damage";
        damageOutput += damage.IsCritical ? " (Critical)." : ".";
        Debug.Log(damageOutput);
    }

    //Scriptable functions
    public virtual void OnHit(Unit other, Ability ability) { }
    public virtual void OnDamage(Damage damage, Unit other = null) { }
    public virtual void OnDamaged(Damage damage, Unit other = null){ }
    public virtual void OnHealed(Damage damage, GameObject obj = null) { }
    public virtual void OnKill(Unit other){ }
    public virtual void OnInterrupted(Unit other = null){ }
    public virtual void OnCast(Ability ability = null) { }
    public virtual void OnDeath(Unit other = null)
    {
        if(other != null) { other.OnKill(this); }
        CleanUp();
        Destroy(gameObject);
    }

    private void CleanUp()
    {
        //Clears any effects that might not have a cleanup timer attached to it
        foreach(Ability ability in effects.Values)
        {
            ability.CleanUp(true);
        }
    }

    private void UpdateInterface()
    {
        if(unitInterface != null)
        {
            unitInterface.UpdateHealthBar(healthCurrent, healthMax);
        }
    }
}
