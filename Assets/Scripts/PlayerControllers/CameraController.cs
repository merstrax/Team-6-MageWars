using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cameraController : MonoBehaviour
{
    [SerializeField] Transform followTransform;
    [SerializeField] float sensitivity;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;

    float rotX;

    // Start is called before the first frame update
    void Start()
    {
        //Setup cursor logic to be visisble and locked
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CinemachineVirtualCamera _playerCamera = FindFirstObjectByType<CinemachineVirtualCamera>();
        _playerCamera.Follow = followTransform;
    }
    // Update is called once per frame
    void Update()
    {
        float x = InputController.instance.Look.x * sensitivity * Time.deltaTime;
        float y = -InputController.instance.Look.y * sensitivity * Time.deltaTime;
        
        #region Camera Controls
        
        followTransform.transform.rotation *= Quaternion.AngleAxis(x, Vector3.up);
        followTransform.transform.rotation *= Quaternion.AngleAxis(y, Vector3.right);
        
        
        Vector3 angles = followTransform.transform.localEulerAngles;
        angles.z = 0;
        
        float angle = angles.x; //followTransform.transform.localEulerAngles.x;
        
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
        
        if(InputController.instance.Move.x == 0 && InputController.instance.Move.y == 0)
        {
            return;
        }

        //Set the player rotation based on the look transform
        transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);
        //reset the y rotation of the look transform
        followTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);

        #endregion
    }

    public void ChangeSensitivity(float normal)
    {
        sensitivity = normal;
    }
}
