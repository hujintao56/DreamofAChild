using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierMove : MonoBehaviour
{
    public List<Transform> startPoints;
    public List<Transform> controlPoints;
    public List<Transform> endPoints;
    private Transform startPoint;
    private Transform controlPoint;
    private Transform endPoint;
    private float t;
    public float duration = 1.0f; // 从A到B移动的持续时间
    private Quaternion startRotation;
    private Quaternion endRotation;
    private bool isMoving = false; // 添加一个标志来控制移动状态
    public int num = 0;

    void Update()
    {
        startPoint = startPoints[num];
        endPoint = endPoints[num];
        controlPoint = controlPoints[num];

        if (isMoving) // 如果isMoving为true，则执行移动和旋转
        {
            if (t < 1.0f)
            {
                t += Time.deltaTime / duration; // 确保在指定的持续时间内完成动画

                // 计算当前贝塞尔曲线上的点
                Vector3 position = CalculateBezierPoint(t, startPoint.position, controlPoint.position, endPoint.position);
                transform.position = position;

                // 计算旋转
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            }
            else
            {
                isMoving = false; // 当动画完成时，停止移动
            }
        }
    }

    public void StartBezierMove()
    {
        t = 0.0f; // 重置t值以便重新开始动画
        startRotation = transform.rotation; // 重置初始旋转
        endRotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, 360)); // 重置结束旋转,顺时针旋转x度
        isMoving = true; // 开始移动
    }

    // 二次贝塞尔曲线计算
    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;
    }
}
