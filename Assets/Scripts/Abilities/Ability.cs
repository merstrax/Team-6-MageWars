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

    public CastType GetCastType() { return castType; }
    public AbilityType GetAbilityType() {  return abilityType; }

    Unit owner;
    Unit other;

    bool isCasting;
    float castStartTime;
    float cooldownStart;
    bool canCast = true;
    Vector3 castTarget;

    int chargesCurrent;

    [Header("Ability Components")]
    [SerializeField] Collider myCollider;
    [SerializeField] Renderer myRenderer;
    [SerializeField] Rigidbody myRigidbody;

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
        if(myRenderer != null)
            myRenderer.enabled = false;
        if(abilityType != AbilityType.MOVEMENT)
            speed = 0f;
        this.owner = owner;
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
    }

    //Movement Ability Handling
    public virtual void CastMovement(playerController.InputDirection inputDirection = playerController.InputDirection.NONE)
    {
        StartCoroutine(DashMovement(inputDirection));
        OnCast();
    }

    IEnumerator DashMovement(playerController.InputDirection inputDirection = playerController.InputDirection.NONE)
    {
        CharacterController _controller = owner.GetComponent<CharacterController>();
        if (_controller != null)
        {
            float startTime = Time.time;
            Vector3 _direction = Vector3.zero;

            switch (inputDirection)
            {
                case playerController.InputDirection.UP:
                    _direction = (_controller.transform.forward * speed);
                    break;
                case playerController.InputDirection.DOWN:
                    _direction = (-1 * _controller.transform.forward) * speed;
                    break;
                case playerController.InputDirection.LEFT:
                    _direction = (-1 * _controller.transform.right) * speed;
                    break;
                case playerController.InputDirection.RIGHT:
                    _direction = (_controller.transform.right * speed);
                    break;
                default:
                    _direction = owner.GetMoveDir() * speed;
                    break;
            }

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
