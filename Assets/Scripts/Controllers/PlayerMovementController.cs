using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Player Controller")]
    [SerializeField] PlayerController player;
    [SerializeField] PlayerInputController inputController;
    [SerializeField] CharacterController characterController;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] Animator animator;

    //Player Movement
    [Header("Player Movement")]
    [Range(0, 10)][SerializeField] float speed;
    [Range(0, 3)][SerializeField] float sprintMod;
    [Range(0, 3)][SerializeField] int jumpMax;
    [Range(0, 15)][SerializeField] int jumpSpeed;
    [Range(0, 50)][SerializeField] int gravity;

    float originalSpeed;

    Vector3 playerVel;
    int jumpCount;
    bool isSprinting;
    public bool IsSprinting() { return isSprinting; }

    private void Start()
    {
        originalSpeed = speed;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateMovement();
        UpdateSprint();
    }

    void UpdateMovement()
    {
        //Reset jump variables
        if (characterController.isGrounded)
        {
            playerVel = Vector3.zero;
            jumpCount = 0;
            animator.SetBool("Jumping", false);
        }

        //Transform and move based on local space
        characterController.Move(speed * Time.deltaTime * player.GetMoveDir());

        float agentSpeed = inputController.move.x;
        float animSpeed = animator.GetFloat("MoveX");
        animator.SetFloat("MoveX", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * 5.0f));

        agentSpeed = inputController.move.y;
        animSpeed = animator.GetFloat("MoveZ");
        animator.SetFloat("MoveZ", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * 5.0f));


        characterController.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;
    }

    public void DoJump()
    {
        //Jump logic
        if (jumpCount < jumpMax)
        {
            animator.Play("JumpStart");
            animator.SetBool("Jumping", true);
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
    }

    void UpdateSprint()
    {
        if (inputController.sprint)
        {
            speed = originalSpeed * sprintMod;
            isSprinting = true;
        }
        else
        {
            speed = originalSpeed;
            isSprinting = false;
        }
    }
}