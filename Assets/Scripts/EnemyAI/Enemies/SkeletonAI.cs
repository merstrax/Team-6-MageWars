using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonAI : enemyAI
{
    // Skeleton Melee
    [Range(0, 3.0f)][SerializeField] private float meleeRate;

    // Skeleton Movement
    [Range(0f, 10f)][SerializeField] private float movementSpeed;
    private NavMeshAgent agent;
    private Animator animator;

    protected override void Start()
    {
        // Call the base class
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.speed = movementSpeed;
    }

    protected override void Update()
    {
        // Call the base Update
        base.Update();

        // Move towards the player if in range and visible
        if (IsPlayerInRange() && IsPlayerVisible())
        {
            MoveTowardsPlayer();
            StartCoroutine(Attack());
        }
    }

    // Override the Attack coroutine
    public override IEnumerator Attack()
    {
        // Check if the player is in range and visible
        if (IsPlayerInRange() && IsPlayerVisible())
        {
            yield return MeleeAttack();
        }
        else
        {
            yield return null;
        }
    }

    private IEnumerator MeleeAttack()
    {
        // Play melee attack animation
        animator.SetTrigger("MeleeAttack");

        // Wait for the animation to finish (adjust the duration as needed)
        yield return new WaitForSeconds(meleeRate);

        // Deal damage to the player
        GameManager.instance.player.GetComponent<Unit>().TakeDamage(7, this as Unit);

        // Wait for melee cooldown
        yield return new WaitForSeconds(meleeRate);
    }
    private void RightMeleeAttackHit()
    {
        // Deal damage to the player
        GameManager.instance.player.GetComponent<Unit>().TakeDamage(7, this as Unit);
    }
}
