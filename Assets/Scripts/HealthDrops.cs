using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDrops : MonoBehaviour
{
    // time before drop disappears 
    [SerializeField] float lifeSpan = 15f;

    private void Start()
    {
        //Start Coroutine to handle automatic destruction 
        StartCoroutine(DestroyAfterTime());

    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that collided is the player
        Unit playerUnit = other.GetComponent<Unit>();  // This will work since PlayerController inherits from Unit

        if (playerUnit != null && other.CompareTag("Player"))
        {
            // Restore player's health to maximum using the Unit methods
            playerUnit.SetHealthCurrent(playerUnit.GetHealthMax());

            // Destroy the health drop once picked up
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifeSpan);

        // Destroys Drop after after lifespan
        Destroy(gameObject);
    }
}
