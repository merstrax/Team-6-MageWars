using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Player Controller")]
    [SerializeField] PlayerController player;
    [SerializeField] InputController inputController;
    [SerializeField] CharacterController characterController;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rigidBody;
 
    //Player Movement
    [Header("Player Movement")]
    [Range(0, 10)][SerializeField] float speed;
    [Range(0, 3)][SerializeField] float sprintMod;
    [Range(0, 3)][SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;


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

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        //Reset jump variables
        if (characterController.isGrounded)
        {
            animator.SetTrigger("JumpEnd");
            if (inputController.Jump)
                DoJump();
        }

        //Transform and move based on local space
        Vector3 _moveVector = player.GetMoveDir() * player.GetSpeed();
        rigidBody.AddForce(_moveVector.normalized * 10f, ForceMode.Force);

        float agentSpeed = inputController.Move.x;
        float animSpeed = animator.GetFloat("MoveX");
        animator.SetFloat("MoveX", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * 5.0f));

        agentSpeed = inputController.Move.y;
        animSpeed = animator.GetFloat("MoveZ");
        animator.SetFloat("MoveZ", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * 5.0f));

        agentSpeed = speed / player.GetSpeed();
        animSpeed = animator.GetFloat("Speed");
        animator.SetFloat("Speed", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * 5.0f));

        //rigidBody.Move(playerVel * Time.deltaTime, Quaternion.identity);
        //playerVel.y -= gravity * Time.deltaTime;
    }

    public void DoJump()
    {
        animator.SetTrigger("Jump");
        rigidBody.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
    }

    void UpdateSprint()
    {
        if (inputController.Sprint)
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
            rigidBody.AddForce(_direction, ForceMode.Impulse);
            //rigidBody.velocity = _direction * Time.deltaTime;
            yield return null;
        }
    }

    public void TeleportMovement(Vector3 location){ }
}
