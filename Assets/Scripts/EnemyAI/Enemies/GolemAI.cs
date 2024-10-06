using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

public class GolemAI : enemyAI
{
    // Golem Ranged 
    [Range(0, 30.0f)][SerializeField] float rockVelocity;
    [Range(0, 45)][SerializeField] int rockRange;
    [Range(0, 30)][SerializeField] int minimumThrowDist;
    [Range(0, 4.0f)][SerializeField] float rangedCooldown;

    // Golem Melee
    [Range(0, 2.0f)][SerializeField] float meleeRate;

    // Golem Movement
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

        if (IsPlayerInRange() && IsPlayerVisible())
        {
            MoveTowardsPlayer();
        }
    }

    // override the Attack coroutine
    public override IEnumerator Attack()
    {
        // Check if the player is in range and visible
        if (IsPlayerInRange() && IsPlayerVisible())
        {
            // Perform melee attack
            yield return MeleeAttack();   
        }
        else
        {
            yield return null;
        }
    }

    
    private IEnumerator MeleeAttack()
    {
        // Play left melee attack animation
        animator.SetTrigger("LeftMeleeAttack");

        // Wait for the animation to finish
        yield return new WaitForSeconds(meleeRate);

        // Play right melee attack animation
        animator.SetTrigger("RightMeleeAttack");

        // Wait for the animation to finish
        yield return new WaitForSeconds(meleeRate);

        // Deal damage to the player
        GameManager.instance.player.GetComponent<Unit>().TakeDamage(15, this as Unit); 

        // Wait for melee cooldown
        yield return new WaitForSeconds(meleeRate);
    }

    private void LeftMeleeAttackHit()
    {
        // Deal damage to the player
        GameManager.instance.player.GetComponent<Unit>().TakeDamage(15, this as Unit);
    }

    private void RightMeleeAttackHit()
    {
        // Deal damage to the player
        GameManager.instance.player.GetComponent<Unit>().TakeDamage(15, this as Unit);
    }
}