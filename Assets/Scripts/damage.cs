using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damage : MonoBehaviour
{
    [SerializeField] enum damageType { arcane, frost, fire, wind, melee, earth }
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rigidBody;

    [Range(0, 10)][SerializeField] int damageAmount;
    [Range(0, 50)][SerializeField] int speed;
    [Range(0, 5)][SerializeField] int destroyTime;
    [Range(0, 3)][SerializeField] float damageRate;

    bool canDamage = true;

    void Start()
    {
        rigidBody.velocity = transform.forward * speed; // Initialize projectile speed
        Destroy(gameObject, destroyTime); // Destroy after a specified time
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IDamage damageable = other.GetComponent<IDamage>();
            if (damageable != null && canDamage)
            {
                float finalDamage = damageAmount; // Base damage

                switch (type)
                {
                    case damageType.arcane:
                        finalDamage *= 1.5f; // Example of increased damage
                        break;
                    case damageType.frost:
                        // Apply slow effect logic here
                        break;
                    case damageType.fire:
                        // Apply damage over time logic here
                        break;
                    case damageType.wind:
                        // Apply knockback logic here
                        break;
                        // Handle other damage types...
                }

                damageable.TakeDamage(finalDamage); // Apply damage
                canDamage = false; // Prevent further damage
                Destroy(gameObject); // Destroy the projectile after hitting
            }
        }
    }
}