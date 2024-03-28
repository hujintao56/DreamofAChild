using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform; // 玩家的Transform组件
    public float smoothSpeed = 0.125f; // 相机跟随的平滑度
    public Vector3 offset; // 相机相对于玩家的偏移量

    void LateUpdate()
    {
        Vector3 desiredPosition = playerTransform.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // 可选，让相机始终朝向玩家或某个点
        // transform.LookAt(playerTransform);
    }
}
