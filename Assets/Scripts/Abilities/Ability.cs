using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    [Header("Ability Stats")]
    [SerializeField] protected AbilityStats stats;

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
    public EffectTargetType EffectTargetType;
    public EffectStatusType StatusType;
    public EffectAttributeFlags AttributeFlags;
    public EffectElementFlags ElementFlags;
    public EffectTriggerFlags TriggerFlags;
    public EffectModifierType ModifierType;
    public Ability EffectAbility;
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

    [Header("Ability Movement")]
    [SerializeField] protected float speed;
    [SerializeField] protected float range;
    [SerializeField] protected bool isTarget;

    public int GetID() { return AbilityID; }
    public CastType GetCastType() { return CastType; }
    public AbilityType GetAbilityType() {  return AbilityType; }

    protected Unit owner;
    protected Unit other;

    bool isCasting;
    float castStartTime;
    float cooldownStart;
    bool canCast = true;
    Vector3 castTarget;

    int chargesCurrent;

    private int EffectStackCount = 0;
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

        if(AbilityType == AbilityType.EFFECT)
            UpdateEffect();
    }

    protected virtual void UpdateEffect()
    {
        if (AbilityType == AbilityType.EFFECT)
        {
            if (EffectType == EffectType.DAMAGE)
            {
                if (EffectLastTick + EffectTickSpeed < Time.time)
                {
                    DoDamage();
                    Debug.Log(AbilityName + ": Dealt " + CalculatedDamage() + " Damage");
                    EffectLastTick = Time.time;
                }
            }

            if (EffectTimeApplied + EffectDuration < Time.time)
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

        Debug.Log("Spell Cast");

        if (CastType == CastType.INSTANT)
        {
            castTarget = lookAt;
            FinishCast();
        }
        else
        {

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
        transform.LookAt(castTarget);
        myRigidbody.velocity = transform.forward * speed;
    }

    public virtual void CleanUp(bool instant = false)
    {
        if (instant)
        {
            Destroy(gameObject);
            return;
        }
        Destroy(gameObject, EffectDuration);
    }

    public virtual void CleanUpEffect()
    {
        switch (EffectTargetType)
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
            Vector3 _direction = owner.GetMoveDir() * speed;

            while (Time.time < startTime + EffectDuration)
            {
                _controller.Move(_direction * Time.deltaTime);
                yield return null;
            }
        }
    }

    //Damage Handlers
    public virtual float CalculatedDamage()
    {
        if (owner == null) return 0f;

        float _calcDamage = owner.GetDamageModifier() * EffectAmount;

        if(EffectStackCount > 1)
        {
            _calcDamage *= EffectStackCount;
        }

        return _calcDamage;
    }

    public virtual void DoDamage()
    {
        float _damage = CalculatedDamage();

        if (other != null)
        {
            other.TakeDamage(_damage, owner);

            OnDamage();
        }

        if (AbilityType == AbilityType.PROJECTILE)
        {
            Destroy(gameObject);
        }
    }

    public virtual void DoEffectApply()
    {
        bool isSuccess = Random.Range(0, 1) < EffectTriggerChance;
        if (isSuccess && EffectAbility != null)
        {
            Ability toApply = Instantiate(EffectAbility);
            toApply.SetOwner(owner);
            toApply.SetOther(other);

            switch(toApply.EffectTargetType)
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
        if(EffectStackMax > 0)
        {
            if(EffectStackCount < EffectStackMax)
            {
                EffectStackCount++;
                Debug.Log(AbilityName + ": Stacks increased to " + EffectStackCount);
            }
        }
        
        //Reset effect duration
        EffectTimeApplied = Time.time;
        Debug.Log(AbilityName + ": Reset Time Applied with " + ((EffectTimeApplied + EffectDuration) - Time.time) + " Seconds Remaining");
    }

    //Triggers
    protected virtual void OnCast() 
    {
        //Used for scripts
        owner.OnCast();
    }

    protected virtual void OnHit()
    {
        if (EffectAmount > 0)
        {
            if(TriggerFlags.HasFlag(EffectTriggerFlags.ON_HIT))
            {
                Debug.Log("OnHit Started");
                DoEffectApply();
                Debug.Log("OnHit Ended");
            }
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
            Destroy(gameObject);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
        }
    }
}
