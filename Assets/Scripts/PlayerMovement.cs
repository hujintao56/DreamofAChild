using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float vertical;
    private float speed = 8f;
    private bool isFacingRight = true;
    public bool isClimbing = false;
    public bool isOnFloor = true;
    public bool isOnCeiling = false;
    public bool isAnimating = false;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    public BezierMove bezierMoveScript; // 引用BezierMove脚本

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        Flip();
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            // 在爬墙，能垂直方向移动
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
        if (!isClimbing && !isOnFloor && !isOnCeiling)
        {
            rb.gravityScale = 1f;
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Enter Wall");
            isClimbing = true;
        }
        else if (collision.gameObject.CompareTag("Floor"))
        {
            Debug.Log("Enter Floor");
            isOnFloor = true;
            rb.gravityScale = 1f;
        }
        else if (collision.gameObject.CompareTag("Ceiling"))
        {
            Debug.Log("Enter Ceiling");
            isOnCeiling = true;
        }
        else if (collision.gameObject.CompareTag("GoUp"))
        {
            Debug.Log("Animating");
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
