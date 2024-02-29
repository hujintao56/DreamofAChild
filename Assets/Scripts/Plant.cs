using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public float minHeight = 2.0f; // Plant变矮后的最小高度
    private float shrinkSpeed = 2.0f; // Plant缩小的速度
    private bool isShrinking = false; // 是否正在缩小

    private void Update()
    {
        if (isShrinking)
        {
            // 计算新的高度
            float newHeight = Mathf.Max(transform.localScale.y - shrinkSpeed * Time.deltaTime, minHeight);
            float heightChange = transform.localScale.y - newHeight; // 计算高度变化量
            transform.localScale = new Vector3(transform.localScale.x, newHeight, transform.localScale.z);

            // 调整位置以使植物从上往下缩小
            transform.position -= new Vector3(0, heightChange / 2, 0); // 由于缩放，需要调整位置

            // 如果已经达到最小高度，则停止缩小
            if (newHeight == minHeight)
            {
                isShrinking = false;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 检查碰撞的对象是否是Item
        if (collision.collider.gameObject.CompareTag("Item"))
        {
            // 开始缩小
            isShrinking = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Item"))
        {
            // 开始缩小
            isShrinking = false;
        }
    }
}
