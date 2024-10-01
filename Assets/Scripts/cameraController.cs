using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
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
    }

    // Update is called once per frame
    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        //Invert Y camera
        if (invertY)
        {
            rotX += mouseY;
        }
        else
            rotX -= mouseY;

        //Clamp the rotX on the x-axis
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        //Rotate the camera on the x-axis
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        //Rotate the player on the y-axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }

    public void ChangeSensitivity(float normal)
    {
        sensitivity = normal;
    }
}
