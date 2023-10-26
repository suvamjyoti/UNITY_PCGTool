using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const string LogChannel = "CameraController";

    private Transform initialTransform;
    private Quaternion initialRotation;
    private Vector3 initialPosition;

    private void Awake()
    {
        initialTransform = transform;
        initialRotation = initialTransform.rotation;
        initialPosition = transform.position;
    }

    void Update()
    {
    }

    public void MoveCameraToTopView(Vector3 pivotPosition,int height)
    {


        pivotPosition.y = height;
        GetComponent<Camera>().orthographicSize  = height/1.5f;
        transform.position = pivotPosition;
        transform.rotation = Quaternion.Euler(new Vector3(90,0,0));
    }

    public void MoveCameraToPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void ResetCamera()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }
}


