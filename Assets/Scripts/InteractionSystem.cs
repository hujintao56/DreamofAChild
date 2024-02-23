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
    // Carrying object?
    private GameObject carryingObject;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && carryingObject)
        {
            DropObject();
        }

        if (DetectObject())
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
