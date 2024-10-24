using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerMoveController : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] Transform orientation;
    [SerializeField] Transform model;
    [SerializeField] Rigidbody rigidBody;
    [SerializeField] Animator animator;
    [SerializeField] CameraController cameraController;

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
    private float rotateX;
    private bool doRotation;
    private bool doJump;
    private float moveSpeed;
    private float horizontalMovement;
    private float verticalMovement;
    private bool IsDashing;

    // Update is called once per frame
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (SceneManager.GetActiveScene().name == "LoadGameScene") return;

        PlayerSpawnPoint _playerSpawn = FindFirstObjectByType<PlayerSpawnPoint>();
        MoveRigidbody(_playerSpawn.transform.position, Quaternion.identity);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayerSpawnPoint _playerSpawn = FindFirstObjectByType<PlayerSpawnPoint>();
        MoveRigidbody(_playerSpawn.transform.position, Quaternion.identity);
    }

    Vector3 groundRay;
    private void Update()
    {
        groundRay = new Vector3(transform.position.x, transform.position.y + playerHeight, transform.position.z);

        isGrounded = Physics.Raycast(groundRay, Vector3.down, playerHeight + 0.3f, groundMask);

        UpdateInput();
        UpdateAnimations();

        if (isGrounded)
        {
            rigidBody.drag = groundDrag;
        }
        else
            rigidBody.drag = 0;

        walkSpeed = PlayerController.instance.GetSpeed();
        runSpeed = walkSpeed * 2.0f;
    }

    private void FixedUpdate()
    {
        UpdateMovement();

        if (doRotation)
        {
            Transform _cam = cameraController.GetFollowTransform();
            rigidBody.MoveRotation(rigidBody.rotation * Quaternion.AngleAxis(rotateX, Vector3.up));
            rotateX = 0;
            _cam.localEulerAngles = new Vector3(_cam.localEulerAngles.x, 0, 0);
        }
    }

    public void MoveRigidbody(Vector3 position, Quaternion rotation)
    {
        rigidBody.Move(position, rotation);
    }

    public void RotateRigidbody(float rotation, bool doRotate = false)
    {
        rotateX += rotation;
        doRotation = doRotate;
    }

    private void UpdateAnimations()
    {
        if (InputController.instance == null) return;

        float agentSpeed = Mathf.Round(InputController.instance.Move.x);
        float animSpeed = animator.GetFloat("MoveX");
        animator.SetFloat("MoveX", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * 10.0f));

        agentSpeed = Mathf.Round(InputController.instance.Move.y);
        animSpeed = animator.GetFloat("MoveZ");
        animator.SetFloat("MoveZ", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * 10.0f));

        animSpeed = animator.GetFloat("Speed");
        animator.SetFloat("Speed", Mathf.Lerp(animSpeed, moveSpeed, Time.deltaTime * 10.0f));

        animator.SetBool("IsGrounded", isGrounded);
    }

    private void UpdateInput()
    {
        if (InputController.instance == null || PlayerController.instance.IsDead) return;

        horizontalMovement = InputController.instance.Move.x;
        verticalMovement = InputController.instance.Move.y;

        if (InputController.instance.Jump && isGrounded)
        {
            animator.SetTrigger("Jump");
            doJump = true;
            InputController.instance.Jump = false;
        }
    }

    private void UpdateMovement()
    {
        if(InputController.instance == null) { return; }
        if (!isGrounded)
        {
            verticalMovement = 0;
            horizontalMovement = 0;
        }

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;

        if (verticalMovement == 0 && horizontalMovement == 0)
        {
            moveSpeed = 0;
        }
        else
        {
            moveSpeed = 1;
        }

        if (InputController.instance.Sprint && verticalMovement >= 0)
        {
            rigidBody.AddForce(10f * runSpeed * moveDirection, ForceMode.Force);
            moveSpeed *= 2;
        }
        else
        {
            rigidBody.AddForce(10f * walkSpeed * moveDirection, ForceMode.Force);
        }

        if (doJump)
        {
            rigidBody.AddForce(jumpSpeed * Vector3.up, ForceMode.Impulse);
            InputController.instance.Jump = false;
            doJump = false;
        }

        SpeedControl();
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new(rigidBody.velocity.x, 0f, rigidBody.velocity.z);

        if (InputController.instance.Sprint)
        {
            if (flatVel.magnitude > runSpeed && !IsDashing)
            {
                Vector3 limitedVal = flatVel.normalized * runSpeed;
                rigidBody.velocity = new Vector3(limitedVal.x, rigidBody.velocity.y, limitedVal.z);
            }
        }
        else
        {
            if (flatVel.magnitude > walkSpeed && !IsDashing)
            {
                Vector3 limitedVal = flatVel.normalized * walkSpeed;
                rigidBody.velocity = new Vector3(limitedVal.x, rigidBody.velocity.y, limitedVal.z);
            }
        }
    }

    Coroutine dashCoroutine;
    public void StartDash(float speed, float duration)
    {
        if (dashCoroutine == null)
        {
            dashCoroutine = StartCoroutine(DashMovement(speed, duration));
        }
        else
        {
            StopCoroutine(dashCoroutine);
            dashCoroutine = StartCoroutine(DashMovement(speed, duration));
        }
    }

    public IEnumerator DashMovement(float speed, float duration)
    {
        IsDashing = true;
        Vector3 _direction = orientation.forward * speed;
        rigidBody.AddForce(_direction, ForceMode.Impulse);

        yield return new WaitForSeconds(duration);
        dashCoroutine = null;
        IsDashing = false;

    }
}
