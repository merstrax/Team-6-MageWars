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
    }

    // Override the Attack coroutine
    public override IEnumerator Attack()
    {
        if (!canAttack || isAttacking)
            yield break; // If it's on cooldown or already attacking, do nothing

        // Perform melee attack
        if (IsPlayerInRange() && IsPlayerVisible())
        {
            isAttacking = true;
            canAttack = false; // Prevent other attacks until cooldown
            yield return MeleeAttack();
            isAttacking = false;
            yield return new WaitForSeconds(meleeRate); // Cooldown period
            canAttack = true; // Reset attack ability after cooldown
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
    }

    private void RightMeleeAttackHit()
    {
        // Deal damage to the player
        GameManager.instance.player.GetComponent<Unit>().TakeDamage(7, this as Unit);
    }
}
