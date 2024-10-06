using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BearsAI : enemyAI
{
    // Bear Roar
    [Range(0, 5.0f)][SerializeField] float roarCooldown;
    [Range(0, 3.0f)][SerializeField] float stunDuration;
    [Range(0, 15.0f)][SerializeField] private float nextRoarTime;
    [Range(0, 10.0f)][SerializeField] float roarRadius;


    // Bear Melee
    [Range(0, 3.0f)][SerializeField] float meleeRate;

    // Bear Movement
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

        if (IsPlayerInRange() && IsPlayerVisible())
        {
            MoveTowardsPlayer();
        }
    }

    private void TryRoar()
    {
        if (Time.time >= nextRoarTime)
        {
            // Optionally, you can add a condition to check if the player is within a certain range for the roar
            if (Vector3.Distance(transform.position, GameManager.instance.player.transform.position) <= roarRadius) 
            {
                StartCoroutine(Roar());
                nextRoarTime = Time.time + roarCooldown; // Reset roar cooldown
            }
        }
    }

    private IEnumerator Roar()
    {
        // Trigger roar animation
        animator.SetTrigger("Roar");

        // Wait for the roar animation to finish (adjust the duration as needed)
        yield return new WaitForSeconds(1f);

        // Apply stun/root to the player
        IsStunned(); 
        IsRooted(); 
    }

    // Override the Attack coroutine
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
        GameManager.instance.player.GetComponent<Unit>().TakeDamage(10, this as Unit);

        // Wait for melee cooldown
        yield return new WaitForSeconds(meleeRate);
    }

    private void LeftMeleeAttackHit()
    {
        // Deal damage to the player
        GameManager.instance.player.GetComponent<Unit>().TakeDamage(10, this as Unit);
    }

    private void RightMeleeAttackHit()
    {
        // Deal damage to the player
        GameManager.instance.player.GetComponent<Unit>().TakeDamage(10, this as Unit);
    }
}
