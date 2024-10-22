using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [SerializeField] float cameraRange;

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = PlayerController.instance.transform.position;
        newPosition.y += cameraRange;
        transform.position = newPosition;
    }
}
