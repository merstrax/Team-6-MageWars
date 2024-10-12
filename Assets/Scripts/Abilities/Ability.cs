using System.Collections;
using System.Collections.Generic;
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
    public AbilityType GetAbilityType() {  return AbilityInfo.AbilityType; }

    protected Unit owner;
    protected Unit other;
    Vector3 castTarget;

    private int EffectStackCount = 1;
    public int GetStacks() { return EffectStackCount; }
    private float EffectTimeApplied = 0;
    private float EffectLastTick;

    [Header("Ability Components")]
    [SerializeField] protected SphereCollider myCollider;
    [SerializeField] protected GameObject myVisual;
    [SerializeField] protected Rigidbody myRigidbody;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        EffectTimeApplied = Time.time;

        if (AbilityInfo.AbilityType == AbilityType.AREAOFEFFECT && AbilityInfo.EffectTickSpeed == 0)
            AbilityInfo.EffectTickSpeed = 0.1f;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (owner == null)
        {
            Destroy(gameObject);
        }

        if(AbilityInfo.AbilityType == AbilityType.EFFECT)
            UpdateEffect();
    }

    protected virtual void UpdateEffect()
    {
        if (AbilityInfo.AbilityType == AbilityType.EFFECT)
        {
            if (AbilityInfo.EffectType == EffectType.DAMAGE)
            {
                if (EffectLastTick + AbilityInfo.EffectTickSpeed < Time.time)
                {
                    DoDamage(other);
                    EffectLastTick = Time.time;
                }
            }

            if (EffectTimeApplied + AbilityInfo.EffectDuration < Time.time)
            {
                CleanUpEffect();
            }
        }
    }

    public void SetOwner(Unit owner)
    {
        this.owner = owner;
    }

    public void SetOther(Unit other)
    {
        this.other = other;
    }

    //Basic Cast Handling
    public virtual void StartCast(Unit owner, Vector3 lookAt)
    {
        this.owner = owner;
        owner.ProccessEvent(TriggerFlags.ON_CAST, other, this);

        if (AbilityInfo.CastType == CastType.INSTANT)
        {
            castTarget = lookAt;
            FinishCast();
        }
        else
        {
            Debug.Log("Wow");
        }
    }

    public virtual void InterruptCast()
    {
    }

    public virtual void FinishCast()
    {
        Cast();
        OnCast();
        CleanUp();
    }

    protected virtual void Cast(Transform end = null)
    {
        if (myRigidbody != null && !Info().IsTarget) {
            transform.LookAt(castTarget);
            myRigidbody.velocity = transform.forward * AbilityInfo.AbilitySpeed;
        }
        else
        {
            transform.position = castTarget;
        }
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
                if(owner != null)
                owner.RemoveEffect(this);
                break;
            case EffectTargetType.OTHER:
                if(other != null)
                other.RemoveEffect(this);
                break;
            default:
                break;
        }
    }

    //Movement Ability Handling
    public virtual void CastMovement()
    {
        if (owner.TryGetComponent<PlayerMovementController>(out PlayerMovementController controller))
        {
            StartCoroutine(controller.DashMovement(Info().AbilitySpeed, Info().EffectDuration));
            OnCast();
        }
        CleanUp();
    }

    //Damage Handlers
    public virtual Damage CalculatedDamage()
    {
        if (owner == null) return new Damage();

        Damage _calcDamage = owner.CalculateDamage(AbilityInfo.EffectAmount, AbilityInfo.DamageCoefficent);
        _calcDamage.Amount *= EffectStackCount;

        return _calcDamage;
    }

    public virtual void DoDamage(Unit other)
    {
        Damage _damage = CalculatedDamage();

        if (other != null)
        {
            other.TakeDamage(_damage, this, owner);

            OnDamage();
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

            toApply.SetAbilityInfo(AbilityInfo.EffectAbility);
            toApply.SetOwner(owner);
            toApply.SetOther(other);

            switch(toApply.AbilityInfo.EffectTargetType)
            {
                case EffectTargetType.OWNER:
                    owner.ApplyEffect(toApply);
                    break;
                case EffectTargetType.OTHER:
                    other.ApplyEffect(toApply);
                    break;
                default:
                    break;
            }
        }
    }

    public virtual void AddStack()
    {
        if(AbilityInfo.EffectStackMax > 0)
        {
            if(EffectStackCount < AbilityInfo.EffectStackMax)
            {
                EffectStackCount++;
            }
        }
        
        //Reset effect duration
        EffectTimeApplied = Time.time;
    }

    //Triggers
    protected virtual void OnCast() 
    {
        //Used for scripts
        owner.ProccessEvent(TriggerFlags.ON_CAST, other, this);
    }

    protected virtual void OnHit(Unit other)
    {
        if (AbilityInfo.EffectAmount > 0)
        {
            if(AbilityInfo.TriggerFlags.HasFlag(TriggerFlags.ON_HIT))
            {
                DoEffectApply(other);
            }
            owner.ProccessEvent(TriggerFlags.ON_HIT, other, this);

            DoDamage(other);
        }else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDamage(float amount = 0.0f)
    {
        //Used for scripts
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        this.other = other.GetComponent<Unit>();

        if (this.other != owner && this.other != null)
        {
            OnHit(this.other);
        }

        if(other.CompareTag("MapObject") && Info().AbilityType != AbilityType.AREAOFEFFECT)
        {
            Destroy(gameObject);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
        }
    }

    public float GetRemainingTime()
    {
        return Mathf.Max(EffectTimeApplied + AbilityInfo.EffectDuration - Time.time, 0);
    }

    public override string ToString()
    {
        string output = AbilityInfo.AbilityName;

        if(AbilityInfo.EffectStackMax > 0)
        {
            output += " x" + EffectStackCount;
        }

        if(AbilityInfo.EffectDuration > 0)
        {
            output += " | " + GetRemainingTime().ToString("0.0");
        }

        return output;
    }
}
