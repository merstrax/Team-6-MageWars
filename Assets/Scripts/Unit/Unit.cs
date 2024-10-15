using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[System.Flags]
public enum StatusFlag
{
    NONE,
    SLOWED = 1,
    ROOTED = 2,
    STUNNED = 4,
    INVULNERABLE = 8,
    CASTING = 16,
}

//These flags choose which attribute the effect is applied to
[System.Flags]
public enum AttributeFlags
{
    NONE = 0,
    HEALTH = 1,
    DAMAGE = 2,
    DEFENSE = 4,
    MOVESPEED = 8,
    CRIT_CHANCE = 16,
    CRIT_DAMAGE = 32,
    COOLDOWN = 64,
    JUMPHEIGHT = 128,
    JUMPSPEED = 256,
}

public struct Stats
{
    public float Health { get; set; }
    public float Damage { get; set; }
    public float Defense { get; set; }
    public float Speed { get; set; }
    public float CritChance { get; set; }
    public float CritDamage { get; set; }
    public float Cooldown { get; set; }

    private float[] values;
    private float[] valueModifiers;

    public Stats(float health, float damage, float defense, float speed, float critChance, float critDamage, float cooldown)
    {
        Health = health;
        Damage = damage;
        Defense = defense;
        Speed = speed;
        CritChance = critChance;
        CritDamage = critDamage;
        Cooldown = cooldown;

        values = new[] { Health, Damage, Defense, Speed, CritChance, CritDamage, Cooldown };
        valueModifiers = new float[] { 1, 1, 1, 1, 1, 1, 1 };
    }

    public void ResetValues()
    {
        values = new[] { Health, Damage, Defense, Speed, CritChance, CritDamage, Cooldown };
        valueModifiers = new float[] { 1, 1, 1, 1, 1, 1, 1 };
    }

    public float this[AttributeFlags attribute]
    {
        get
        {
            int index = (int)(Mathf.Log((int)attribute) / Mathf.Log(2));
            return values[index];
        }
        set
        {
            int index = (int)(Mathf.Log((int)attribute) / Mathf.Log(2));
            values[index] = value;
        }
    }

    public void UpdateModifier(AttributeFlags attribute, float amount, int stackcount = 1)
    {
        amount = amount < 1.0f ? 1.0f - ((1.0f - amount) * stackcount) : 1 + ((amount - 1.0f) * stackcount);
        int index = (int)(Mathf.Log((int)attribute) / Mathf.Log(2));
        valueModifiers[index] *= amount;
    }

    public float GetModifier(AttributeFlags attribute)
    {
        int index = (int)(Mathf.Log((int)attribute) / Mathf.Log(2));
        return valueModifiers[index];
    }
}

public class Unit : MonoBehaviour, IDamage
{
    [Header("Unit Scriptable Object - TODO")]

    [Header("Unit Info")]
    [SerializeField] protected string unitName;
    public string GetUnitName() { return unitName; }

    [SerializeField] protected AnimationContainer animations;
    public string GetAnimation(AnimationType animation) { return animations[animation]; }

    [SerializeField] protected Transform[] castPositions;
    public Transform GetCastPos(uint castPos)
    {
        if (castPos > castPositions.Length)
            castPos = 0;

        if (castPositions.Length == 0)
            return transform;

        return castPositions[castPos];
    }

    [Header("Unit Stats")]
    [SerializeField] protected float healthRegen;
    [SerializeField] protected float healthBase;
    [SerializeField] protected float damageBase;
    [SerializeField] protected float defenseBase;
    [SerializeField] protected float speedBase;
    [SerializeField] protected float critChanceBase;
    [SerializeField] protected float critDamageBase;
    [SerializeField] protected float cooldownBase;

    protected Stats stats;

    public float healthCurrent;
    protected float healthMax;

    public float GetHealthCurrent() { return healthCurrent; }
    public void SetHealthCurrent(float healthCurrent) { this.healthCurrent = healthCurrent; }
    public float GetHealthPercent() { return healthCurrent / healthMax; }

    public float GetHealthMax() { return healthMax; }
    public void SetHealthMax(float healthMax) { this.healthMax = healthMax; }

    public float GetHealthRegen() { return healthRegen; }
    public void SetHealthRegen(float healthRegen) { this.healthRegen = healthRegen; }

