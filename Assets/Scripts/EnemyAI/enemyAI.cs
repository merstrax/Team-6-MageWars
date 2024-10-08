using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class enemyAI : Unit
{
    // Define AI States
    protected enum AIState { Idle, Roaming, Chasing, Attacking, Dead, Searching }
    protected AIState currentState = AIState.Idle;

    [Header("Render Components")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected Renderer model;
    [SerializeField] protected ParticleSystem particle;
    [SerializeField] protected Image healthBar;
    [SerializeField] protected CapsuleCollider bodyCollider;
    [SerializeField] protected CapsuleCollider headCollider;

    [Header("AI Nav")]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Transform headPos;
    [SerializeField] protected int faceTargetSpeed;

    [Header("Enemy Combat")]
    [SerializeField] protected GameObject rightHandPos;
    [SerializeField] protected GameObject leftHandPos;
    [SerializeField] protected GameObject bolt;
    [Range(0, 3f)] [SerializeField] protected float attackRate;
    [Range(0, 3f)] [SerializeField] protected float meleeAttackRange;
    [Range(6, 15f)] [SerializeField] protected float rangedAttackRange;

    [Header("Aggro and Roaming")]
    [SerializeField] protected float aggroRange;
    [SerializeField] protected float roamRange;
    [SerializeField] protected float renderDistance;
    [SerializeField] protected int viewAngle;
    [SerializeField] protected int roamDist;
    [SerializeField] protected int roamTimer;

    [Header("Health Drop")]
    [SerializeField] protected GameObject healthDropPrefab;
    [Range(0f, 1f)] [SerializeField] protected float dropChance;

    // Variables 
    protected Vector3 playerDir;
    protected Vector3 startPos;
    protected Vector3 lastAttackPosition;
    protected bool isAttacking;
    protected bool isDead;
    protected bool isRoaming;
    protected bool canAttack = true;
    protected Coroutine roamCoroutine;

    protected override void Start()
    {
        base.Start();
        startPos = transform.position;
        currentState = AIState.Idle;
    }

    protected virtual void Update()
    {
        if (isDead) return;

        switch (currentState)
        {
            case AIState.Idle:
                IdleState();
                break;
            case AIState.Roaming:
                RoamingState();
                break;
            case AIState.Chasing:
                ChasingState();
                break;
            case AIState.Attacking:
                AttackingState();
                break;
            case AIState.Searching:
                SearchingState();
                break;
        }
    }

    // --- State Methods ---

    protected void IdleState()
    {
        if (IsPlayerInRange() && IsPlayerVisible())
        {
            currentState = AIState.Chasing;
        }
        else if (!isRoaming)
        {
            StartRoaming();
        }
    }

    protected void SearchingState()
    {
        // Move to the last attack position
        agent.SetDestination(lastAttackPosition);

        // TODO: Implement patrolling logic around last known position
        // You could implement a small loop with random positions around `lastAttackPosition`

        // Check visibility of the player
        if (IsPlayerVisible())
        {
            currentState = AIState.Chasing; // Transition to chasing state if player is visible
        }
        else
        {
            // Optional: Return to idle after some time if the player is not found
            StartCoroutine(IdleAfterSearching()); // Start idle timer
        }
    }

    protected void RoamingState()
    {
        if (IsPlayerInRange() && IsPlayerVisible())
        {
            StopRoaming();
            currentState = AIState.Chasing;
        }
    }

    protected void ChasingState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

        if (distanceToPlayer <= meleeAttackRange)
        {
            currentState = AIState.Attacking;
            StartCoroutine(Attack("melee"));
        }
        else if (distanceToPlayer <= rangedAttackRange)
        {
            currentState = AIState.Attacking;
            StartCoroutine(Attack("ranged"));
        }
        else if (!IsPlayerInRange() || !IsPlayerVisible())
        {
            currentState = AIState.Idle;
        }
        else
        {
            MoveTowardsPlayer();
        }
    }

    protected void AttackingState()
    {
        // Handle attacking through coroutines
        if (!isAttacking)
        {
            currentState = AIState.Chasing;
        }
    }

    // --- AI Utility Methods ---

    protected void StartRoaming()
    {
        if (roamCoroutine == null)
        {
            roamCoroutine = StartCoroutine(Roam());
        }
    }

    protected void StopRoaming()
    {
        if (roamCoroutine != null)
        {
            StopCoroutine(roamCoroutine);
            roamCoroutine = null;
        }
    }

    protected IEnumerator Roam()
    {
        isRoaming = true;
        yield return new WaitForSeconds(roamTimer);

        Vector3 randomPos = Random.insideUnitSphere * roamDist + startPos;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
        currentState = AIState.Roaming;
        isRoaming = false;
    }

    protected void MoveTowardsPlayer()
    {
        agent.SetDestination(GameManager.instance.player.transform.position);
        animator.SetBool("isMoving", true);
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

        if (angleToPlayer > viewAngle) return false;

        RaycastHit hit;
        int layerMask = LayerMask.GetMask("Player");

        if (Physics.Raycast(headPos.position, directionToPlayer, out hit, aggroRange, layerMask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    protected void FaceTarget()
    {
        playerDir = (GameManager.instance.player.transform.position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public virtual IEnumerator Attack(string attackType)
    {
        isAttacking = true;
        canAttack = false;

        if (attackType == "melee")
        {
            animator.SetTrigger("MeleeAttack");
            yield return new WaitForSeconds(attackRate);
            // TODO: Damage logic for melee attack
        }
        else if (attackType == "ranged")
        {
            animator.SetTrigger("RangedAttack");
            yield return new WaitForSeconds(attackRate);
            Instantiate(bolt, rightHandPos.transform.position, Quaternion.identity);
        }

        isAttacking = false;
        canAttack = true;
        currentState = AIState.Chasing;
    }

    // --- Combat and Health ---

    public override void OnDeath(Unit other = null)
    {
        isDead = true;
        agent.SetDestination(transform.position);
        animator.SetTrigger("Die");
        TryDropHealthPickup();
        base.OnDeath(other);
    }

    protected void TryDropHealthPickup()
    {
        // Health drop logic - chance to drop health item
        float randomValue = Random.Range(0f, 1f);
        if (randomValue <= dropChance && healthDropPrefab != null)
        {
            Instantiate(healthDropPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // If the player is out of aggro range, go into searching state
            if (!IsPlayerInRange())
            {
                lastAttackPosition = GameManager.instance.player.transform.position; // Store the player's position
                currentState = AIState.Searching; // Switch to Searching state
            }
            else
            {
                // Handle combat initiation or player interaction
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Handle player exiting range
        }
    }

    // Coroutine to wait and then return to idle state after searching
    private IEnumerator IdleAfterSearching()
    {
        yield return new WaitForSeconds(2f); // Wait before transitioning
        currentState = AIState.Idle; // Transition back to idle
    }
}
