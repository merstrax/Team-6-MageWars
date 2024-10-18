using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ability : MonoBehaviour
{
    [Header("Ability Stats")]
    [SerializeField] protected AbilityStats AbilityInfo;
    public void SetAbilityInfo(AbilityStats abilityInfo) { AbilityInfo = abilityInfo; }
    public AbilityStats GetAbilityInfo() { return AbilityInfo; }
    public AbilityStats Info() { return AbilityInfo; }

    public int GetID() { return AbilityInfo.AbilityID; }
    public CastType GetCastType() { return AbilityInfo.CastType; }
    public AbilityType GetAbilityType() { return AbilityInfo.AbilityType; }

    protected Unit owner;
    protected Unit other;
    protected List<Unit> others;
    protected Vector3 castTarget;

    protected int EffectStackCount = 1;
    public int GetStacks() { return EffectStackCount; }
    protected float EffectTimeApplied = 0;
    protected float EffectLastTick;

    [Header("Ability Components")]
    [SerializeField] protected Collider myCollider;
    [SerializeField] protected GameObject myVisual;
    [SerializeField] protected Rigidbody myRigidbody;

    [Header("Additional Scripted Fields")]
    [SerializeField] protected string scriptName;
    Damage _calcDamage;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        EffectTimeApplied = Time.time;

        others = new List<Unit>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (AbilityInfo.AbilityType == AbilityType.EFFECT)
            UpdateEffect();

        //Area Effects with matching TickSpeed and Duration are handled like single target onHit effects
        if (AbilityInfo.AbilityType == AbilityType.AREAOFEFFECT && Info().EffectTickSpeed != 0)
            UpdateAreaEffect();

        if (EffectTimeApplied + AbilityInfo.EffectDuration < Time.time)
        {
            CleanUpEffect();
        }
    }

    protected virtual void UpdateEffect()
    {
        if (AbilityInfo.EffectType == EffectType.DAMAGE)
        {
            if (EffectLastTick + AbilityInfo.EffectTickSpeed < Time.time)
            {
                DoDamage(other);
                EffectLastTick = Time.time;
            }
        }
    }

    protected virtual void UpdateAreaEffect()
    {
        if (AbilityInfo.EffectType == EffectType.DAMAGE)
        {
            if (EffectLastTick + AbilityInfo.EffectTickSpeed < Time.time)
            {
                foreach (Unit unit in others)
                {
                    DoDamage(unit);
                }
                EffectLastTick = Time.time;
            }
        }
    }

    public void SetOwner(Unit owner)
    {
        if (owner == null) return;

        this.owner = owner;
        transform.gameObject.tag = owner.tag;
    }

    public void SetOther(Unit other)
    {
        if (other == null) return;

        this.other = other;
    }

    //Basic Cast Handling
    public virtual void CastStart(Unit owner, Vector3 lookAt)
    {
        if (owner != null)
            SetOwner(owner);

        castTarget = lookAt;

        if (myCollider != null)
            myCollider.enabled = false;

        if (myVisual != null)
            myVisual.SetActive(false);

        OnCastStart();
    }

    protected virtual void OnCastStart()
    {
        //Used for scripts
        if(owner != null)
            owner.ProccessEvent(TriggerFlags.ON_CAST_START, other, this);
    }

    public virtual void Cast(Vector3 end = default)
    {
        if(myCollider != null)
            myCollider.enabled = true;

        if(myVisual != null)
            myVisual.SetActive(true);

        if (end != default)
            castTarget = end;

        if (myRigidbody != null && !Info().IsTarget)
        {
            transform.LookAt(castTarget);
            myRigidbody.velocity = transform.forward * AbilityInfo.AbilitySpeed;
        }
        else
        {
            transform.position = castTarget;
        }

        OnCast();
        CastEnd();
    }

    protected virtual void OnCast()
    {
        //Used for scripts
        if(owner != null)
            owner.ProccessEvent(TriggerFlags.ON_CAST, other, this);
    }

    public virtual void CastEnd()
    {
        OnCastEnd();
        CleanUp();
    }

    protected virtual void OnCastEnd()
    {
        if (owner != null)
            owner.ProccessEvent(TriggerFlags.ON_CAST_END, other, this);
    }

    public virtual void CastInterrupt()
    {
        if (owner != null)
            owner.ProccessEvent(TriggerFlags.ON_INTERRUPT, other, this);
    }

    public virtual void CleanUp(bool instant = false)
    {
        if (instant)
        {
            Destroy(gameObject);
            return;
        }
        Destroy(gameObject, AbilityInfo.EffectDuration);
    }

    public virtual void CleanUpEffect()
    {
        switch (AbilityInfo.EffectTargetType)
        {
            case EffectTargetType.OWNER:
                if (owner != null)
                    owner.RemoveEffect(this);
                else
                    CleanUp(true);
                break;
            case EffectTargetType.OTHER:
                if (other != null)
                    other.RemoveEffect(this);
                else
                    CleanUp(true);
                break;
            default:
                break;
        }
    }

    //Movement Ability Handling
    public virtual void CastMovement()
    {
        if (owner.TryGetComponent<PlayerMoveController>(out PlayerMoveController controller))
        {
            controller.StartDash(Info().AbilitySpeed, Info().EffectDuration);
            OnCast();
        }
        CleanUp();
    }

    //Damage Handlers
    public virtual Damage CalculatedDamage()
    {
        if (owner == null)
        {
            _calcDamage = new Damage(AbilityInfo.EffectAmount);
            _calcDamage.Amount *= EffectStackCount;
            
            return _calcDamage;
        }

        _calcDamage = owner.CalculateDamage(AbilityInfo.EffectAmount, AbilityInfo.DamageCoefficent);
        _calcDamage.Amount *= EffectStackCount;

        return _calcDamage;
    }

    public virtual void DoDamage(Unit other)
    {
        Damage _damage = CalculatedDamage();

        if (other != null)
        {
            other.TakeDamage(_damage, this, owner);

            OnDamage(other);
        }

        if (AbilityInfo.AbilityType == AbilityType.PROJECTILE)
        {
            Destroy(gameObject);
        }
    }

    public virtual void DoHeal(Unit other)
    {
        _calcDamage = new Damage(AbilityInfo.EffectAmount);
        _calcDamage.Amount *= EffectStackCount;

        switch (AbilityInfo.EffectTargetType)
        {
            case EffectTargetType.OWNER:
                if (owner != null)
                    owner.Heal(_calcDamage, this, other);
                break;
            case EffectTargetType.OTHER:
                if (other != null)
                    other.Heal(_calcDamage, this, owner);
                break;
            default:
                break;
        }

        if (AbilityInfo.AbilityType == AbilityType.PROJECTILE)
        {
            Destroy(gameObject);
        }
    }

    public virtual void DoHealPercent(Unit other)
    {
        _calcDamage = new Damage(AbilityInfo.EffectAmount);
        _calcDamage.Amount *= EffectStackCount;

        switch (AbilityInfo.EffectTargetType)
        {
            case EffectTargetType.OWNER:
                if (owner != null)
                    owner.HealPercent(_calcDamage, this, other);
                break;
            case EffectTargetType.OTHER:
                if (other != null)
                    other.HealPercent(_calcDamage, this, other);
                break;
            default:
                break;
        }

        if (AbilityInfo.AbilityType == AbilityType.PROJECTILE)
        {
            Destroy(gameObject);
        }
    }

    public virtual void DoEffectApply(Unit other)
    {
        bool isSuccess = Random.Range(0, 1) < AbilityInfo.EffectTriggerChance;
        if (isSuccess && AbilityInfo.EffectAbility != null)
        {
            GameObject _effect = new(AbilityInfo.EffectAbility.AbilityName);
            Ability toApply = _effect.AddComponent<Ability>();

            if (other == null) other = this.other;

            toApply.SetAbilityInfo(AbilityInfo.EffectAbility);
            toApply.SetOwner(owner);
            toApply.SetOther(other);

            switch (toApply.AbilityInfo.EffectTargetType)
            {
                case EffectTargetType.OWNER:
                    if (owner != null)
                        owner.ApplyEffect(toApply);
                    break;
                case EffectTargetType.OTHER:
                    if (other != null)
                        other.ApplyEffect(toApply);
                    break;
                default:
                    break;
            }
        }
    }

    public virtual void DoEffectApply(Unit other, AbilityStats effect)
    {
        GameObject _effect = new(effect.AbilityName);
        Ability toApply = _effect.AddComponent<Ability>();

        if (other == null) other = this.other;

        toApply.SetAbilityInfo(effect);
        toApply.SetOwner(owner);
        toApply.SetOther(other);

        switch (effect.EffectTargetType)
        {
            case EffectTargetType.OWNER:
                if (owner != null)
                    owner.ApplyEffect(toApply);
                break;
            case EffectTargetType.OTHER:
                if (other != null)
                    other.ApplyEffect(toApply);
                break;
            default:
                break;
        }
    }

    public virtual void AddStack()
    {
        if (AbilityInfo.EffectStackMax > 1)
        {
            if (EffectStackCount < AbilityInfo.EffectStackMax)
            {
                EffectStackCount += Info().EffectStackAmount;
            }
        }

        //Reset effect duration
        EffectTimeApplied = Time.time;
    }

    //Triggers
    public virtual void OnHit(Unit other)
    {
        if (AbilityInfo.EffectAmount > 0)
        {
            if (AbilityInfo.TriggerFlags.HasFlag(TriggerFlags.ON_HIT))
            {
                DoEffectApply(other);
            }
            if (owner != null)
                owner.ProccessEvent(TriggerFlags.ON_HIT, other, this);

            switch(AbilityInfo.EffectType)
            {
                case EffectType.HEAL:
                    if (AbilityInfo.ModifierType == EffectModifierType.MULTIPLY)
                        DoHealPercent(other);
                    else
                        DoHeal(other);
                    break;
                case EffectType.DAMAGE:
                    DoDamage(other);
                    break;
                default:
                    break;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnDamage(Unit other)
    {
        if (Info().TriggerFlags.HasFlag(TriggerFlags.ON_DAMAGE))
        {
            DoEffectApply(other);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || AbilityInfo == null)
            return;

        Unit _unit = other.GetComponent<Unit>();
        this.other = _unit;

        if (this.other != owner && this.other != null && !other.CompareTag(gameObject.tag))
        {
            //Area Effects that dont persist should be treated as single target abilities that hit multiple targets
            if (Info().AbilityType == AbilityType.AREAOFEFFECT && Info().EffectTickSpeed != 0)
            {
                if (!others.Contains(_unit))
                {
                    others.Add(_unit);
                }
            }
            else
            {
                OnHit(this.other);
            }
        }

        if ((other.CompareTag("MapObject") || other.gameObject.layer == LayerMask.NameToLayer("Terrain")) && Info().AbilityType != AbilityType.AREAOFEFFECT)
        {
            //Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
            return;

        Unit _unit = other.GetComponent<Unit>();

        if (others.Contains(_unit))
        {
            others.Remove(_unit);
        }
    }

    public float GetRemainingTime()
    {
        return Mathf.Max(EffectTimeApplied + AbilityInfo.EffectDuration - Time.time, 0);
    }

    public override string ToString()
    {
        string output = AbilityInfo.AbilityName;

        if (AbilityInfo.EffectStackMax > 0)
        {
            output += " x" + EffectStackCount;
        }

        if (AbilityInfo.EffectDuration > 0)
        {
            output += " | " + GetRemainingTime().ToString("0.0");
        }

        return output;
    }
}