    public float GetDamageBonus() { return stats[AttributeFlags.DAMAGE]; }
    public float GetDamageModifier() { return stats.GetModifier(AttributeFlags.DAMAGE); }

    public float GetDefenseBonus() { return stats[AttributeFlags.DEFENSE]; }
    public float GetDefenseModifier() { return stats.GetModifier(AttributeFlags.DEFENSE); }

    public float GetCritChanceBonus() { return stats[AttributeFlags.CRIT_CHANCE]; }
    public float GetCritDamageBonus() { return stats[AttributeFlags.CRIT_DAMAGE]; }

    public float GetStat(AttributeFlags attribute) { return stats[attribute]; }
    public float GetStatModifier(AttributeFlags attribute) { return stats.GetModifier(attribute); }

    private Dictionary<int, Ability> effects = new();
    public Dictionary<int, Ability> GetEffects() { return effects; }


    //Healthbar
    [Header("Interface")]
    [SerializeField] UnitInterface unitInterface;

    private static Dictionary<TriggerFlags, MethodInfo> Events = new()
    {
        { TriggerFlags.ON_CAST_START, typeof(Unit).GetMethod("OnCastStart") },
        { TriggerFlags.ON_CAST, typeof(Unit).GetMethod("OnCast") },
        { TriggerFlags.ON_CAST_END, typeof(Unit).GetMethod("OnCastEnd")},
        { TriggerFlags.ON_HIT, typeof(Unit).GetMethod("OnHit")},
        { TriggerFlags.ON_DAMAGE, typeof(Unit).GetMethod("OnDamage")},
        { TriggerFlags.ON_DAMAGED, typeof(Unit).GetMethod("OnDamaged")},
        { TriggerFlags.ON_KILL, typeof(Unit).GetMethod("OnKill")},
        { TriggerFlags.ON_UPDATE, typeof(Unit).GetMethod("OnUpdate")},
        { TriggerFlags.ON_HEAL, typeof(Unit).GetMethod("OnHealed")},
        { TriggerFlags.ON_DEATH, typeof(Unit).GetMethod("OnDeath")},
        { TriggerFlags.ON_INTERRUPT, typeof(Unit).GetMethod("OnInterrupt")},
        { TriggerFlags.ON_SLOW, typeof(Unit).GetMethod("OnSlow")},
        { TriggerFlags.ON_ROOT, typeof(Unit).GetMethod("OnRoot")},
        { TriggerFlags.ON_STUN, typeof(Unit).GetMethod("OnStun")},
    };

    #region Stat Handling
    protected virtual void Start()
    {
        stats = new(healthBase, damageBase, defenseBase, speedBase, critChanceBase, critDamageBase, cooldownBase);

        UpdateStats();
        healthCurrent = healthMax;

        UpdateInterface();

        if (animations != null)
            animations.Inititialize();
    }

    protected void UpdateStats()
    {
        //Reset Bonus to base
        stats.ResetValues();

        foreach (Ability ability in effects.Values)
        {
            if (ability.Info().EffectType == EffectType.MODIFIER)
            {
                var modifyFlag = ability.Info().AttributeFlags;
                int currentFlag = 1;

                while (currentFlag < (int)AttributeFlags.COOLDOWN)
                {
                    if (modifyFlag.HasFlag((AttributeFlags)currentFlag))
                    {
                        switch (ability.Info().ModifierType)
                        {
                            case EffectModifierType.ADD:
                                stats[(AttributeFlags)currentFlag] += (ability.Info().EffectAmount * ability.GetStacks());
                                break;
                            case EffectModifierType.MULTIPLY:
                                stats.UpdateModifier((AttributeFlags)currentFlag, ability.Info().EffectAmount, ability.GetStacks());
                                break;
                        }
                    }

                    currentFlag <<= 1;
                }
            }
        }

        healthMax = stats[AttributeFlags.HEALTH] * stats.GetModifier(AttributeFlags.HEALTH);
        healthCurrent = Mathf.Min(healthCurrent, healthMax);

        stats[AttributeFlags.MOVESPEED] *= stats.GetModifier(AttributeFlags.MOVESPEED);
        if (stats[AttributeFlags.MOVESPEED] < stats.Speed) { ApplyStatus(StatusFlag.SLOWED); }
        else { RemoveStatus(StatusFlag.SLOWED); }

        stats[AttributeFlags.CRIT_CHANCE] *= stats.GetModifier(AttributeFlags.CRIT_CHANCE);
        stats[AttributeFlags.CRIT_DAMAGE] *= stats.GetModifier(AttributeFlags.CRIT_DAMAGE);
    }
    #endregion

