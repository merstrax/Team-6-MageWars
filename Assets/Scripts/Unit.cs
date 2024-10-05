using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IDamage
{
    [Header("Unit Scriptable Object - TODO")]

    [Header("Unit Info")]
    [SerializeField] protected string unitName;
    [SerializeField] protected float healthCurrent;
    [SerializeField] protected float healthMax;
    [SerializeField] protected float healthRegen;
    [SerializeField] protected float damageModifier;
    [SerializeField] protected float defenseModifier;

    public string GetUnitName() { return unitName; }
    public float GetHealthCurrent() { return healthCurrent; }
    public void SetHealthCurrent(float healthCurrent) { this.healthCurrent = healthCurrent; }

    public float GetHealthMax() { return healthMax; }
    public void SetHealthMax(float healthMax) { this.healthMax = healthMax; }

    public float GetHealthRegen() { return healthRegen; }
    public void SetHealthRegen(float healthRegen) { this.healthRegen = healthRegen; }

    public float GetDamageModifier() { return damageModifier; }
    public void SetDamageModifier(float damageModifier) { this.damageModifier = damageModifier; }

    public float GetDefenseModifier() { return defenseModifier; }
    public void SetDefenseModifier(float defenseModifier) { this.defenseModifier = defenseModifier; }

    [Header("Ability Position")]
    [SerializeField] Transform castPos;
    public Transform GetCastPos() { return castPos; }

    //Status Effects
    private bool isCasting;
    private bool isSlowed;
    private bool isStunned;
    private bool isRooted;

    public bool IsCasting() { return isCasting; }
    public void SetCasting(bool isCasting) { this.isCasting = isCasting; }

    public bool IsSlowed() { return isSlowed; }
    public void SetSlowed(bool isSlowed) { this.isSlowed = isSlowed; }

    public bool IsStunned() { return isStunned; }
    public void SetStunned(bool isStunned) { this.isStunned = isStunned; }

    public bool IsRooted() { return isRooted; }
    public void SetRooted(bool isRooted) { this.isRooted = isRooted; }

    protected Vector3 moveDir;
    public Vector3 GetMoveDir() { return moveDir; }
    public void SetMoveDir(Vector3 moveDir) { this.moveDir = moveDir; }
    public void UpdateMoveDir(Vector2 move) 
    { 
        moveDir = move.x * transform.right +
                    move.y * transform.forward;
    }

    public virtual void InterruptCasting()
    {
        SetCasting(false);
        OnInterrupted();
    }

    //Overridable but not recommended
    public virtual void TakeDamage(float amount, Unit other = null)
    {
        healthCurrent -= (amount * defenseModifier);

        if (other != null)
        {
            other.OnDamage(amount, this);
        }
        
        OnDamaged(amount, other);

        if(healthCurrent <= 0)
        {
            OnDeath(other);
        }
    }

    //Scriptable functions
    public virtual void OnHit(Unit other, Ability ability) { }
    public virtual void OnDamage(float amount, Unit other = null) { }
    public virtual void OnDamaged(float amount, Unit other = null){ }
    public virtual void OnHealed(float amount, GameObject obj = null) { }
    public virtual void OnKill(Unit other){ }
    public virtual void OnInterrupted(Unit other = null){ }
    public virtual void OnCast(Ability ability = null) { }
    public virtual void OnDeath(Unit other = null)
    {
        if(other != null) { other.OnKill(this); }
        Destroy(gameObject);
    }
}
