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

    Color colorOrig;

    bool playerInRange;
    bool isAttacking;
    bool isDead;
    public bool isMoving;
    public bool canAttack = true;

    Vector3 playerDir;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
       
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
