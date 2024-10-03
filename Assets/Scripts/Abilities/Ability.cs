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

        if (!canCast && CooldownRemaining() <= 0)
        {
            canCast = true;
            chargesCurrent = Mathf.Min(chargesCurrent + 1, chargesMax);
        }
    }

    public virtual bool ReadyToCast()
    {
        return canCast;
    }

    public virtual void StartCooldown()
    {
        cooldownStart = Time.time;
        canCast = false;
    }

    public virtual float CooldownRemaining()
    {
        return cooldownStart + cooldown - Time.time;
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
        transform.LookAt(castTarget);
        myRigidbody.velocity = transform.forward * speed;
    }

    public virtual void CastMovement(playerController.InputDirection inputDirection)
    {
        CharacterController _controller = owner.GetComponent<CharacterController>();
        if (_controller != null)
        {
            switch (inputDirection)
            {
                case playerController.InputDirection.UP:
                    _controller.Move(_controller.transform.forward * speed);
                    break;
                case playerController.InputDirection.DOWN:
                    _controller.Move((-1 * _controller.transform.forward) * speed);
                    break;
                case playerController.InputDirection.LEFT:
                    _controller.Move((-1 * _controller.transform.right) * speed);
                    break;
                case playerController.InputDirection.RIGHT:
                    _controller.Move(_controller.transform.right * speed);
                    break;
                default:
                    break;
            }
        }
        StartCooldown();
    }

    protected virtual void OnCast() 
    {
        //Used for scripts
        owner.OnCast();
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

    protected virtual void OnDamage(float amount = 0.0f)
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
