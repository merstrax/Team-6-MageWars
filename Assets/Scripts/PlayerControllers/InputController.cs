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
        /*if (context.performed)
        {
            moveController.DoJump();
        }*/
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        Interact = context.performed;
    }

    public void OnDebug(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (SceneManager.GetActiveScene().name == "ShowcaseScene")
                SceneManager.LoadScene("Godrick Valley");
            else
                SceneManager.LoadScene("ShowcaseScene");
        }
    }

    void Start()
    {
        //Setup cursor logic to be visisble and locked
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        instance = this;
    }

    float rotX;
    float rotY;

    // Update is called once per frame
    void LateUpdate()
    {
        //float x = look.x * sensitivity * Time.deltaTime;
        //float y = -look.y * sensitivity * Time.deltaTime;
        //
        //#region Camera Controls
        //
        //followTransform.transform.rotation *= Quaternion.AngleAxis(x, Vector3.up);
        //followTransform.transform.rotation *= Quaternion.AngleAxis(y, Vector3.right);
        //
        //
        //Vector3 angles = followTransform.transform.localEulerAngles;
        //angles.z = 0;
        //
        //float angle = angles.x; //followTransform.transform.localEulerAngles.x;
        //
        //if (angle > 180 && angle < 355)
        //{
        //    angles.x = 355;
        //}
        //else if (angle < 180 && angle > 30)
        //{
        //    angles.x = 30;
        //}
        //
        //followTransform.transform.localEulerAngles = angles;
        //
        //#endregion
        //
        //#region Player Movement
        //
        //player.UpdateMoveDir(move.x, move.y);
        //
        //if(move.x == 0 && move.y == 0)
        //{
        //    return;
        //}
        //
        ////Set the player rotation based on the look transform
        //transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);
        ////reset the y rotation of the look transform
        //followTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);
        //#endregion
    }
}
