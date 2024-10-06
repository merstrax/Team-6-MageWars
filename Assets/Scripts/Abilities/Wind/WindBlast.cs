using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBlast : Ability 
{
    private int attackCount = 0; // Tracks the number of attacks
    private const int maxAttacks = 3; // Maximum number of attacks
    private const float firstTwoDamage = 5f; // Damage for first two attacks
    private const float thirdAttackDamage = 10f; // Damage for third attack
    private const float knockbackForce = 10f; // Knockback force for third attack
    private const float bleedDamage = 2f; // Damage per tick for bleed
    private const float bleedDuration = 8f; // Duration of bleed effect

    private float abilityTimer = 0f; // Timer for ability cooldown
    private float knockbackTimer = 0f; // Timer for knockback cooldown
    private const float knockbackCooldown = 7f; // Cooldown for knockback
    private const float abilityCooldown = 3f; // Cooldown for the ability
    private float comboTimer = 0f; // Timer to track combo time
    private const float comboResetTime = 2f; // Time limit to cast next attack in combo
    private const float attackIntervalMin = 0.5f; // Min cooldown between combo attacks
    private const float attackIntervalMax = 1f; // Max cooldown between combo attacks
    private float attackCooldown = 0f; // Timer for individual combo attacks

    protected override void Update()
    {
        base.Update();

        // Update knockback cooldown timer
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.deltaTime;
        }

        // Update ability cooldown timer
        if (abilityTimer > 0)
        {
            abilityTimer -= Time.deltaTime;
        }

        // Update combo cooldown timer
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;

            // Reset combo if it exceeds the combo reset time
            if (comboTimer <= 0)
            {
                ResetCombo();
            }
        }

        // Update individual attack cooldown for combos
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

    }

    public override void StartCast(Unit owner, Vector3 lookAt)
    {
        if (abilityTimer <= 0) // Check if the ability is off cooldown
        {
            base.StartCast(owner, lookAt);
            comboTimer = comboResetTime; 
            // Additional logic to handle casting
        }
        else
        {
            Debug.Log("Ability is on cooldown.");
        }
    }

    public override void FinishCast()
    {
        if (attackCooldown <= 0)
        { 
            if (attackCount < maxAttacks)
            {
             // Handle the casting logic for each attack
                if (attackCount < maxAttacks)
                {
                    // First two attacks
                    DoDamage();
                    attackCount++;

                     // Apply bleed effect
                    ApplyBleed();
                }
                else
                {
                    // Third attack
                    DoDamage();

                    // Check if knockback is on cooldown
                    if (knockbackTimer <= 0)
                    {
                        KnockbackEnemy();
                        // Reset knockback cooldown
                        knockbackTimer = knockbackCooldown;
                    }
                    else
                    {
                        // If knockback is on cooldown, apply double bleed
                        ApplyBleed(2); // Pass 2 as the multiplier
                    }

                    // Set cooldown between combo attacks
                    attackCooldown = Random.Range(attackIntervalMin, attackIntervalMax); 
                }
                // Reset attack count after the third attack
                attackCount = 0;

                // Reset ability cooldown
                abilityTimer = abilityCooldown;
            }
        }
    }

    private void ApplyBleed(int multiplier = 1) // Add multiplier parameter
    {
        // Logic to apply bleed effect
        if (other != null)
        {
            StartCoroutine(BleedEffect(other, multiplier)); // Pass multiplier to coroutine
        }
    }

    private IEnumerator BleedEffect(Unit target, int multiplier) // Add multiplier parameter
    {
        for (float time = 0; time < bleedDuration; time += 1f)
        {
            // Apply multiplied bleed damage
            target.TakeDamage(bleedDamage * multiplier);

            // Apply damage every second
            yield return new WaitForSeconds(1f);
        }
    }

    private void KnockbackEnemy()
    {
        // Logic to apply knockback to the target
        if (other != null)
        {
            Vector3 knockbackDirection = (other.transform.position - owner.transform.position).normalized;
            other.GetComponent<Rigidbody>().AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            Debug.Log("Knocked back " + other.name);
        }
    }

    private void ResetCombo()
    {
        // reset attack count, ability, and combo timers 
        attackCount = 0; 
        abilityTimer = abilityCooldown;
        comboTimer = 0; 
    }

    public override void InterruptCast()
    {
        base.InterruptCast();
        // Reset the attack count if interrupted
        ResetCombo(); 
    }
}