    //Status Effects
    StatusFlag statusFlag = StatusFlag.NONE;
    public void ApplyStatus(StatusFlag status, Ability source = null, Unit other = null)
    {
        statusFlag |= status;
        if (source != null)
        {
            ProccessEvent(GetStatusTrigger(status), other, source);
        }
    }
    public void RemoveStatus(StatusFlag status, Ability source = null, Unit other = null)
    {
        if (source != null)
        {
            foreach (Ability ability in effects.Values)
            {
                if (ability.GetID() == source.GetID()) continue;
                if ((StatusFlag)ability.Info().StatusType == status) return;
            }
        }
        statusFlag &= ~status;
    }

    TriggerFlags GetStatusTrigger(StatusFlag status)
    {
        return status switch
        {
            StatusFlag.SLOWED => TriggerFlags.ON_SLOW,
            StatusFlag.ROOTED => TriggerFlags.ON_ROOT,
            StatusFlag.STUNNED => TriggerFlags.ON_STUN,
            StatusFlag.INVULNERABLE => TriggerFlags.NONE,
            StatusFlag.CASTING => TriggerFlags.ON_CAST,
            _ => TriggerFlags.NONE,
        };
    }

    public bool IsInvulnerable() { return statusFlag.HasFlag(StatusFlag.INVULNERABLE); }
    public bool IsCasting() { return statusFlag.HasFlag(StatusFlag.CASTING); }
    public bool IsSlowed() { return statusFlag.HasFlag(StatusFlag.SLOWED); }
    public bool IsStunned() { return statusFlag.HasFlag(StatusFlag.STUNNED); }
    public bool IsRooted() { return statusFlag.HasFlag(StatusFlag.ROOTED); }

    protected Vector3 moveDir;
    public Vector3 GetMoveDir() { return moveDir; }
    public void SetMoveDir(Vector3 moveDir) { this.moveDir = moveDir; }
    public void UpdateMoveDir(Vector2 move)
    {
        moveDir = move.x * transform.right +
                    move.y * transform.forward;
    }
    public float GetSpeed()
    {
        if (IsStunned() || IsRooted())
        {
            return 0.0f;
        }
        return stats[AttributeFlags.MOVESPEED];
    }

    public Damage CalculateDamage(float damage, float coefficient, bool canCrit = true)
    {
        damage += (coefficient * stats[AttributeFlags.DAMAGE]);
        damage *= stats.GetModifier(AttributeFlags.DAMAGE);

        bool isCritical = Random.Range(0.0f, 1.0f) < stats[AttributeFlags.CRIT_CHANCE];
        damage *= isCritical && canCrit ? stats[AttributeFlags.CRIT_DAMAGE] : 1.0f;

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
        float damageReduction = (IDamage.DefenseCoefficient / (IDamage.DefenseCoefficient + stats[AttributeFlags.DEFENSE]));
        damage *= damageReduction;

        if (stats.GetModifier(AttributeFlags.DEFENSE) != 0)
            damage /= stats.GetModifier(AttributeFlags.DEFENSE);

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

        if (ability.Info().StatusType != EffectStatusType.NONE)
        {
            ApplyStatus((StatusFlag)ability.Info().StatusType, ability);
        }
    }

    public void RemoveEffect(Ability ability)
    {
        int _id = ability.GetID();
        if (effects.ContainsKey(_id))
        {
            if (ability.Info().StatusType != EffectStatusType.NONE)
            {
                RemoveStatus((StatusFlag)ability.Info().StatusType, ability);
            }

            effects[_id].CleanUp(true);
            effects.Remove(_id);
        }
        UpdateStats();

        UpdateInterface();
    }

    public void RemoveAllEffects()
    {
        CleanUp();
    }

    public virtual void InterruptCasting(Ability source = null, Unit other = null)
    {
        RemoveStatus(StatusFlag.CASTING);
        ProccessEvent(TriggerFlags.ON_INTERRUPT, other, source);
    }

    public virtual void TakeDamage(float amount, Ability source = null, Unit other = null)
    {
        TakeDamage(new Damage(amount), source, other);
    }

