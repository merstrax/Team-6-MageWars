using RadicalForge.Gameplay;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputController : MonoBehaviour
{
    public Vector2 move;
    public Vector2 look;
    public bool sprint;
    public bool jump;
    public bool[] ability = {false,false,false,false};

    [Header("Camera Controls")]
    [SerializeField] GameObject followTransform;
    [SerializeField] float sensitivity;

    //Player Movement
    [Header("Player Objects")]
    [SerializeField] PlayerController player;
    [SerializeField] CharacterController controller;
    [SerializeField] PlayerMovementController moveController;

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    public void OnAbility1(InputAction.CallbackContext context)
    {
        ability[0] = context.performed;
    }

    public void OnAbility2(InputAction.CallbackContext context)
    {
        ability[1] = context.performed;
    }

    public void OnAbility3(InputAction.CallbackContext context)
    {
        ability[2] = context.performed;
    }

    public void OnAbility4(InputAction.CallbackContext context)
    {
        ability[3] = context.performed;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        sprint = context.performed;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveController.DoJump();
        }
    }

    void Start()
    {
        //Setup cursor logic to be visisble and locked
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    float rotX;
    float rotY;

    // Update is called once per frame
    void Update()
    {
        float x = look.x * sensitivity * Time.deltaTime;
        float y = -look.y * sensitivity * Time.deltaTime;

        #region Camera Controls
        
        followTransform.transform.rotation *= Quaternion.AngleAxis(x, Vector3.up);
        followTransform.transform.rotation *= Quaternion.AngleAxis(y, Vector3.right);

        
        Vector3 angles = followTransform.transform.localEulerAngles;
        angles.z = 0;

        float angle = followTransform.transform.localEulerAngles.x;

        if (angle > 180 && angle < 355)
        {
            angles.x = 355;
        }
        else if (angle < 180 && angle > 30)
        {
            angles.x = 30;
        }
        
        followTransform.transform.localEulerAngles = angles;
        
        #endregion

        #region Player Movement

        player.UpdateMoveDir(move);

        if(move.x == 0 && move.y == 0)
        {
            return;
        }

        //Set the player rotation based on the look transform
        transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);
        //reset the y rotation of the look transform
        followTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);
        #endregion
    }
}
