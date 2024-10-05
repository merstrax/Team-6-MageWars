using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public enum AbilityType
    {
        PROJECTILE,
        AREAOFEFFECT,
        OVERTIME,
        STATUS,
        MOVEMENT,
        TELEPORT,
        JUMP
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
    [SerializeField] protected AbilityStats stats;

    [Header("Ability Info")]
    [SerializeField] protected string abilityName;
    [SerializeField] protected float cooldown;
    [SerializeField] protected int chargesMax;
    [SerializeField] protected float castTime;
    [SerializeField] protected CastType castType;
    [SerializeField] protected AbilityType abilityType;
    [SerializeField] protected ElementType elementType;
    [SerializeField] protected StatusEffect statusEffect;

    [Header("Ability Damages")]
    [SerializeField] protected float damageAmount;
    [SerializeField] protected float tickSpeed;
    [SerializeField] protected float duration;

    [Header("Ability Movement")]
    [SerializeField] protected float speed;
    [SerializeField] protected float range;
    [SerializeField] protected bool isTarget;

    public CastType GetCastType() { return castType; }
    public AbilityType GetAbilityType() {  return abilityType; }

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
        chargesCurrent = chargesMax;
    }

    public string GetName()
    {
        return abilityName;
    }

    public void SetAsHandler(Unit owner)
    {
        if(myCollider != null)
            myCollider.enabled = false;
        if (myVisual != null)
            myVisual.SetActive(false);
        this.owner = owner;

        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
            audioSource.Pause();
    }

    public bool IsMovementAbility()
    {
        return abilityType == AbilityType.MOVEMENT || abilityType == AbilityType.TELEPORT;
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
            Destroy(gameObject);
        }

        UpdateCooldown();
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
                    cooldownStart = Time.time;
                }
            }
            canCast = true;
        }
    }

    //Cooldown Handling
    public bool ReadyToCast()
    {
        return canCast && HasChargesRemaining();
    }

    public void StartCooldown()
    {
        if (HasCharges())
        {
            if (chargesCurrent == chargesMax)
            {
                cooldownStart = Time.time;
            }
            ConsumeCharge();
        }
        else
        {
            cooldownStart = Time.time;
            canCast = false;
        }
    }

    public int CooldownRemaining()
    {
        return Mathf.RoundToInt(cooldownStart + cooldown - Time.time);
    }

    //Charges Handling
    bool HasCharges()
    {
        return chargesMax > 0;
    }

    bool HasChargesRemaining()
    {
        return chargesCurrent > 0 || !HasCharges();
    }

    void ConsumeCharge()
    {
        chargesCurrent = Mathf.Max(chargesCurrent - 1, 0);
    }

    bool HasMaxCharges()
    {
        return chargesCurrent == chargesMax;
    }

    public void GainCharge(int amount = 1)
    {
        chargesCurrent = Mathf.Min(chargesCurrent + amount, chargesMax);
    }

    //Basic Cast Handling
    public virtual void StartCast(Unit owner, Vector3 lookAt)
    {
        this.owner = owner;
        isCasting = true;
        castStartTime = Time.time;

        if (castType == CastType.INSTANT)
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
    }

    protected virtual void Cast(Transform end = null)
    {
        transform.LookAt(castTarget);
        myRigidbody.velocity = transform.forward * speed;

        CleanUp();
    }

    protected virtual void CleanUp()
    {
        Destroy(gameObject, duration);
    }

    //Movement Ability Handling
    public virtual void CastMovement()
    {
        StartCoroutine(DashMovement());
        OnCast();
    }

    IEnumerator DashMovement()
    {
        CharacterController _controller = owner.GetComponent<CharacterController>();
        if (_controller != null)
        {
            float startTime = Time.time;
            Vector3 _direction = owner.GetMoveDir() * speed;

            while (Time.time < startTime + duration)
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

        if (abilityType == AbilityType.PROJECTILE)
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
        if (damageAmount > 0)
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
