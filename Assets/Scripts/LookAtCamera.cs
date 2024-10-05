using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private bool canRotateX = false;
    [SerializeField] private bool canRotateY = false;

    private Transform cameraTransform;
    private float initialYRotation;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        initialYRotation = transform.eulerAngles.y;
    }
    private void LateUpdate()
    {
        if (canRotateX)
        {
            Vector3 lookTarget = new Vector3(cameraTransform.position.x, transform.position.y, cameraTransform.position.z);
            transform.LookAt(2 * transform.position - lookTarget);
        }
        else
        {
            transform.LookAt(2 * transform.position - cameraTransform.position);
        }

        if (canRotateY)
        {
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.y = initialYRotation;
            transform.eulerAngles = currentRotation;
        }
    }

}
