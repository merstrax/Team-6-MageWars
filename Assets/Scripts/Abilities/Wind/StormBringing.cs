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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
