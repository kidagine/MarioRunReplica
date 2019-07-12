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
    private readonly float jumpForce = 250f;
    private readonly float hopForce = 85.0f;
    private readonly float lowJumpMultiplier = 1.0f;
    private float fallMultiplier = 1.5f;
    private float jumpTimer = 0.08f;
    private float hitOnceTimer = 0.2f;
    private float spinJumpCooldownTimer = 0.0f;
    private int killStreak;
    private bool isJumping;
    private bool isHoping;
    private bool isAirSpinning;
    private bool isGrounded;
    private bool hasHitOnce;


    void Update()
    {
        if (GameManager.isScrollingOn)
        {
            if (!GameManager.isPausered)
            {
                Run();
                if (!isHoping)
                {
                    Jump();
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
        if (!EventSystem.current.IsPointerOverGameObject())
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
                        rb.velocity = new Vector2(0, 0);
                        isAirSpinning = true;
                    }
                }
                if (isAirSpinning)
                {
                    spinJumpCooldownTimer -= Time.deltaTime;
                }

                animator.SetBool("IsJumping", true);
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
        Vector2 boxPosition = new Vector2(transform.position.x, transform.position.y-0.3f);
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
            animator.SetTrigger("FlagWon");
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            transform.position = new Vector2(other.transform.position.x + 0.2f, transform.position.y);
        }
        else if (other.gameObject.CompareTag("Death"))
        {
            FindObjectOfType<AudioManager>().Play("Death");
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
