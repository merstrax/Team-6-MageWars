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
    [SerializeField] private GameObject rockPrefab; 
    private float lastRangedAttackTime; 

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
            if (Vector3.Distance(transform.position, GameManager.instance.player.transform.position) > minimumThrowDist)
            {
                // Perform ranged attack
                yield return RangedAttack();
            }
            else
            {
                // Perform melee attack
                yield return MeleeAttack();
            }
        }
        else
        {
            yield return null;
        }
    }

    private IEnumerator RangedAttack()
    {
        // Check cooldown
        if (Time.time - lastRangedAttackTime < rangedCooldown)
        {
            yield break; // Exit if the cooldown is still active
        }

        // Update the last attack time
        lastRangedAttackTime = Time.time;

        // Play ranged attack animation (if you have one)
        animator.SetTrigger("ThrowRock");

        // Create the rock projectile
        GameObject rock = Instantiate(rockPrefab, transform.position, Quaternion.identity);
        Rigidbody rockRb = rock.GetComponent<Rigidbody>();

        // Calculate the direction to the player
        Vector3 direction = (GameManager.instance.player.transform.position - transform.position).normalized;

        // Set the rock's velocity
        rockRb.velocity = direction * rockVelocity;

        // Wait for a short duration to prevent immediate follow-up attacks
        yield return new WaitForSeconds(rangedCooldown);
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