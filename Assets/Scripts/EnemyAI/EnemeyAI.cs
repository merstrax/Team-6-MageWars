using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


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
    [SerializeField] protected Material targetMaterial;

    [Header("AI Nav")]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Transform headPos;
    [SerializeField] protected int faceTargetSpeed;
    [SerializeField] protected LayerMask groundMask;

    [Header("Aggro and Roaming")]
    protected float aggroRange;
    protected float kiteRange;
    protected float roamRange;
    protected float roamTimer;
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
    protected Vector3 roamPos;
    protected bool isAttacking;
    protected bool isDead;
    protected bool canAttack = true;

    // Coroutines
    private Coroutine roamCoroutine;

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
        roamRange = enemyStats.roamRange;
        roamTimer = enemyStats.roamTimer;
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

    float distanceToPlayer;
    protected virtual void Update()
    { 
        distanceToPlayer = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
        if (isDead || distanceToPlayer >= 100.0f) return;

        if (abilityCooldown != null && animator.GetLayerWeight(animator.GetLayerIndex("Attack")) >= 0.001f)
        {
            float attackWeight = animator.GetLayerWeight(animator.GetLayerIndex("Attack"));
            float movWeight = animator.GetLayerWeight(animator.GetLayerIndex("Movement"));
            animator.SetLayerWeight(animator.GetLayerIndex("Attack"), Mathf.Lerp(attackWeight, 0, Time.deltaTime * 5f));
            animator.SetLayerWeight(animator.GetLayerIndex("Movement"), Mathf.Lerp(movWeight, 1, Time.deltaTime * 5f));
        }

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
        if (roamCoroutine == null)
        {
            roamCoroutine = StartCoroutine(Roam());
        }

        if (agent.remainingDistance < 1.0f)
        {
            agent.isStopped = true;
            animator.SetFloat("Speed", 0.0f);
        }

        if (distanceToPlayer <= aggroRange && IsTargetVisible())
        {
            currentState = AIState.Chasing;
            agent.isStopped = false;
            if (roamCoroutine != null)
            {
                StopCoroutine(roamCoroutine);
                roamCoroutine = null;
            }
        }
    }

    protected void ChasingState()
    {
        float distanceFromStart = Vector3.Distance(transform.position, startPos);

        if (distanceFromStart >= kiteRange)
        {
            ResetStart();
            return;
        }

        if (AnyAbilityInRange(distanceToPlayer))
        {
            currentState = AIState.Attacking;
            animator.SetFloat("Speed", 0.0f);
        }
        else
        {
            if (distanceToPlayer >= agent.stoppingDistance)
                MoveTowardsTarget();
            else
                animator.SetFloat("Speed", 0.0f);
        }
    }

    protected void ResetStart()
    {
        agent.isStopped = false;
        agent.speed = GetSpeed() * 2;
        agent.SetDestination(startPos);
        currentState = AIState.Reset;

        ApplyStatus(StatusFlag.INVULNERABLE);
        RemoveAllEffects();
    }

    protected void ResetState()
    {
        float distanceFromStart = Vector3.Distance(transform.position, startPos);
        if (distanceFromStart <= 1.0f)
        {
            target = null;
            ResetEnd();
            return;
        }
    }

    protected virtual void ResetEnd()
    {
        healthRegen = enemyStats.healthRegen;
        healthBase = enemyStats.healthBase;
        damageBase = enemyStats.damageBase;
        defenseBase = enemyStats.defenseBase;
        speedBase = enemyStats.speedBase;
        critChanceBase = enemyStats.critChanceBase;
        critDamageBase = enemyStats.critDamageBase;
        cooldownBase = enemyStats.cooldownBase;

        currentState = AIState.Idle;

        RemoveStatus(StatusFlag.INVULNERABLE);
        RemoveAllEffects();

        stats = new(healthBase, damageBase, defenseBase, speedBase, critChanceBase, critDamageBase, cooldownBase);

        UpdateStats();
        healthCurrent = healthMax;

        UpdateInterface();
    }

    protected void MoveTowardsTarget()
    {
        NavMeshPath path = new();
        agent.CalculatePath(PlayerController.instance.transform.position, path);
        agent.SetPath(path);
        animator.SetFloat("Speed", agent.velocity.magnitude / GetSpeed());
    }

    protected bool IsTargetInRange()
    {
        return distanceToPlayer <= aggroRange;
    }

    protected bool IsTargetVisible()
    {
        Vector3 directionToTarget = (PlayerController.instance.transform.position - headPos.position).normalized;
        float angleToTarget = Vector3.Angle(directionToTarget, transform.forward);

        if (angleToTarget < viewAngle)
        {
            return true;
        }
        return false;
    }

    protected void FaceTarget()
    {
        targetDirection = (PlayerController.instance.transform.position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    private IEnumerator Roam()
    {
        // Generate random offset withing the roam range
        float randomX = Random.Range(-roamRange, roamRange);
        float randomZ = Random.Range(-roamRange, roamRange);

        // Calculate new position 
        roamPos = startPos + new Vector3(randomX, 0, randomZ);

        if (Physics.Raycast(roamPos, Vector3.down, out RaycastHit ray, agent.height * 0.5f + 0.2f, groundMask))
        {
            roamPos.y = ray.point.y;
        }
        else if (Physics.Raycast(roamPos, Vector3.up, out ray, agent.height * 0.5f + 0.2f, groundMask))
        {
            roamPos.y = ray.point.y;
        }

        // Move the AI to the new position
        agent.isStopped = false;
        NavMeshPath path = new();
        agent.CalculatePath(roamPos, path);
        agent.SetPath(path);
        animator.SetFloat("Speed", 0.5f);

        // Wait for the specified roam timer before the next movement
        yield return new WaitForSeconds(roamTimer);

        roamCoroutine = null;
    }
    #endregion

    #region Attack Handling
    protected void AttackingState()
    {
        // stop movement while attacking
        agent.isStopped = true;

        // Fetch ability>cast>CD
        abilityChosen = ChooseAttack();
        FaceTarget();
        if (abilityChosen != null && canCastAbility && !IsStunned())
        {
            CastAbility(abilityChosen);
        }

        // Handle attacking through coroutine
        if (!AnyAbilityInRange(distanceToPlayer))
        {
            agent.isStopped = false;
            currentState = AIState.Chasing;
        }
        else if (distanceToPlayer > aggroRange)
        {
            currentState = AIState.Reset;
        }
    }

    Ability _ability;
    private void CastAbility(AbilityHandler ability)
    {
        _ability = Instantiate(ability.GetAbility(), GetCastPos(ability.GetAbility().Info().CastPosition).position, transform.rotation);
        _ability.SetOwner(this);

        Vector3 toCastPos = PlayerController.instance.transform.position;

        if (_ability.Info().AbilityType == AbilityType.PROJECTILE)
        {
            toCastPos.y += (PlayerController.instance.GetComponent<CapsuleCollider>().height * 0.75f);
        }
        canCastAbility = false;
        _ability.CastStart(this, toCastPos);
    }

    public override void OnCastStart(Unit other = null, Ability source = null, Damage damage = default)
    {
        string animation = animations[source.Info().AnimationType];
        animator.SetLayerWeight(animator.GetLayerIndex("Attack"), 1);
        animator.SetLayerWeight(animator.GetLayerIndex("Movement"), 0);
        animator.SetTrigger(animation);
    }

    protected virtual IEnumerator Attack()
    {
        yield return new WaitForSeconds(abilityRate);

        canCastAbility = true;
        abilityChosen = null;
        abilityCooldown = null;
    }

    public void CastByAnimation()
    {
        if (_ability != null)
            _ability.Cast(GetCastPos(_ability.Info().CastPosition), PlayerController.instance.transform.position);
    }

    Coroutine abilityCooldown;

    public override void OnCastEnd(Unit other = null, Ability source = null, Damage damage = default)
    {
        if (abilityChosen != null)
        {
            abilityChosen.StartCooldown();
        }

        abilityCooldown = StartCoroutine(Attack());
    }

    protected virtual AbilityHandler ChooseAttack()
    {
        foreach (AbilityHandler abilityHandler in abilityHandlers)
        {
            float abilityDist = abilityHandler.GetAbility().Info().AbilityRange;

            if (abilityHandler.ReadyToCast() && abilityDist >= distanceToPlayer)
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

        agent.isStopped = false;
        currentState = AIState.Chasing;
    }

    public override void OnSlow(Unit other = null, Ability source = null, Damage damage = default)
    {
        agent.speed = GetSpeed();
    }

    public override void OnStun(Unit other = null, Ability source = null, Damage damage = default)
    {
        animator.SetTrigger("Stun");
        agent.speed = GetSpeed();
    }

    public override void OnStunEnd(Unit other = null, Ability source = null, Damage damage = default)
    {
        animator.SetTrigger("StunEnd");
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
        if (gameObject == null) return null;

        return gameObject;
    }
    #endregion
}
