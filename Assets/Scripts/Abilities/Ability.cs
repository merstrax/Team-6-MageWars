using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public enum DamageType
    {
        PROJECTILE,
        AREAOFEFFECT,
        OVERTIME,
        STATUS
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

    public enum ControlType
    {
        NONE,
        STUN,
        SLOW,
        ROOT
    }

    [Header("Ability Stats")]
    [SerializeField] AbilityStats stats;

    [Header("Ability Info")]
    [SerializeField] string abilityName;
    [SerializeField] float cooldown;
    [SerializeField] int chargesMax;
    [SerializeField] DamageType damageType;
    [SerializeField] ElementType elementType;
    [SerializeField] ControlType controlType;

    [SerializeField] float damageAmount;
    [SerializeField] float tickSpeed;
    [SerializeField] float duration;

    // Start is called before the first frame update
    protected virtual void Start()
    {
       
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    public virtual void DoDamage()
    {

    }
}
