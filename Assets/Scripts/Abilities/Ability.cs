using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Ability : MonoBehaviour
{
    public enum AbilityType
    {
        PROJECTILE,
        AREAOFEFFECT,
        OVERTIME,
        STATUS,
        MOVEMENT
    }

    public enum ElementType
    {
        NONE,
        ARCANE,
        FROST,
        FIRE,
        WIND,
        PHYSICAL
    }

    public enum StatusEffect
    {
        NONE,
        STUN,
        SLOW,
        ROOT
    }

    public enum CastType
    {
        INSTANT,
        CASTTIME,
        CHANNEL,
        CHARGED
    }

    [Header("Ability Stats")]
    [SerializeField] AbilityStats stats;

    [Header("Ability Info")]
    [SerializeField] string abilityName;
    [SerializeField] float cooldown;
    [SerializeField] int chargesMax;
    [SerializeField] float castTime;
    [SerializeField] CastType castType;
    [SerializeField] AbilityType abilityType;
    [SerializeField] ElementType elementType;
    [SerializeField] StatusEffect statusEffect;

    [Header("Ability Damages")]
    [SerializeField] float damageAmount;
    [SerializeField] float tickSpeed;
    [SerializeField] float duration;

    [Header("Ability Movement")]
    [SerializeField] float speed;
    [SerializeField] float range;
    [SerializeField] bool isTarget;

    Unit owner;
    Unit other;

    bool isCasting;
    float castStartTime;

    Vector3 castTarget;

    [SerializeField] Collider myCollider;
    [SerializeField] Renderer myRenderer;
    [SerializeField] Rigidbody myRigidbody;

    public string GetName()
    {
        return abilityName;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(isCasting)
        {
            //TODO: Cast Logic
        }

        if (owner == null)
        {
            //Destroy(gameObject);
        }
    }

    public virtual void StartCast(Unit owner, Vector3 lookAt)
    {
        this.owner = owner;
        if (castType == CastType.INSTANT)
        {
            castTarget = lookAt;
            Debug.Log("Start Cast Ability: " + GetName());
            FinishCast();
        }
        isCasting = true;
        castStartTime = Time.time;
    }

    public virtual void InterruptCast()
    {
        isCasting = false;
    }

    public virtual void FinishCast()
    {
        Cast();
        OnCast();
    }

    protected virtual void Cast(Transform end = null)
    {
        //transform.localPosition = owner.GetCastPos().localPosition;
        Debug.Log("Owner Cast Ability: " + owner.GetUnitName());


        transform.LookAt(castTarget);
        myRigidbody.velocity = transform.forward * speed;
    }

    protected virtual void OnCast() 
    { 
        //Used for scripts
    }

    protected virtual void OnHit()
    {
        if (damageAmount > 0)
        {
            DoDamage();
        }else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDamage()
    {
        //Used for scripts
    }

    public virtual float CalculatedDamage()
    {
        if(owner == null) return 0f;

        return owner.GetDamageModifier() * damageAmount;
    }

    public virtual void DoDamage()
    {
        float _damage = CalculatedDamage();

        if (other != null)
        {
            other.TakeDamage(_damage);

            OnDamage();
        }
       
        if(abilityType == AbilityType.PROJECTILE)
        {
            Destroy(gameObject);
        }
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
