using System.Collections;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using static UnityEditor.Progress;

public class InteractionSystem : MonoBehaviour
{
    // Detection Point
    public Transform detectionPoint;
    // Detection Radius
    private const float detectionRadius = 0.2f;
    // Detection Layer
    public LayerMask detectionLayer;
    // Long Press defined as
    public float longPressDuration = 2f;
    // Check packing time
    private float currentPressDuration = 0f;
    // Detected Item
    private GameObject targetObject;
    // Carrying object
    private GameObject carryingObject;
    // player
    public PlayerMovement playerMovement;
    // silk prefab
    public GameObject silkPrefab;
    // silk
    private GameObject currentSilk;
    // Spinning
    public bool isSpittingSilk = false;
    // ceiling position when start spinning
    private Vector2 ceilingPosition;

    [SerializeField] private Rigidbody2D rb;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && carryingObject)
        {
            DropObject();
        }

        if (!isSpittingSilk && DetectObject())
        {
            Item item = targetObject.GetComponent<Item>();

            if (Input.GetKeyDown(KeyCode.Q) && item.isPacked)
            {
                CarryObject();
            }

            if (LongPressing())
            {
                //Debug.Log("interacting");
                currentPressDuration += Time.deltaTime;  // time counting 
                Debug.Log("Pressing...");

                if (currentPressDuration >= 2f)
                {
                    if (item != null && !item.isPacked)
                    {
                        Debug.Log("Two seconds reached. Packing completed!");
                        currentPressDuration = 0f;
                        PackObject(targetObject); // pass target object to PackObject method
                    }
                    else
                    {
                        Debug.Log("Item is already packed or does not have an ItemScript component.");
                    }
                }
            }
            else
            {
                //Debug.Log("long pressing stoped");
                currentPressDuration = 0f;  // reset time counting 
                targetObject = null; // reset target
            }
        }

        if ((playerMovement.isOnCeiling || isSpittingSilk) && LongPressing())
        {
            if (!isSpittingSilk)
            {
                StartSpittingSilk(); // 开始吐丝
                isSpittingSilk = true; // 标记为正在吐丝
            }

            MoveDownSlowly(); // 玩家缓慢下移
            UpdateSilk(); // 更新蛛丝状态
        }
        else
        {
            isSpittingSilk = false; // 停止吐丝
        }
    }

    public void StartSpittingSilk()
    {
        ceilingPosition = transform.position; // 假设玩家当前位置就是ceiling的位置
        ceilingPosition.y += 0.5f;
        currentSilk = Instantiate(silkPrefab, ceilingPosition, Quaternion.identity); // 生成蛛丝
    }

    public void UpdateSilk()
    {
        if (currentSilk != null)
        {
            Vector2 playerPosition = transform.position;
            float distance = Vector2.Distance(playerPosition, ceilingPosition); // 计算距离
            Vector2 midPoint = (playerPosition + ceilingPosition) / 2; // 计算中间点

            // 调整蛛丝位置
            currentSilk.transform.position = midPoint;

            // 调整蛛丝长度
            currentSilk.transform.localScale = new Vector3(currentSilk.transform.localScale.x, distance, currentSilk.transform.localScale.z);
            //currentSilk.transform.localScale = new Vector3(distance * 0.25f, currentSilk.transform.localScale.y, currentSilk.transform.localScale.z);

            // 可能需要调整蛛丝的旋转以确保它垂直
            currentSilk.transform.rotation = Quaternion.Euler(0, 0, 0);
            //currentSilk.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
    }

    private void MoveDownSlowly()
    {
        rb.velocity = new Vector2(rb.velocity.x, -playerMovement.speed / 2); // 假设下移速度为常规速度的一半
    }

    private void DropObject()
    {
        carryingObject.SetActive(true);
        carryingObject.transform.position = transform.position;
        carryingObject = null;
    }

    private void CarryObject()
    {
        carryingObject = targetObject;
        carryingObject.SetActive(false);
    }

    private void PackObject(GameObject obj)
    {
        SpriteRenderer itemRenderer = obj.GetComponent<SpriteRenderer>();
        Item item = obj.GetComponent<Item>();

        if (itemRenderer != null)
        {
            itemRenderer.color = Color.yellow;
            item.isPacked = true;
            // TODO: other changes goes here
        }
        else
        {
            Debug.LogError("Item does not have a SpriteRenderer component.");
        }
    }

    private bool LongPressing()
    {
        return Input.GetKey(KeyCode.E);
    }

    private bool DetectObject()
    {
        Collider2D collider = Physics2D.OverlapCircle(detectionPoint.position, detectionRadius, detectionLayer);
        if (collider != null)
        {
            targetObject = collider.gameObject; // assign collider to targetObject
            return true;
        }
        else
        {
            targetObject = null; // no collider, reset target
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(detectionPoint.position, detectionRadius);
    }
}
