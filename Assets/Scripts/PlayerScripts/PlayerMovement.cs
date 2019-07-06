﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask groundLayerMask;

    private readonly float runSpeed = 1.75f;
    private readonly float jumpForce = 250f;
    private readonly float hopForce = 85.0f;
    private readonly float fallMultiplier = 2f;
    private readonly float lowJumpMultiplier = 1.5f;
    private float jumpTimer = 0.08f;
    private float hitOnceTimer = 0.15f;
    private bool isJumping;
    private bool isHoping;
    private bool isGrounded;
    private bool isPoweredUp;
    private bool isPausered;
    private bool hasHitOnce;


    void Update()
    {
        if (GameManager.isScrollingOn)
        {
            if (!isPausered)
            {
                Run();
                if (!isHoping)
                {
                    Jump();
                }
            }
            CheckGround();
        }
        else if (isPausered)
        {
            rb.velocity = new Vector2(0.0f, 0.0f);
            animator.SetBool("IsRunning", false);
            if (Input.GetMouseButtonDown(0))
            {
                GameManager.isScrollingOn = true;
                isPausered = false;
            }
        }

        if (hasHitOnce)
        {
            hitOnceTimer -= Time.deltaTime;
            if (hitOnceTimer <= 0)
            {
                hasHitOnce = false;
                hitOnceTimer = 0.05f;
            }
        }
    }

    void FixedUpdate()
    {
        CheckVerticalVelocity();
    }

    private void Run()
    {
        rb.velocity = new Vector2(runSpeed, rb.velocity.y);
        animator.SetBool("IsRunning", true);
    }

    private void Jump()
    {
        if (isGrounded)
        {
            if (Input.GetMouseButtonDown(0))
            {
                FindObjectOfType<AudioManager>().Play("Jump");
                isJumping = true;
                jumpTimer = 0.08f;
                rb.AddForce(new Vector2(0.0f, jumpForce));
            }
            animator.SetBool("IsJumping", false);
        }
        else
        {
            if (Input.GetMouseButton(0) && isJumping)
            {
                if (jumpTimer >= 0)
                {
                    rb.AddForce(new Vector2(0.0f, 30.0f));
                    jumpTimer -= Time.deltaTime;
                }
                else
                {
                    isJumping = false;
                }
            }
            animator.SetBool("IsJumping", true);
        }
        if (Input.GetMouseButtonUp(0))
        {
            isJumping = false;
        }

    }

    private void CheckVerticalVelocity()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void CheckGround()
    {
        Vector2 boxPosition = new Vector2(transform.position.x, transform.position.y-0.3f);
        Vector2 boxSize = new Vector2(0.2f, 0.2f);
        float boxAngle = 0.0f;

        isGrounded = Physics2D.OverlapBox(boxPosition, boxSize, boxAngle, groundLayerMask);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("PowerUp"))
        {
            if (!isPoweredUp)
            {
                Time.timeScale = 0.0f;
                StartCoroutine(PoweringUp());
                GameManager.isScrollingOn = false;
                isPoweredUp = true;
                rb.velocity = new Vector2(0.0f, 0.0f);
            }
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            foreach (ContactPoint2D point in other.contacts)
            {
                if (!hasHitOnce)
                {
                    if (!(point.normal.y >= 0.99f))
                    {
                        rb.AddForce(new Vector2(0.0f, hopForce * 3f));
                    }
                    else
                    {
                        FindObjectOfType<AudioManager>().Play("Stomp");
                        rb.AddForce(new Vector2(0.0f, hopForce * 6f));
                        other.gameObject.GetComponent<IEnemy>().Stomped();
                    }
                    hasHitOnce = true;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pauser"))
        {
            GameManager.isScrollingOn = false;
            isPausered = true;
        }
        else if (other.gameObject.CompareTag("Vault"))
        {
            isHoping = true;
            if (isGrounded)
            {
                rb.AddForce(new Vector2(0.0f, hopForce * 3f));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Vault"))
        {
            isHoping = false;
        }
    }


    IEnumerator PoweringUp()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        transform.localScale = new Vector3(1.2f, 1.2f, 1.0f);
        yield return new WaitForSecondsRealtime(0.1f);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        yield return new WaitForSecondsRealtime(0.1f);
        transform.localScale = new Vector3(1.2f, 1.2f, 1.0f);
        yield return new WaitForSecondsRealtime(0.1f);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        yield return new WaitForSecondsRealtime(0.1f);
        transform.localScale = new Vector3(1.2f, 1.2f, 1.0f);
        Time.timeScale = 1.0f;
        GameManager.isScrollingOn = true;
    }

}