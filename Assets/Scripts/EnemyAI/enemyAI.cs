using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class EnemyAI : Unit, ITargetable
{
    [Header("Enemy Stats")]
    [SerializeField] protected enemyStats enemyStats;
    // Define AI States
    protected enum AIState { Idle, Reset, Chasing, Attacking, Dead }
    protected AIState currentState = AIState.Idle;

    [Header("Render Components")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected Renderer model;
    [SerializeField] protected CapsuleCollider bodyCollider;
    [SerializeField] protected CapsuleCollider headCollider;
    [SerializeField] protected Material targetMaterial;

    [Header("AI Nav")]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Transform headPos;
    [SerializeField] protected int faceTargetSpeed;

    [Header("Aggro and Roaming")]
    protected float aggroRange;
    protected float kiteRange;
    protected int viewAngle;

    [Header("Drop")]
    [SerializeField] protected GameObject[] dropPrefab;  
    protected float dropChance = 1;

    [Header("AI Abilities")]
    Ability[] abilityPassive;
    Ability[] abilities;
    float abilityRate;
    protected bool canCastAbility = true;
    AbilityHandler[] abilityHandlers;
    AbilityHandler abilityChosen;

    // Variables 
    protected Unit target;
    protected Vector3 targetDirection;
    protected Vector3 startPos;
    protected Vector3 lastAttackPosition;
    protected bool isAttacking;
    protected bool isDead;
    protected bool canAttack = true;

    protected override void Start()
    {
        healthRegen = enemyStats.healthRegen;
        healthBase = enemyStats.healthBase;
        damageBase = enemyStats.damageBase;
        defenseBase = enemyStats.defenseBase;
        speedBase = enemyStats.speedBase;
        critChanceBase = enemyStats.critChanceBase;
        critDamageBase = enemyStats.critDamageBase;
        cooldownBase = enemyStats.cooldownBase;
        abilityPassive = enemyStats.abilityPassive;
        abilities = enemyStats.abilities;
        abilityRate = enemyStats.abilityRate;
        aggroRange = enemyStats.aggroRange;
        kiteRange = enemyStats.kiteRange;
        dropChance = enemyStats.dropChance;
        viewAngle = enemyStats.viewAngle;

        startPos = transform.position;
        currentState = AIState.Idle;

        // keeps track of charges, CD, targeting type
        abilityHandlers = new AbilityHandler[abilities.Length];

        for (int i = 0; i < abilities.Length; i++)
        {
            if (abilities[i] != null)
            {
                abilityHandlers[i] = gameObject.AddComponent<AbilityHandler>();
                abilityHandlers[i].Setup(this, abilities[i]);
            }
        }

        base.Start();

        agent.speed = GetSpeed();
        SetupTarget(targetMaterial);
    }

    protected virtual void Update()
    {
        if (isDead) return;

        switch (currentState)
        {
            case AIState.Idle:
                IdleState();
                break;
            case AIState.Chasing:
                ChasingState();
                break;
            case AIState.Reset:
                ResetState();
                break;
            case AIState.Attacking:
                AttackingState();
                break;
            default:
                break;
        }
    }

    #region Movement and Targeting
    protected void IdleState()
    {
        if (IsTargetInRange() && IsTargetVisible())
        {
            currentState = AIState.Chasing;
        }
    }

    protected void ChasingState()
    {
        float distanceFromStart = Vector3.Distance(transform.position, startPos);

        if (distanceFromStart >= kiteRange)
        {
            currentState = AIState.Reset;
            healthCurrent = healthMax;
            ApplyStatus(StatusFlag.INVULNERABLE);

            RemoveAllEffects();

            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        if (AnyAbilityInRange(distanceToTarget))
        {
            currentState = AIState.Attacking;
        }
        else
        {
            MoveTowardsTarget();
        }
    }

    protected void ResetState()
    {
        agent.SetDestination(startPos);

        float distanceFromStart = Vector3.Distance(transform.position, startPos);

        if (distanceFromStart <= 0.01f)
        {
            target = null;
            currentState = AIState.Idle;
            RemoveStatus(StatusFlag.INVULNERABLE);
            RemoveAllEffects();
        }

        agent.speed = 10;
    }

    protected void MoveTowardsTarget()
    {
        if (target != null)
        {
            agent.SetDestination(target.gameObject.transform.position);
            animator.SetFloat("Speed", agent.velocity.magnitude / GetSpeed());
        }
    }

    protected bool IsTargetInRange()
    {
        Unit[] unitList = FindObjectsOfType<Unit>();
        float distanceToTarget;

        foreach (Unit unit in unitList)
        {
            if (unit.CompareTag(tag)) continue;
            distanceToTarget = Vector3.Distance(transform.position, unit.transform.position);
            target = unit;
            return distanceToTarget <= aggroRange;
        }

        target = null;
        return false;
    }

    protected bool IsTargetVisible()
    {
        Vector3 directionToTarget = (target.transform.position - headPos.position).normalized;
        float angleToTarget = Vector3.Angle(directionToTarget, transform.forward);

        if (angleToTarget < viewAngle)
        {
            return true;
        }
        return false;
    }

    protected void FaceTarget()
    {
        targetDirection = (GameManager.instance.player.transform.position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
    #endregion

    #region Attack Handling
    protected void AttackingState()
    {
        // Fetch ability>cast>CD
        abilityChosen = ChooseAttack();
        if (abilityChosen != null && canCastAbility && target != null && !IsStunned())
        {

            string animation = animations[abilityChosen.GetAbility().Info().AnimationType];
            StartCoroutine(Attack(animation));
            abilityChosen.StartCooldown();
        }

        // Handle attacking through coroutines
        float distanceToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

        if (!AnyAbilityInRange(distanceToPlayer))
        {
            currentState = AIState.Chasing;
        }
    }

    private void CastAbility(AbilityHandler ability)
    {
        Ability _ability = Instantiate(ability.GetAbility(), GetCastPos(ability.GetAbility().Info().CastPosition).position, transform.rotation);
        _ability.SetOwner(this);

        Vector3 toCastPos = target.gameObject.transform.position;

        if (_ability.Info().AbilityType == AbilityType.PROJECTILE)
        {
            toCastPos = target.gameObject.transform.position;
            toCastPos.y += (target.GetComponent<CapsuleCollider>().height * 0.75f);
        }

        _ability.StartCast(this, toCastPos);
    }

    protected virtual IEnumerator Attack(string animation)
    {
        canCastAbility = false;
        animator.SetLayerWeight(animator.GetLayerIndex("Attack"), 1);
        animator.SetLayerWeight(animator.GetLayerIndex("Movement"), 0);
        animator.SetTrigger(animation);

        yield return new WaitForSeconds(abilityRate);

        canCastAbility = true;
        animator.SetLayerWeight(animator.GetLayerIndex("Attack"), 0);
        animator.SetLayerWeight(animator.GetLayerIndex("Movement"), 1);
    }

    public void CastByAnimation()
    {
        CastAbility(abilityChosen);
    }

    protected virtual AbilityHandler ChooseAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

        foreach (AbilityHandler abilityHandler in abilityHandlers)
        {
            float abilityDist = abilityHandler.GetAbility().Info().AbilityRange;

            if (abilityHandler.ReadyToCast() && abilityDist > distanceToPlayer)
            {
                return abilityHandler;
            }
        }

        return null;
    }

    protected bool AnyAbilityInRange(float dist)
    {
        foreach (AbilityHandler abilityHandler in abilityHandlers)
        {
            float abilityDist = abilityHandler.GetAbility().Info().AbilityRange;

            if (abilityHandler.ReadyToCast() && abilityDist > dist)
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Death Events
    public override void OnDeath(Unit other = null, Ability source = null, Damage damage = default)
    {
        isDead = true;
        ApplyStatus(StatusFlag.INVULNERABLE);
        agent.speed = 0;
        agent.isStopped = true;     
        animator.SetTrigger("Death"); 
    }

    public void DeathAfterAnimation()
    {
        TryDropPickup(); 
        base.OnDeath(); 
    }

    protected void TryDropPickup()
    {
        // Health drop logic - chance to drop health item
        float randomValue = Random.Range(0f, 1f);
        if (randomValue >= dropChance && dropPrefab != null && dropPrefab.Length > 0)
        {
            int randomIndex = Random.Range(0, dropPrefab.Length);

            Instantiate(dropPrefab[randomIndex], transform.position, Quaternion.identity);
        }
    }
    #endregion

    #region Trigger Events
    public override void OnDamaged(Unit other = null, Ability source = null, Damage damage = default)
    {
        animator.SetTrigger("Hit");
        target = other;

        currentState = AIState.Chasing; 
    }

    public override void OnSlow(Unit other = null, Ability source = null, Damage damage = default)
    {
        agent.speed = GetSpeed();
    }

    public override void OnStun(Unit other = null, Ability source = null, Damage damage = default)
    {
        agent.speed = GetSpeed();
    }

    public override void OnRoot(Unit other = null, Ability source = null, Damage damage = default)
    {
        agent.speed = GetSpeed();
    }
    #endregion

    #region Targetable Implementation
    public Material TargetMaterial { get; set; }
    public bool IsTargetDisabled { get; set; }

    public void SetupTarget(Material resourceMaterial)
    {
        TargetMaterial = Instantiate(resourceMaterial);
        if (TargetMaterial != null)
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                List<Material> materials = new() { TargetMaterial };
                materials.AddRange(renderer.materials);
                renderer.SetMaterials(materials);
            }

            TargetMaterial.SetFloat("_OutlineWidth", 0f);
        }
    }

    public void OnTarget(bool setTarget)
    {
        if (TargetMaterial == null) return;

        if (setTarget && !IsTargetDisabled)
        {
            TargetMaterial.SetFloat("_OutlineWidth", 0.075f);
        }
        else
        {
            TargetMaterial.SetFloat("_OutlineWidth", 0f);
        }
    }

    public GameObject GameObject()
    {
        return gameObject;
    }
    #endregion
}
