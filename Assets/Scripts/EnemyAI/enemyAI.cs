using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class enemyAI : Unit
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

    // Variables 
    protected Unit target;
    protected Vector3 playerDir;
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

        animator = GetComponent<Animator>(); 
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

    // --- State Methods ---
    protected void IdleState()
    {
        animator.SetBool("isMoving", false);
        if (IsPlayerInRange() && IsPlayerVisible())
        {
            currentState = AIState.Chasing;
        }
    }

    protected void ResetState()
    {
        agent.SetDestination(startPos);
        animator.SetBool("isMoving", true);

        float distanceFromStart = Vector3.Distance(transform.position, startPos);

        if (distanceFromStart <= 0.01f)
        {
            currentState = AIState.Idle;
            RemoveStatus(StatusFlag.INVULNERABLE);
            RemoveAllEffects();
        }

        agent.speed = 10;
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

        float distanceToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

        if (AnyAbilityInRange(distanceToPlayer))
        {
            currentState = AIState.Attacking;
        }
        else
        {
            MoveTowardsPlayer();
        }
    }

    protected void AttackingState()
    {
        animator.SetBool("isMoving", false);
        // Fetch ability>cast>CD
        AbilityHandler abilityHandler = ChooseAttack();
        if (abilityHandler != null && canCastAbility && target != null && !IsStunned())
        {
            CastAbility(abilityHandler);
            StartCoroutine(Attack());
            abilityHandler.StartCooldown();
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
        Ability _ability = Instantiate(ability.GetAbility(), GetCastPos(0).position, transform.rotation);
        _ability.SetOwner(this);

        Vector3 toCastPos = target.gameObject.transform.position;

        if (_ability.Info().AbilityType == AbilityType.PROJECTILE)
        {
            toCastPos = target.gameObject.transform.position;
            toCastPos.y += (target.GetComponent<CapsuleCollider>().height * 0.75f);
        }

        _ability.StartCast(this, toCastPos);
    }

    // --- AI Utility Methods ---

    protected void MoveTowardsPlayer()
    {
        if (target != null)
        {
            agent.speed = GetSpeed();
            agent.SetDestination(target.gameObject.transform.position);
            animator.SetBool("isMoving", !IsStunned() && !IsRooted());
        }
    }

    protected bool IsPlayerInRange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);
        return distanceToPlayer <= aggroRange;
    }

    protected bool IsPlayerVisible()
    {
        Vector3 directionToPlayer = (GameManager.instance.player.transform.position - headPos.position).normalized;
        float angleToPlayer = Vector3.Angle(directionToPlayer, transform.forward);
        if (angleToPlayer < viewAngle)
        {
            int layerMask = LayerMask.GetMask(new string[] { "Player" });

            if (Physics.Raycast(headPos.position, directionToPlayer, out RaycastHit hit, aggroRange))
            {
                if (hit.collider.gameObject.TryGetComponent<Unit>(out Unit unit))
                {
                    target = unit;

                    return true;
                }
            }
        }
        target = null;
        return false;
    }

    protected void FaceTarget()
    {
        playerDir = (GameManager.instance.player.transform.position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    protected virtual IEnumerator Attack()
    {
        canCastAbility = false;
         
        yield return new WaitForSeconds(abilityRate * Time.deltaTime); 

        canCastAbility = true;
    }

    // --- Combat and Health ---

    public override void OnDeath(Unit other = null, Ability source = null, Damage damage = default)
    {
        isDead = true;
        //animator.SetTrigger("Die");
        TryDropPickup();
        base.OnDeath(other);
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

    //protected void SearchingState()
    //{
    //    // Move to the last attack position
    //    agent.SetDestination(lastAttackPosition);

    //    // TODO: Implement patrolling logic around last known position
    //    // You could implement a small loop with random positions around `lastAttackPosition`

    //    // Check visibility of the player
    //    if (IsPlayerVisible())
    //    {
    //        currentState = AIState.Chasing; // Transition to chasing state if player is visible
    //    }
    //    else
    //    {
    //        // Optional: Return to idle after some time if the player is not found
    //        StartCoroutine(IdleAfterSearching()); // Start idle timer
    //    }
    //}

    //protected void RoamingState()
    //{
    //    if (IsPlayerInRange() && IsPlayerVisible())
    //    {
    //        StopRoaming();
    //        currentState = AIState.Chasing;
    //    }
    //}
    //protected void StartRoaming()
    //{
    //    if (roamCoroutine == null)
    //    {
    //        roamCoroutine = StartCoroutine(Roam());
    //    }
    //}

    //protected void StopRoaming()
    //{
    //    if (roamCoroutine != null)
    //    {
    //        StopCoroutine(roamCoroutine);
    //        roamCoroutine = null;
    //    }
    //}

    //protected IEnumerator Roam()
    //{
    //    isRoaming = true;
    //    yield return new WaitForSeconds(roamTimer);

    //    Vector3 randomPos = Random.insideUnitSphere * roamDist + startPos;
    //    NavMeshHit hit;
    //    NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
    //    agent.SetDestination(hit.position);
    //    currentState = AIState.Roaming;
    //    isRoaming = false;
    //}
    //private IEnumerator IdleAfterSearching()
    //{
    //    yield return new WaitForSeconds(2f); // Wait before transitioning
    //    currentState = AIState.Idle; // Transition back to idle
    //}

}
