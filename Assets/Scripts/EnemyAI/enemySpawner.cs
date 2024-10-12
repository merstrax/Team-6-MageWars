using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawner : MonoBehaviour
{
    [SerializeField] float spawnRadius = 15f; // adjust the radius as needed
    [SerializeField] Transform spawnCenter; // rock/tree/pillar : center of the spawn area

    bool playerInRange;

    public void SpawnEnemy(GameObject enemy)
    {
        // Calculate a random position within the spawn area
        Vector3 spawnPosition = GetRandomSpawnPosition();

        Instantiate(enemy, spawnPosition, spawnCenter.rotation);
    }

    Vector3 GetRandomSpawnPosition()
    {
        // Generate a random angle (0 to 360 degrees)
        float angle = Random.Range(0f, 360f);

        // Calculate the x and z coordinates based on the angle and radius
        float x = Mathf.Sin(angle * Mathf.Deg2Rad) * spawnRadius;
        float z = Mathf.Cos(angle * Mathf.Deg2Rad) * spawnRadius;

        // If a spawn center is specified, offset the position by its coordinates
        if (spawnCenter != null)
        {
            // Offset the position by the spawn center's coordinates
            x += spawnCenter.position.x;
            z += spawnCenter.position.z;
        }

        // is y coord 0? 
        return new Vector3(x, 0f, z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public bool InRange()
    {
        return playerInRange;
    }
}
