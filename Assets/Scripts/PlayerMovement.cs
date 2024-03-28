using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float vertical;
    public float speed = 8f;
    private bool isFacingRight = true;
    public bool isClimbing = false;
    public bool isOnFloor = true;
    public bool isOnCeiling = false;
    public bool isAnimating = false;
    public bool isSilkClimbing = false;
    public bool isHidden = false;

    private Transform silkTransform;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    public BezierMove bezierMoveScript; // 引用BezierMove脚本
    public InteractionSystem interactionSystem;
    public Animator animator;

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if(math.abs(horizontal) >= math.abs(vertical))
            animator.SetFloat("Speed", math.abs(horizontal));
        else if (math.abs(vertical) > math.abs(horizontal))
            animator.SetFloat("Speed", math.abs(vertical));

        //Flip();
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            // 在爬墙，能上下移动
            rb.velocity = new Vector2(rb.velocity.x, vertical * speed);
        }
        if (isOnFloor)
        {
            // 在地面上，能左右移动
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
        if (isOnCeiling)
        {
            // 在天花板上，能左右移动
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
        if (!interactionSystem.isSpittingSilk && isSilkClimbing)
        {
            // 在蛛丝上，能上下移动
            rb.velocity = new Vector2(rb.velocity.x, vertical * speed);

            if (!isOnCeiling && !isOnFloor && !isClimbing)
            {
                // 爬绳子时调整玩家位置，使其对齐到绳子的中心
                Vector2 targetPosition = new Vector2(silkTransform.position.x, rb.position.y);
                rb.position = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.deltaTime);
            }
        }
        if (!isClimbing && !isOnCeiling && !isSilkClimbing && !isOnFloor)
        {
            rb.gravityScale = 4f;
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            //Debug.Log("On Wall");
            isClimbing = true;
        }
        else if (collision.gameObject.CompareTag("Floor"))
        {
            //Debug.Log("On Floor");
            isOnFloor = true;
            rb.gravityScale = 4f;
        }
        else if (collision.gameObject.CompareTag("Ceiling"))
        {
            //Debug.Log("On Ceiling");
            isOnCeiling = true;
        }
        else if (collision.gameObject.CompareTag("GoUp"))
        {
            //Debug.Log("Animating");
            isAnimating = true;
            bezierMoveScript.StartBezierMove();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // 离开墙壁时
            Debug.Log("Left Wall");
            isClimbing = false;
        }
        else if (collision.gameObject.CompareTag("Floor"))
        {
            // 离开地面时
            Debug.Log("Left Floor");
            isOnFloor = false;
            rb.gravityScale = 0f;
        }
        else if (collision.gameObject.CompareTag("Ceiling"))
        {
            // 离开天花板时
            Debug.Log("Left Ceiling");
            isOnCeiling = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Item") ||
            other.gameObject.layer == LayerMask.NameToLayer("Cover"))
        {
            isHidden = true;
        }

        if (other.tag == "Silk")
        {
            rb.gravityScale = 0; // 停止重力影响，开始爬绳子
            isSilkClimbing = true;
            silkTransform = other.transform; // 获取绳子的Transform
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Item") ||
            other.gameObject.layer == LayerMask.NameToLayer("Cover"))
        {
            isHidden = false;
        }

        if (other.tag == "Silk")
        {
            isSilkClimbing = false;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}
