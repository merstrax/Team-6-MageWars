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

    [SerializeField] protected bool isTarget;

    public int GetID() { return AbilityInfo.AbilityID; }
    public CastType GetCastType() { return AbilityInfo.CastType; }
    public AbilityType GetAbilityType() {  return AbilityInfo.AbilityType; }

    protected Unit owner;
    protected Unit other;

    bool isCasting;
    float castStartTime;
    float cooldownStart;
    bool canCast = true;
    Vector3 castTarget;
    bool hasDamaged;

    int chargesCurrent;

    private int EffectStackCount = 1;
    public int GetStacks() { return EffectStackCount; }
    private float EffectTimeApplied = 0;
    private float EffectLastTick;

    [Header("Ability Components")]
    [SerializeField] protected Collider myCollider;
    [SerializeField] protected GameObject myVisual;
    [SerializeField] protected Rigidbody myRigidbody;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        EffectTimeApplied = Time.time;
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
                    DoDamage();
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
        isCasting = true;
        castStartTime = Time.time;

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
        isCasting = false;
    }

    public virtual void FinishCast()
    {
        Cast();
        OnCast();
        CleanUp();
    }

    protected virtual void Cast(Transform end = null)
    {
        if (myRigidbody != null && !isTarget) {
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
                owner.RemoveEffect(this);
                break;
            case EffectTargetType.OTHER:
                other.RemoveEffect(this);
                break;
            default:
                break;
        }
    }

    //Movement Ability Handling
    public virtual void CastMovement()
    {
        StartCoroutine(DashMovement());
        OnCast();
        CleanUp();
    }

    IEnumerator DashMovement()
    {
        if (owner.TryGetComponent<CharacterController>(out var _controller))
        {
            float startTime = Time.time;
            Vector3 _direction = owner.GetMoveDir() * AbilityInfo.AbilitySpeed;

            while (Time.time < startTime + AbilityInfo.EffectDuration)
            {
                _controller.Move(_direction * Time.deltaTime);
                yield return null;
            }
        }
    }

    //Damage Handlers
    public virtual Damage CalculatedDamage()
    {
        if (owner == null) return new Damage();

        Damage _calcDamage = owner.CalculateDamage(AbilityInfo.EffectAmount, AbilityInfo.DamageCoefficent);
        _calcDamage.Amount *= EffectStackCount;


        return _calcDamage;
    }

    public virtual void DoDamage()
    {
        Damage _damage = CalculatedDamage();

        if (other != null)
        {
            other.TakeDamage(_damage, owner);

            OnDamage();

        }

        if (AbilityInfo.AbilityType == AbilityType.PROJECTILE)
        {
            Destroy(gameObject, 1.0f);
        }
        hasDamaged = true;
    }

    public virtual void DoEffectApply()
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
                //Debug.Log(AbilityInfo.AbilityName + ": Stacks increased to " + EffectStackCount);
            }
        }
        
        //Reset effect duration
        EffectTimeApplied = Time.time;
        //Debug.Log(AbilityInfo.AbilityName + ": Reset Time Applied with " + ((EffectTimeApplied + AbilityInfo.EffectDuration) - Time.time) + " Seconds Remaining");
    }

    //Triggers
    protected virtual void OnCast() 
    {
        //Used for scripts
        owner.OnCast();
    }

    protected virtual void OnHit()
    {
        if (AbilityInfo.EffectAmount > 0)
        {
            if(AbilityInfo.TriggerFlags.HasFlag(EffectTriggerFlags.ON_HIT))
            {
                DoEffectApply();
            }
            if(!hasDamaged)
            DoDamage();
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
            OnHit();
        }

        if(other.CompareTag("MapObject"))
        {
            Destroy(gameObject, 0.5f);
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
