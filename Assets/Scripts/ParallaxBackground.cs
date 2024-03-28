using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform playerTransform;
    public float parallaxSpeed = 0.02f; // 背景移动的速度

    private Vector3 previousPlayerPosition;

    void Start()
    {
        if (playerTransform != null)
        {
            previousPlayerPosition = playerTransform.position;
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            // 计算玩家的移动差值
            Vector3 deltaPlayerPosition = playerTransform.position - previousPlayerPosition;

            // 根据玩家移动的反方向移动背景
            transform.localPosition -= new Vector3(deltaPlayerPosition.x * parallaxSpeed, deltaPlayerPosition.y * parallaxSpeed, 0);

            // 更新玩家的上一个位置
            previousPlayerPosition = playerTransform.position;
        }
    }
}