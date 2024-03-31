using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Plant : MonoBehaviour
{
    public float minHeight = 2.0f; // Plant变矮后的最小高度
    private float shrinkSpeed = 2.0f; // Plant缩小的速度
    private bool isShrinking = false; // 是否正在缩小
    public float times = 1.0f; // 图像倍率

    private void Update()
    {
        if (isShrinking)
        {
            float currentHeight = transform.localScale.y;
            float newHeight = Mathf.Max(currentHeight - shrinkSpeed * Time.deltaTime, minHeight);
            float heightChange = currentHeight - newHeight; // 计算高度变化量

            transform.localScale = new Vector3(transform.localScale.x, newHeight, transform.localScale.z);

            // 将物体向下移动高度变化的一半，以保持底部位置固定
            transform.position -= new Vector3(0, heightChange * times * 2, 0);

            if (newHeight == minHeight)
            {
                isShrinking = false;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Item"))
        {
            isShrinking = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Item"))
        {
            isShrinking = false;
        }
    }
}