    //Overridable but not recommended
    public virtual void TakeDamage(Damage damage, Ability source = null, Unit other = null)
    {
        if (IsInvulnerable()) return;

        damage.Amount = CalculateDefense(damage.Amount);

        if (other != null)
        {
            other.ProccessEvent(TriggerFlags.ON_DAMAGE, other, source, damage);
        }

        ProccessEvent(TriggerFlags.ON_DAMAGED, damage: damage, source: source, other: other);

        healthCurrent -= damage.Amount;

        if (healthCurrent <= 0)
        {
            ProccessEvent(TriggerFlags.ON_DEATH, source: source, other: other);
        }
        if (unitInterface != null)
        {
            UpdateInterface();
            unitInterface.CreateFloatingNumber((int)damage.Amount);
        }
    }

    public virtual void Heal(Damage damage, Ability source = null, Unit other = null)
    {
        healthCurrent += damage.Amount;
        healthCurrent = Mathf.Min(healthCurrent, healthMax);

        ProccessEvent(TriggerFlags.ON_HEAL, damage: damage, source: source, other: other);

        if (unitInterface != null)
        {
            UpdateInterface();
            //unitInterface.CreateFloatingNumber((int)damage.Amount); Add Heal Color
        }
    }

    public virtual void HealPercent(Damage damage, Ability source = null, Unit other = null)
    {
        damage.Amount = (healthMax * damage.Amount);

        healthCurrent += damage.Amount;
        healthCurrent = Mathf.Min(healthCurrent, healthMax);

        Debug.Log("Healed: " + damage.Amount);

        ProccessEvent(TriggerFlags.ON_HEAL, damage: damage, source: source, other: other);

        if (unitInterface != null)
        {
            UpdateInterface();
            //unitInterface.CreateFloatingNumber((int)damage.Amount); Add Heal Color
        }
    }

    public virtual void ProccessEvent(TriggerFlags trigger, Unit other = null, Ability source = null, Damage damage = new Damage())
    {
        int effectsCount = effects.Count;

        foreach (Ability ability in effects.Values)
        {
            if (ability.Info().TriggerFlags.HasFlag(trigger))
            {
                if (source != ability)
                    ability.DoEffectApply(other);
                if (effectsCount != effects.Count) break;
            }
        }

        if (trigger == TriggerFlags.ON_DEATH)
        {
            OnDeath();
        }

        Events[trigger].Invoke(this, new object[] { other, source, damage });
    }

    //Scriptable functions
    public virtual void OnCastStart(Unit other = null, Ability source = null, Damage damage = default) { }
    public virtual void OnCast(Unit other = null, Ability source = null, Damage damage = default) { }
    public virtual void OnCastEnd(Unit other = null, Ability source = null, Damage damage = default) { }
    public virtual void OnHit(Unit other = null, Ability source = null, Damage damage = default) { }
    public virtual void OnDamage(Unit other = null, Ability source = null, Damage damage = default) { }
    public virtual void OnDamaged(Unit other = null, Ability source = null, Damage damage = default) { }
    public virtual void OnKill(Unit other = null, Ability source = null, Damage damage = default) { }
    public virtual void OnUpdate(Unit other = null, Ability source = null, Damage damage = default) { }
    public virtual void OnHealed(Unit other = null, Ability source = null, Damage damage = default) { }
    public virtual void OnInterrupted(Unit other = null, Ability source = null, Damage damage = default) { }
    public virtual void OnSlow(Unit other = null, Ability source = null, Damage damage = default) { }
    public virtual void OnRoot(Unit other = null, Ability source = null, Damage damage = default) { }
    public virtual void OnStun(Unit other = null, Ability source = null, Damage damage = default) { }

    public virtual void OnDeath(Unit other = null, Ability source = null, Damage damage = default)
    {
        if (other != null)
        {
            other.ProccessEvent(TriggerFlags.ON_KILL, other: this);
        }
        CleanUp();
        Destroy(gameObject);
    }

    private void CleanUp()
    {
        //Clears any effects that might not have a cleanup timer attached to it
        foreach (Ability ability in effects.Values)
        {
            ability.CleanUp(true);
        }
        effects.Clear();
    }

    public void UpdateInterface()
    {
        if (unitInterface != null)
        {
            unitInterface.UpdateHealthBar(healthCurrent, healthMax);
        }
    }
}
