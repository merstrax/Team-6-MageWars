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
            agent.Move(playerDir);

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

    private bool IsPlayerInRange()
    {
        // Raycast to check if the player is within aggro range
        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPLayer = Vector3.Angle(playerDir, transform.forward); 

        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPLayer <= viewAngle)   
            {
                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed); 
    }
}
