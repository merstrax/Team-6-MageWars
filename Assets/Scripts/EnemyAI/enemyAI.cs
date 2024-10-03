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
    [SerializeField] int viewAngle; 

    Vector3 playerDir;

    Color colorOrig;

    bool playerInRange;
    bool isAttacking;
    bool isDead;
    public bool isMoving;
    public bool canAttack = true;

    float angleToPLayer;
    float stopDistOrig;

    Coroutine someCo;  

    // Start is called before the first frame update
    void Start()
    {
      
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

    private bool IsPlayerInRange()
    {
        // Raycast to check if the player is within aggro range
        playerDir = gameManager.instance.player.transform.position - headPos.position;
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
