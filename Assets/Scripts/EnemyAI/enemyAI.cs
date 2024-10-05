using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : Unit
{
    [Header("Render Components")]
    [SerializeField] Animator animator;
    [SerializeField] Renderer model;
    [SerializeField] ParticleSystem particle;
    //[SerializeField] TextMeshPro healthText; 
    [SerializeField] CapsuleCollider bodyCollider;
    [SerializeField] CapsuleCollider headCollider;

    [Header("AI Nav")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [Range(0, 30)][SerializeField] int faceTargetSpeed;

    [Header("Enemy Combat")]
    [SerializeField] GameObject rightHandPos;
    [SerializeField] GameObject leftHandPos;
    [SerializeField] GameObject bolt; 
    [Range(0, 3)][SerializeField] float attackRate;

    [Header("Aggro and Roaming")]
    [Range(0, 25)][SerializeField] float aggroRange;
    [Range(0, 25)][SerializeField] float roamRange;
    [Range(50, 150)][SerializeField] float renderDistance;
    [Range(10, 50)][SerializeField] int viewAngle;
    [Range(0, 20)][SerializeField] int roamDist;
    [Range(0, 5.0f)][SerializeField] int roamTimer;

    // Variables 
    Vector3 playerDir;
    Vector3 startPos;

    Color colorOrig;

    bool playerInRange;
    bool isAttacking;
    bool isDead;
    bool isRoaming; 
    public bool isMoving;
    public bool canAttack = true;
    float angleToPLayer;
    float stopDistOrig;

    Coroutine someCo;  

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color; 
        stopDistOrig = agent.stoppingDistance;
        startPos = transform.position; 
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player is in range
        if (IsPlayerInRange())
        {
            // Move towards the player
            MoveTowardsPlayer(); 

            // Face the player
            FaceTarget();

            // Shoot at the player
            if (!isAttacking)
            {
                //StartCoroutine(shoot());
            }
        }
        else
        {
            // Stop moving and shooting
            agent.Move(Vector3.zero);
            StopAllCoroutines();
        }
    }

    // Move towards the player
    private void MoveTowardsPlayer()
    {
        // Set the destination to the player's position
        agent.SetDestination(GameManager.instance.player.transform.position);
    } 

    // Check if the player is in range
    public bool IsPlayerInRange()
    {
        // Calculate the distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

        // Check if the player is within range
        return distanceToPlayer <= aggroRange;
    }

    // Check if the player is visible
    public bool IsPlayerVisible()
    {
        // Calculate the direction to the player
        Vector3 directionToPlayer = (GameManager.instance.player.transform.position - headPos.position).normalized;

        // Calculate the angle to the player
        float angleToPlayer = Vector3.Angle(directionToPlayer, transform.forward);
        // Check if the player is within the view angle
        if (angleToPlayer > viewAngle)
            return false;

        // Perform a raycast to check if there's a clear line of sight to the player
        RaycastHit hit;
        int layerMask = LayerMask.GetMask("Player");

        if (Physics.Raycast(headPos.position, directionToPlayer, out hit, aggroRange, layerMask))
        {
            // Check if the hit object is the player and there's a clear line of sight
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        // If the player isn't hit or isn't visible, return false
        return false;
    }

    // Face the target
    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    // Roam around the environment
    IEnumerator roam()
    {
        isRoaming = true;
        yield return new WaitForSeconds(roamTimer);

        agent.stoppingDistance = 0;
        Vector3 randomPos = Random.insideUnitSphere * roamDist;
        randomPos += startPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);

        isRoaming = false;
        someCo = null;
    }

    public virtual IEnumerator Attack()
    {
        // Basic implementation of attack behavior
        yield return null; 
    } 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
        else
        {
            // Handle other colliders
            Debug.Log("Other collider entered trigger: " + other.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
        else
        {
            // Handle other colliders
            Debug.Log("Other collider exited trigger: " + other.name);
        }
    }

}
