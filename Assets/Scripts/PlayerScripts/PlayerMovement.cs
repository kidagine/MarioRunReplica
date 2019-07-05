using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask groundLayerMask;

    private readonly float runSpeed = 1.75f;
    private readonly float jumpForce = 350.0f;
    private readonly float hopForce = 85.0f;
    private bool isGrounded;
    private bool isHopping;
    private bool isPoweredUp;


    void Update()
    {
        if (GameManager.isScrollingOn)
        {
            Run();
            Jump();
            CheckGround();
        }
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
            animator.SetBool("IsJumping", false);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                FindObjectOfType<AudioManager>().Play("Jump");
                rb.AddForce(new Vector2(0.0f, jumpForce));
            }
        }
        else
        {
            animator.SetBool("IsJumping", true);
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
        if (other.gameObject.CompareTag("Wall"))
        {
            foreach (ContactPoint2D point in other.contacts)
            {
                   
                if (!(point.normal.y >= 0.9f))
                {
                        isHopping = true;
                        rb.AddForce(new Vector2(0.0f, hopForce));
                }
            }
        }
        if (other.gameObject.CompareTag("Enemy"))
        {
            foreach (ContactPoint2D point in other.contacts)
            {
                if (!(point.normal.y >= 0.9f))
                {
                    if (isGrounded)
                    {
                    }
                    isHopping = true;
                    rb.AddForce(new Vector2(0.0f, hopForce * 1.3f));
                }
                else
                {
                    FindObjectOfType<AudioManager>().Play("Stomp");
                    rb.AddForce(new Vector2(0.0f, hopForce * 1.2f));
                    other.gameObject.GetComponent<Goomba>().Stomped();
                }
            }
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
