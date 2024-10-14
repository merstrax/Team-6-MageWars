using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public enum DropType { maxHealth, decreasedCooldown, increasedMovSpeed, increasedDMG }
public class HealthDrops : MonoBehaviour
{
    // time before drop disappears 
    [SerializeField] float lifeSpan;
    [SerializeField] DropType dropType;
    [SerializeField] AbilityStats AbilityDrops;

    private Coroutine destroyTimer;

    private void Update()
    {
        if (destroyTimer == null)
        {
            destroyTimer = StartCoroutine(DestroyAfterTime());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that collided is the player
        Unit playerUnit = other.GetComponent<Unit>();  // This will work since PlayerController inherits from Unit

        if (playerUnit != null && other.CompareTag("Player"))
        {
            // apply the effect of the drop
            ApplyDropEffect(playerUnit);

            // Destroy the drop once picked up
            StopCoroutine(DestroyAfterTime()); 
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyAfterTime()
    {

        yield return new WaitForSeconds(lifeSpan);

        destroyTimer = null; 

        if (gameObject != null)
        {
            // Destroys Drop after after lifespan
            Destroy(gameObject);
        }
    }

    private void ApplyDropEffect(Unit playerUnit)
    {
        Ability drops = gameObject.AddComponent<Ability>();
        drops.DoEffectApply(playerUnit, AbilityDrops);

        //drops.DoEffectApply(playerUnit, AbilityDrops.EffectAbility); 

        /// Implemented Soon!!
        //drops.OnHit(playerUnit);   

        Destroy(drops); 
    }
}
