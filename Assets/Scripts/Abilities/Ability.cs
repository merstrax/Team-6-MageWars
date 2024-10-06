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

    [Header("Ability Movement")]
    [SerializeField] protected float speed;
    [SerializeField] protected float range;
    [SerializeField] protected bool isTarget;

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

    [Header("Ability Components")]
    [SerializeField] protected Collider myCollider;
    [SerializeField] protected GameObject myVisual;
    [SerializeField] protected Rigidbody myRigidbody;

    // Start is called before the first frame update
    protected virtual void Start()
    {
    }

    // Update is called once per frame
    protected virtual void Update()
    { 
    }

    public void SetOwner(Unit owner)
    {
        this.owner = owner;
    }

    //Basic Cast Handling
    public virtual void StartCast(Unit owner, Vector3 lookAt)
    {
        this.owner = owner;
        isCasting = true;
        castStartTime = Time.time;

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

    protected virtual void CleanUp()
    {
        Destroy(gameObject, EffectDuration);
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

        return owner.GetDamageModifier() * EffectAmount;
    }

    public virtual void DoDamage()
    {
        float _damage = CalculatedDamage();

        if (other != null)
        {
            other.TakeDamage(_damage);

            OnDamage();
        }

        if (AbilityType == AbilityType.PROJECTILE)
        {
            Destroy(gameObject);
        }
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
            Debug.Log("Hit detected");
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
