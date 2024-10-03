using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IDamage
{
    [SerializeField] protected string unitName;
    [SerializeField] protected float healthCurrent;
    [SerializeField] protected float healthMax;
    [SerializeField] protected float healthRegen;
    [SerializeField] protected float damageModifier;
    [SerializeField] protected float defenseModifier;

    [SerializeField] Transform castPos;
    public Transform GetCastPos() { return castPos; }

    private bool isCasting;
    private bool isSlowed;
    private bool isStunned;
    private bool isRooted;

    public bool IsCasting() {  return isCasting; }
    public bool IsSlowed() { return isSlowed; }
    public bool IsStunned() { return isStunned; }
    public bool IsRooted() { return isRooted; }

    public float GetDamageModifier() { return damageModifier; }
    public string GetUnitName() { return unitName; }

    public virtual void TakeDamage(float amount)
    {
        healthCurrent -= (amount * defenseModifier);

        if(healthCurrent <= 0)
        {
            OnDeath();
        }
    }

    public virtual void OnDeath()
    {
        Destroy(gameObject);
    }
}
