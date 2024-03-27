using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxFactor = 0.1f; // 这个值越小，背景移动得越慢

    private Vector3 previousCameraPosition;

    void Start()
    {
        previousCameraPosition = cameraTransform.position;
    }

    void Update()
    {
        Vector3 deltaMovement = cameraTransform.position - previousCameraPosition;
        transform.position += deltaMovement * parallaxFactor;
        previousCameraPosition = cameraTransform.position;
    }
}