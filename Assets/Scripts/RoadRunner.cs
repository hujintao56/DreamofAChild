using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadRunner : MonoBehaviour
{
    public Transform areaStart; // 区域的开始点
    public Transform areaEnd; // 区域的结束点
    public LayerMask playerLayer; // 玩家所在的层，用于检测
    private bool isScouting = false; // 是否正在审视区域
    public SpriteRenderer areaRenderer; // 审视区域的精灵渲染器

    void Start()
    {
        StartCoroutine(ScoutRoutine()); // 开始侦察协程
    }

    void Update()
    {
        if (isScouting)
        {
            // 检查玩家是否在侦察区域内
            CheckForPlayer();
        }
    }

    IEnumerator ScoutRoutine()
    {
        while (true)
        {
            isScouting = true; // 开始审视
            StartCoroutine(FlashArea(true)); // 开始闪烁
            yield return new WaitForSeconds(3); // 审视3秒

            isScouting = false; // 休息
            StartCoroutine(FlashArea(false)); // 停止闪烁
            yield return new WaitForSeconds(3); // 休息3秒
        }
    }

    void CheckForPlayer()
    {
        // 计算侦察区域
        Vector2 center = (areaStart.position + areaEnd.position) * 0.5f;
        Vector2 size = new Vector2(Mathf.Abs(areaEnd.position.x - areaStart.position.x), Mathf.Abs(areaEnd.position.y - areaStart.position.y));

        // 检测玩家
        Collider2D hit = Physics2D.OverlapBox(center, size, 0, playerLayer);
        if (hit)
        {
            PlayerMovement player = hit.GetComponent<PlayerMovement>();
            if (player != null && !player.isHidden)
            {
                Debug.Log("你被发现了");
            }
        }
    }

    // 在编辑器中显示侦察区域
    void OnDrawGizmos()
    {
        if (areaStart != null && areaEnd != null)
        {
            Gizmos.color = Color.red;
            Vector2 center = (areaStart.position + areaEnd.position) * 0.5f;
            Vector2 size = new Vector2(Mathf.Abs(areaEnd.position.x - areaStart.position.x), Mathf.Abs(areaEnd.position.y - areaStart.position.y));

            Gizmos.DrawWireCube(center, size);
        }
    }

    IEnumerator FlashArea(bool flashing)
    {
        if (flashing)
        {
            areaRenderer.enabled = true;

            float flashDuration = 3.0f; // 闪烁持续时间
            float flashSpeed = 0.2f; // 闪烁速度
            float elapsedTime = 0;

            while (elapsedTime < flashDuration)
            {
                // 切换精灵的可见性
                areaRenderer.enabled = !areaRenderer.enabled;
                yield return new WaitForSeconds(flashSpeed);
                elapsedTime += flashSpeed;
            }
            // 确保闪烁结束时精灵是可见的
            areaRenderer.enabled = false;
        }
        else
        {
            // 停止闪烁，确保审视区域是可见的
            areaRenderer.enabled = false;
        }
    }

}
