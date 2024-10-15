using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormBringing : Ability
{
    private const float channelDuration = 3f; // Duration of the channeling (2-3 seconds)
    private const float vortexDamage = 10f; // Damage dealt by the vortex each second
    private const float vortexRadius = 5f; // Radius of the vortex effect
    private const float buffDuration = 5f; // Duration of the movement speed and jump buff
    private const float speedIncrease = 1.5f; // Movement speed multiplier
    private const float jumpIncrease = 1.5f; // Jump height multiplier
    private const float abilityCooldown = 7f; /// Cooldown for the ability USE UNIT

    private bool isChanneling = false; // Tracks if the player is currently channeling
    private float channelTimer = 0f; // Timer for channeling duration
    private float abilityTimer = 0f; // Timer for the ability cooldown


    protected override void Update()
    {
        base.Update();

        // Update cooldown timer
        if (abilityTimer > 0)
        {
            abilityTimer -= Time.deltaTime;
        }

        // Handle channeling logic
        if (isChanneling)
        {
            // Continue channeling while the timer is active
            if (channelTimer > 0)
            {
                channelTimer -= Time.deltaTime;

                // Apply vortex damage to nearby enemies
                ApplyVortexDamage();

                // End channeling if the timer reaches 0
                if (channelTimer <= 0)
                {
                    EndChanneling();
                }
            }
        }
    }

    public override void CastStart(Unit owner, Vector3 lookAt)
    {
        // Ensure the ability is off cooldown
        if (abilityTimer <= 0)
        {
            base.CastStart(owner, lookAt);
            StartChanneling();
        }
        else
        {
            Debug.Log("Ability is on cooldown.");
        }
    }

    // Start channeling and root the player
    private void StartChanneling()
    {
        if (owner != null)
        {
            isChanneling = true;
            channelTimer = channelDuration;

            /// Root the player while channeling
            
            Debug.Log("StormBringing started!");
        }
    }

    // End channeling, apply the buff, and reset cooldown
    private void EndChanneling()
    {
        isChanneling = false;

        /// Root the player while channeling

        // Grant movement speed and jump buff
        ApplyBuff();

        // Reset ability cooldown
        abilityTimer = abilityCooldown;
        Debug.Log("StormBringing completed! Buff applied.");
    }

    // Deal vortex damage to nearby enemies
    private void ApplyVortexDamage()
    {
        // Find enemies within the vortex radius
        Collider[] hitEnemies = Physics.OverlapSphere(owner.transform.position, vortexRadius);

        // Apply damage to each enemy in range
        foreach (Collider enemy in hitEnemies)
        {
            Unit enemyUnit = enemy.GetComponent<Unit>();

            // Exclude the player from damage
            if (enemyUnit != null && enemyUnit != owner)
            {
                // Apply damage each second
                enemyUnit.TakeDamage(vortexDamage * Time.deltaTime, this);
                Debug.Log("Dealt vortex damage to " + enemyUnit.name);
            }
        }
    }

    // Apply movement speed and jump height buff
    private void ApplyBuff()
    {
        /// Increase movement speed for the buff duration
        

        /// Increase jump height for the buff duration
       
    }

    public override void CastInterrupt()
    {
        base.CastInterrupt();

        // Stop channeling and unroot the player if interrupted
        isChanneling = false;

        /// Root the player while channeling
        

        Debug.Log("StormBringing was interrupted.");
    }
}
