using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] Transform orientation;
    [SerializeField] Transform model;
    [SerializeField] Rigidbody rigidBody;
    [SerializeField] Animator animator;

    [Header("Movement")]
    [SerializeField] float groundDrag;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;

    [Header("Jumping")]
    [SerializeField] float jumpSpeed;

    [Header("Ground Check")]
    [SerializeField] float playerHeight;
    [SerializeField] LayerMask groundMask;
    public bool isGrounded;

    private Vector3 moveDirection;
    private float moveSpeed;
    private float horizontalMovement;
    private float verticalMovement;

    // Update is called once per frame
    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);

        UpdateInput();
        UpdateAnimations();

        if (isGrounded)
            rigidBody.drag = groundDrag;
        else
            rigidBody.drag = 0;
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void UpdateAnimations()
    {
        float agentSpeed = InputController.instance.Move.x;
        float animSpeed = animator.GetFloat("MoveX");
        animator.SetFloat("MoveX", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * 5.0f));

        agentSpeed = InputController.instance.Move.y;
        animSpeed = animator.GetFloat("MoveZ");
        animator.SetFloat("MoveZ", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * 5.0f));

        animSpeed = animator.GetFloat("Speed");
        animator.SetFloat("Speed", Mathf.Lerp(animSpeed, moveSpeed, Time.deltaTime * 2.0f));

        animator.SetBool("IsGrounded", isGrounded);
    }

    private void UpdateInput()
    {
        horizontalMovement = InputController.instance.Move.x;
        verticalMovement = InputController.instance.Move.y;
    }

    private void UpdateMovement()
    {
        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;

        if (verticalMovement == 0 && horizontalMovement == 0)
        {
            moveSpeed = 0;
        }
        else
        {
            moveSpeed = 1;
        }

        if (InputController.instance.Sprint)
        {
            rigidBody.AddForce(10f * runSpeed * moveDirection, ForceMode.Force);
            moveSpeed *= 2;
        }
        else
        {
            rigidBody.AddForce(10f * walkSpeed * moveDirection, ForceMode.Force);
        }

        if(InputController.instance.Jump && isGrounded)
        {
            animator.SetTrigger("Jump");
            rigidBody.AddForce(jumpSpeed * Vector3.up, ForceMode.Impulse);
            InputController.instance.Jump = false;
        }

        SpeedControl();
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new(rigidBody.velocity.x, 0f, rigidBody.velocity.z);

        if (InputController.instance.Sprint)
        {
            if (flatVel.magnitude > runSpeed)
            {
                Vector3 limitedVal = flatVel.normalized * runSpeed;
                rigidBody.velocity = new Vector3(limitedVal.x, rigidBody.velocity.y, limitedVal.z);
            }
        }
        else
        {
            if (flatVel.magnitude > walkSpeed)
            {
                Vector3 limitedVal = flatVel.normalized * walkSpeed;
                rigidBody.velocity = new Vector3(limitedVal.x, rigidBody.velocity.y, limitedVal.z);
            }
        }
    }
}
