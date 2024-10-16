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


    Vector3 playerVel;
    int jumpCount;
    bool isSprinting;
    public bool IsSprinting() { return isSprinting; }

    bool isJumping;

    // Update is called once per frame
    private void Update()
    {
        if(player.IsDead) return;

        UpdateSprint();
        UpdateMovement();
    }

    void UpdateMovement()
    {
        //Reset jump variables
        if (characterController.isGrounded)
        {
            animator.SetTrigger("JumpEnd");
            if (inputController.jump)
                DoJump();
        }

        //Transform and move based on local space
        characterController.Move(speed * Time.deltaTime * player.GetMoveDir());

        float agentSpeed = inputController.move.x;
        float animSpeed = animator.GetFloat("MoveX");
        animator.SetFloat("MoveX", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * 5.0f));

        agentSpeed = inputController.move.y;
        animSpeed = animator.GetFloat("MoveZ");
        animator.SetFloat("MoveZ", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * 5.0f));

        agentSpeed = speed / player.GetSpeed();
        animSpeed = animator.GetFloat("Speed");
        animator.SetFloat("Speed", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * 5.0f));

        characterController.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;
    }

    public void DoJump()
    {
        animator.SetTrigger("Jump");
        playerVel.y = jumpSpeed;
    }

    void UpdateSprint()
    {
        if (inputController.sprint)
        {
            speed = player.GetSpeed() * sprintMod;
            isSprinting = true;
        }
        else
        {
            speed = player.GetSpeed();
            isSprinting = false;
        }
    }

    public IEnumerator DashMovement(float speed, float duration)
    {
        float startTime = Time.time;
        Vector3 _direction = player.GetMoveDir() * speed;

        while (Time.time < startTime + duration)
        {
            characterController.Move(_direction * Time.deltaTime);
            yield return null;
        }
    }

    public void TeleportMovement(Vector3 location){ }
}
