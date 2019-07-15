using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{

    public Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask groundLayerMask;

    [HideInInspector] public bool IsPoweredUp;
    private readonly float runSpeed = 2.0f;
    private readonly float jumpForce = 200f;
    private readonly float hopForce = 85.0f;
    private readonly float lowJumpMultiplier = 1.0f;
    private float fallMultiplier = 1.5f;
    private float jumpTimer = 0.08f;
    private float hitOnceTimer = 0.2f;
    private float walkingLeftTimer = 1.3f;
    private float spinJumpCooldownTimer = 0.0f;
    private int killStreak;
    private bool isFacingRight = true;
    private bool isJumping;
    private bool isHoping;
    private bool isAirSpinning;
    private bool isGrounded;
    private bool hasHitOnce;
    private bool isWallSliding;
    private bool isWallSlidingOnRight;


    void Update()
    {
        if (GameManager.isScrollingOn)
        {
            if (!GameManager.isPausered)
            {
                if (!GameManager.hasWon)
                {
                    Run();
                    if (!isHoping)
                    {
                        Jump();
                    }
                }
            }
            CheckGround();
        }
        else if (GameManager.isPausered)
        {
            rb.velocity = new Vector2(0.0f, 0.0f);
            animator.SetBool("IsRunning", false);
            if (Input.GetMouseButtonDown(0))
            {
                GameManager.isScrollingOn = true;
                GameManager.isPausered = false;
                hasHitOnce = true;
                if (isWallSliding)
                {
                    Jump();
                }
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
        if (isFacingRight)
        {
            rb.velocity = new Vector2(runSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(-runSpeed, rb.velocity.y);
            if (walkingLeftTimer >= 0.0f)
            {
                walkingLeftTimer -= Time.deltaTime;
            }
            else
            {
                if (isGrounded)
                {
                    Flip();
                    isFacingRight = true;
                    walkingLeftTimer = 1.3f;
                }
            }
        }
        animator.SetBool("IsRunning", true);
    }

    private void Jump()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (!isWallSliding)
            {
                if (isGrounded)
                {
                    spinJumpCooldownTimer = 0f;
                    if (Input.GetMouseButtonDown(0))
                    {
                        FindObjectOfType<AudioManager>().Play("Jump");
                        isJumping = true;
                        isAirSpinning = false;
                        jumpTimer = 0.08f;
                        rb.AddForce(new Vector2(0.0f, jumpForce));
                    }

                    killStreak = 0;
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
                    if (spinJumpCooldownTimer <= 0)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            FindObjectOfType<AudioManager>().Play("SpinJump");
                            animator.SetBool("IsSpinning", true);
                            spinJumpCooldownTimer = 0.7f;
                            rb.gravityScale = 0;
                            fallMultiplier = 0;
                            rb.velocity = Vector2.zero;
                            isAirSpinning = true;
                        }
                    }
                    if (isAirSpinning)
                    {
                        spinJumpCooldownTimer -= Time.deltaTime;
                    }

                    animator.SetBool("IsJumping", true);
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    FindObjectOfType<AudioManager>().Play("Jump");
                    animator.SetBool("IsWallJumping", true);
                    isJumping = true;
                    isAirSpinning = false;  
                    jumpTimer = 0.08f;
                    rb.AddForce(new Vector2(0.0f, jumpForce));
                    Flip();
                    if (isWallSlidingOnRight)
                    {
                        isFacingRight = false;
                    }
                    else
                    {
                        isFacingRight = true;
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                isJumping = false;
            }
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
        Vector2 boxPosition = new Vector2(transform.position.x, transform.position.y - 0.3f);
        Vector2 boxSize = new Vector2(0.2f, 0.2f);
        float boxAngle = 0.0f;

        isGrounded = Physics2D.OverlapBox(boxPosition, boxSize, boxAngle, groundLayerMask);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("PowerUp"))
        {
            if (!IsPoweredUp)
            {
                Time.timeScale = 0.0f;
                StartCoroutine(PoweringUp());
                GameManager.isScrollingOn = false;
                IsPoweredUp = true;
                rb.velocity = new Vector2(0.0f, 0.0f);
            }
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            float offset = 0.2f;
            if (!hasHitOnce)
            {
                if (transform.position.y > other.transform.position.y + offset)
                {
                    FindObjectOfType<AudioManager>().Play("Stomp");
                    rb.AddForce(new Vector2(0.0f, hopForce * 5f));

                    killStreak++;
                    other.gameObject.GetComponent<IEnemy>().Stomped(killStreak);
                }
                else
                {
                    rb.AddForce(new Vector2(0.0f, hopForce * 3f));
                }
                hasHitOnce = true;
            }
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            animator.SetBool("IsWallJumping", false);
            GameManager.isScrollingOn = false;
            GameManager.isPausered = true;
            foreach (ContactPoint2D point in other.contacts)
            {
                if (point.normal.x >= 0.9f)
                {
                    isWallSlidingOnRight = false;
                }
                else if (point.normal.x < 0.9f)
                {
                    isWallSlidingOnRight = true;
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            if (rb.velocity.y < -0.1)
            {
                animator.SetBool("IsWallSliding", true);
                isWallSliding = true;
                rb.velocity = new Vector2(0, -1.5f);
            }
            else
            {
                animator.SetBool("IsWallSliding", false);
                isWallSliding = false;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            animator.SetBool("IsWallSliding", false);
            isWallSliding = false;
            GameManager.isScrollingOn = true;
            GameManager.isPausered = false;
            hasHitOnce = true;
            if (isWallSlidingOnRight)
            {
                rb.velocity = new Vector2(runSpeed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(-runSpeed, rb.velocity.y);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pauser"))
        {
            if (!hasHitOnce)
            {
                GameManager.isScrollingOn = false;
                GameManager.isPausered = true;
            }
        }
        else if (other.gameObject.CompareTag("Vault"))
        {
            isHoping = true;
            if (isGrounded)
            {
                rb.AddForce(new Vector2(0.0f, hopForce * 3f));
            }
        }
        else if (other.gameObject.CompareTag("Flagpole"))
        {   
            FindObjectOfType<AudioManager>().Play("Win");
            FindObjectOfType<AudioManager>().Pause("FirstStageBGM");
            FindObjectOfType<AudioManager>().Pause("Jump");
            animator.SetTrigger("Flag");
            rb.gravityScale = 0;
            fallMultiplier = 0;
            rb.velocity = Vector2.zero;
            GameManager.hasWon = true;
            transform.position = new Vector2(transform.position.x + 0.27f, transform.position.y);
        }
        else if (other.gameObject.CompareTag("Death"))
        {
            FindObjectOfType<GameManager>().GameOver();
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Vault"))
        {
            isHoping = false;
        }
    }

    public void ResetFromSpinJump()
    {
        animator.SetBool("IsSpinning", false);
        rb.gravityScale = 2;
        fallMultiplier = 1.5f;
    }

    public void CinematicPosition(float xValue, float yValue)
    {
        transform.position = new Vector2(transform.position.x + xValue, transform.position.y + yValue);
    }

    private void Flip()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
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
