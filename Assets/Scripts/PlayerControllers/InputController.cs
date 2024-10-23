using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]
public class InputController : MonoBehaviour
{
    public static InputController instance;

    public Vector2 Move;
    public Vector2 Look;
    public bool Sprint;
    public bool Jump;
    public bool Interact;
    public bool[] Ability = {false,false,false,false};

    [SerializeField] GameObject testDummy;

    public void OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Look = context.ReadValue<Vector2>();
    }

    public void OnAbility1(InputAction.CallbackContext context)
    {
        Ability[0] = context.performed;
    }

    public void OnAbility2(InputAction.CallbackContext context)
    {
        Ability[1] = context.performed;
    }

    public void OnAbility3(InputAction.CallbackContext context)
    {
        Ability[2] = context.performed;
    }

    public void OnAbility4(InputAction.CallbackContext context)
    {
        Ability[3] = context.performed;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        Sprint = context.performed;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump = context.performed;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        Interact = context.performed;
    }

    public void OnDebug(InputAction.CallbackContext context)
    {
    }

    void Start()
    {
        //Setup cursor logic to be visisble and locked
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        instance = this;
    }
}
