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
        agent.speed = movementSpeed;
    }

    // Override the Attack coroutine
    public override IEnumerator Attack(string attackType)
    {
        // Check if the player is in range and visible
        if (IsPlayerInRange() && IsPlayerVisible())
        {
            // Perform melee attack
            MeleeAttack();

            yield return new WaitForSeconds(meleeRate);
        }
        else
        {
            yield return null;
        }
    }

    private void MeleeAttack()
    {
        // Play melee attack animation
        animator.Play("MeleeAttack");

        // Deal damage to the player
        GameManager.instance.player.GetComponent<Unit>().TakeDamage(7, other: this as Unit); 
    }

    public void RightHandEnable()
    {

    }
    public void RightHandDisable()
    {

    }
}
