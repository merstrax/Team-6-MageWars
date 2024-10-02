using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    //Player logic variables
    [Header("Player Controller")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] Animator animator;

    [Header("Player Stats")]
    [Range(1, 20)][SerializeField] int healthMax;
    [Range(0, 200)][SerializeField] float regenDelay;
    [SerializeField] float shootRate;

    //Player Movement
    [Header("Player Movement")]
    [Range(0, 10)][SerializeField] float speed;
    [Range(0, 3)][SerializeField] float sprintMod;
    [Range(0, 3)][SerializeField] int jumpMax;
    [Range(0, 15)][SerializeField] int jumpSpeed;
    [Range(0, 50)][SerializeField] int gravity;

    //Player Shoot
    [Header("Player Weapon")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;

    [Header("Player Audio")]
    [SerializeField] AudioSource audioPlayer;
    [SerializeField] AudioClip[] audioDamage;
    [Range(0, 1)][SerializeField] float audioDamageVolume;
    [SerializeField] AudioClip[] audioWalk;
    [Range(0, 1)][SerializeField] float audioWalkVolume;
    [SerializeField] AudioClip[] audioJump;
    [Range(0, 1)][SerializeField] float audioJumpVolume;

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;

    bool isSprinting;
    bool isShooting;
    public bool IsSprinting() { return isSprinting; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 400, Color.red);

        if (Input.GetButtonDown("Fire1"))
        {
            shoot();
        }

        UpdateMovement();
        UpdateSprint();
    }

    void UpdateMovement()
    {
        //Reset jump variables
        if (controller.isGrounded)
        {
            playerVel = Vector3.zero;
            jumpCount = 0;
            animator.SetBool("Jumping", false);
        }

        //Transform and move based on local space
        moveDir = Input.GetAxis("Horizontal") * transform.right +
                    Input.GetAxis("Vertical") * transform.forward;

        controller.Move(speed * Time.deltaTime * moveDir);

        float agentSpeed = Input.GetAxis("Horizontal");
        float animSpeed = animator.GetFloat("MoveX");
        animator.SetFloat("MoveX", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * 5.0f));

        agentSpeed = Input.GetAxis("Vertical");
        animSpeed = animator.GetFloat("MoveZ");
        animator.SetFloat("MoveZ", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * 5.0f));

        //Jump logic
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            animator.Play("JumpStart");
            animator.SetBool("Jumping", true);
            jumpCount++;
            playerVel.y = jumpSpeed;
        }

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;
    }
    

    void UpdateSprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    public void CreateBullet()
    {
        if (bullet != null)
            Instantiate(bullet, shootPos.position, transform.rotation);
    }

    IEnumerator shoot()
    {
        isShooting = true;
        CreateBullet();
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
}
