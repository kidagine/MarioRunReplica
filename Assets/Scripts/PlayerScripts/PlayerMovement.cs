using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{

    public Rigidbody2D rb;
    [SerializeField] private PlayerCheckUp playerCheckUp;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ParticleSystem starParticle;
    [SerializeField] private GameObject coinPrefab;

    [HideInInspector] public bool IsPoweredUp;
    [HideInInspector] public bool isWallInfront;
    private GameObject bubble;
    private GameObject hopedEnemy;
    private BoxCollider2D boxCollider;
    private CircleCollider2D circleCollider;
    private readonly float runSpeed = 2.2f;
    private readonly float jumpForce = 280f;
    private readonly float hopForce = 85.0f;
    private readonly float lowJumpMultiplier = 0.5f;
    private float fallMultiplier = 1.0f;
    private float jumpTimer = 0.08f;
    private float walkingLeftTimer = 1.3f;
    private float spinJumpCooldownTimer = 0.0f;
    private float hopCooldown = 0.4f;
    private int killStreak;
    private bool isFacingRight = true;
    private bool isInvunrable;
    private bool isStarPowered;
    private bool isJumping;
    private bool isHoping;
    private bool isAirSpinning;
    private bool isGrounded;
    private bool isWallSliding;
    private bool isWallSlidingOnRight;


    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        if (GameManager.isScrollingOn)
        {
            if (!GameManager.isBubbled)
            {
                if (!GameManager.isPausered)
                {
                    if (!GameManager.hasWon)
                    {
                        Run();
                        Jump();
                    }
                }
                if (boxCollider.enabled)
                {
                    animator.SetBool("IsBubbled", false);
                    boxCollider.enabled = true;
                    circleCollider.enabled = true;
                }
                CheckGround();
            }
            else
            {
                transform.parent = bubble.transform;
                transform.position = bubble.transform.position;
                if (!boxCollider.enabled)
                {
                    animator.SetBool("IsBubbled", true);
                    boxCollider.enabled = false;
                    circleCollider.enabled = false;
                    rb.velocity = new Vector2(0.0f, rb.velocity.y);
                }
            }
        }
        else if (GameManager.isPausered)
        {
            rb.velocity = new Vector2(0.0f, 0.0f);
            animator.SetBool("IsRunning", false);
            if (Input.GetMouseButtonDown(0))
            {
                GameManager.isScrollingOn = true;
                GameManager.isPausered = false;
                if (isWallSliding)
                {
                    Jump();
                }
            }
        }
        CheckVerticalVelocity();
    }

    private void FixedUpdate()
    {
        CheckRight();
    }

    private void CheckRight()
    {
        Vector2 raycastPosition = new Vector2(transform.position.x + 0.5f, transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(raycastPosition, Vector2.right, 0.05f);
        Debug.DrawRay(raycastPosition, Vector2.right, Color.green, 0.05f, false);
        if (hit.collider != null)
        {
            if (!isStarPowered)
            {
                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    if (!isHoping)
                    {
                        hopedEnemy = null;
                        hopedEnemy = hit.collider.gameObject;
                        animator.SetBool("IsJumping", true);
                        rb.AddForce(new Vector2(0.0f, hopForce * 2.2f));
                        isHoping = true;
                    }
                }
            }
            if (hit.collider.gameObject.CompareTag("Vault"))
            {
                if (!isHoping)
                {
                    animator.SetBool("IsJumping", true);
                    rb.AddForce(new Vector2(0.0f, hopForce * 2.2f));
                    isHoping = true;
                }
            }
        }
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
        if (!isWallInfront)
        {
            animator.SetBool("IsRunning", true);
        }
    }

    private void Jump()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (!isWallSliding)
            {
                if (!isHoping)
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

                        if (!isStarPowered)
                        {
                            killStreak = 0;
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
                        if (spinJumpCooldownTimer <= 0)
                        {
                            if (Input.GetMouseButtonDown(0) && !isWallInfront)
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
                        animator.SetBool("IsRunning", true);
                        animator.SetBool("IsJumping", true);
                    }
                }
                else
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        animator.SetBool("IsSpinning", true);
                        if (hopedEnemy != null)
                        {
                            IEnemy iEnemy = hopedEnemy.GetComponent<IEnemy>();
                            iEnemy.Stomped(killStreak);
                        }
                        spinJumpCooldownTimer = 1.2f;
                        rb.AddForce(new Vector2(0.0f, 150));
                        jumpTimer = 0.1f;
                        Debug.Log("A");
                    }
                    if (Input.GetMouseButton(0))
                    {
                        if (jumpTimer <= 0)
                        {
                            Debug.Log("S");
                            rb.AddForce(new Vector2(0.0f, 150.0f));
                            isHoping = false;
                        }
                        jumpTimer -= Time.deltaTime;
                    }
                    hopCooldown -= Time.deltaTime;
                    if (hopCooldown <= 0.0f)
                    {
                        isHoping = false;
                        animator.SetBool("IsJumping", true);
                        animator.SetBool("IsRunning", true);
                        hopCooldown = 0.4f;
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    FindObjectOfType<AudioManager>().Play("Jump");
                    animator.SetBool("IsWallJumping", true);
                    isWallInfront = false;
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
        Vector2 boxSize = new Vector2(0.05f, 0.2f);
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
                rb.velocity = new Vector2(0.0f, 0.0f);
            }
        }
        else if (other.gameObject.CompareTag("Star"))
        {
            isStarPowered = true;
            StartCoroutine(StarPower());
            StartCoroutine(StarCooldown());
        }
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (!isInvunrable)
            {
                Vector2 direction = transform.position - other.transform.position;
                if (!isStarPowered)
                {
                    if (!isWallInfront)
                    {
                        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
                        {
                            if (direction.y > 0)
                            {
                                if (!isHoping)
                                {
                                    rb.AddForce(new Vector2(0.0f, hopForce * 5f));

                                    killStreak++;
                                    other.gameObject.GetComponent<IEnemy>().Stomped(killStreak);
                                }
                            }
                            else
                            {
                                if (!IsPoweredUp)
                                {
                                    if (FindObjectOfType<GameManager>().GetBubblesAmount() == 0)
                                    {
                                        FindObjectOfType<AudioManager>().Play("Death");
                                        FindObjectOfType<AudioManager>().Pause("FirstStageBGM");
                                        FindObjectOfType<AudioManager>().Pause("Jump");
                                        rb.constraints = RigidbodyConstraints2D.FreezeAll;
                                        animator.SetTrigger("Death");
                                    }
                                    else
                                    {
                                        LoseCoins();
                                        FindObjectOfType<GameManager>().CreateBubble();
                                    }
                                }
                                else
                                {
                                    FindObjectOfType<AudioManager>().Play("PowerDown");
                                    isInvunrable = true;
                                    Time.timeScale = 0.0f;
                                    GameManager.isScrollingOn = false;
                                    rb.velocity = new Vector2(0.0f, 0.0f);
                                    IsPoweredUp = false;
                                    LoseCoins();
                                    StartCoroutine(PoweringDown());
                                }
                            }
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        Hit();
                    }
                }
                else
                {
                    killStreak++;
                    other.gameObject.GetComponent<IEnemy>().Hit(killStreak);
                }
            }
            else
            {
                Physics2D.IgnoreLayerCollision(11, 12, true);
            }
        }
        else if (other.gameObject.CompareTag("Wall"))
        {      
            animator.SetBool("IsWallJumping", false);
            animator.SetBool("IsRunning", false);
            isWallInfront = true;
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
                animator.SetBool("IsRunning", true);
                animator.SetBool("IsWallSliding", true);
                isWallSliding = true;
                rb.velocity = new Vector2(0, -1.5f);
            }
            else
            {
                animator.SetBool("IsRunning", false);
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
            isWallInfront = false;
            GameManager.isScrollingOn = true;
            GameManager.isPausered = false;
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
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsRunning", false);
            GameManager.isScrollingOn = false;
            GameManager.isPausered = true;
        }
        else if (other.gameObject.CompareTag("Flagpole"))
        {
            FindObjectOfType<AudioManager>().Play("Win");
            FindObjectOfType<AudioManager>().Pause("FirstStageBGM");
            FindObjectOfType<AudioManager>().Pause("Jump");
            FindObjectOfType<GameManager>().DisableUIButtons();
            animator.SetTrigger("Flag");
            StopStarPower(true);
            rb.gravityScale = 0;
            fallMultiplier = 0;
            rb.velocity = Vector2.zero;
            GameManager.hasWon = true;
            if (IsPoweredUp)
            {
                transform.position = new Vector2(transform.position.x + 0.34f, transform.position.y);
            }
            else
            {
                transform.position = new Vector2(transform.position.x + 0.27f, transform.position.y);
            }
        }
        else if (other.gameObject.CompareTag("Lava"))
        {
            Hit();
        }
        else if (other.gameObject.CompareTag("Death"))
        {
            if (FindObjectOfType<GameManager>().GetBubblesAmount() == 0)
            {
                FindObjectOfType<AudioManager>().Play("Death");
                FindObjectOfType<AudioManager>().Pause("FirstStageBGM");
                FindObjectOfType<AudioManager>().Pause("Jump");
                FindObjectOfType<GameManager>().GameOver();
            }
            else
            {
                LoseCoins();
                FindObjectOfType<GameManager>().CreateBubble();
            }
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

    public void Hit()
    {
        if (!isInvunrable)
        {
            if (!IsPoweredUp)
            {
                FindObjectOfType<AudioManager>().Play("Death");
                FindObjectOfType<AudioManager>().Pause("FirstStageBGM");
                FindObjectOfType<AudioManager>().Pause("Jump");
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
                animator.SetTrigger("Death");
                boxCollider.enabled = false;
                circleCollider.enabled = false;
            }
            else
            {
                FindObjectOfType<AudioManager>().Play("PowerDown");
                isInvunrable = true;
                Time.timeScale = 0.0f;
                GameManager.isScrollingOn = false;
                rb.velocity = new Vector2(0.0f, 0.0f);
                IsPoweredUp = false;
                LoseCoins();
                StartCoroutine(PoweringDown());
            }
        }
    }

    public void Death()
    {
        FindObjectOfType<GameManager>().GameOver();
    }

    private void LoseCoins()
    {
        int numberOfCoins = FindObjectOfType<GameManager>().GetCoinsAmount() / 10;
        FindObjectOfType<GameManager>().DecrementCoins(numberOfCoins);
        for (int i = 0; i < numberOfCoins; i++)
        {
            GameObject coin = Instantiate(coinPrefab, new Vector2(transform.position.x, transform.position.y + 0.5f), Quaternion.identity);
            Coin coinScript = coin.GetComponent<Coin>();
            coinScript.AddForce(false);
        }
    }

    public void InsideBubble(GameObject bubble)
    {
        this.bubble = bubble;
        GameManager.isBubbled = true;
    }

    IEnumerator StarPower()
    {
        if (isStarPowered)
        {
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.color = Color.yellow;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.color = Color.blue;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.color = Color.green;
            yield return new WaitForSeconds(0.05f);
            StartCoroutine(StarPower());
        }
        else
        {
            spriteRenderer.color = Color.white;
            yield return null;
        }
    }

    IEnumerator StarCooldown()
    {
        starParticle.Play();
        yield return new WaitForSeconds(15.0f);
        StopStarPower(false);
    }

    private void StopStarPower(bool touchedFlag)
    {
        spriteRenderer.color = Color.white;
        if (!touchedFlag)
        {
            FindObjectOfType<AudioManager>().Play("FirstStageBGM");
        }
        FindObjectOfType<AudioManager>().Pause("Star");
        isStarPowered = false;
        starParticle.Stop();
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
        IsPoweredUp = true;
        GameManager.isScrollingOn = true;
    }

    IEnumerator PoweringDown()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        yield return new WaitForSecondsRealtime(0.1f);
        transform.localScale = new Vector3(1.2f, 1.2f, 1.0f);
        yield return new WaitForSecondsRealtime(0.1f);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        yield return new WaitForSecondsRealtime(0.1f);
        transform.localScale = new Vector3(1.2f, 1.2f, 1.0f);
        yield return new WaitForSecondsRealtime(0.1f);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        Time.timeScale = 1.0f;
        GameManager.isScrollingOn = true;
        StartCoroutine(Invunrable());
    }

    IEnumerator Invunrable()
    {
        yield return new WaitForSecondsRealtime(0.13f);
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        yield return new WaitForSecondsRealtime(0.13f);
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        yield return new WaitForSecondsRealtime(0.13f);
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        yield return new WaitForSecondsRealtime(0.13f);
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        yield return new WaitForSecondsRealtime(0.13f);
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        yield return new WaitForSecondsRealtime(0.13f);
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        yield return new WaitForSecondsRealtime(0.13f);
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        yield return new WaitForSecondsRealtime(0.13f);
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        yield return new WaitForSecondsRealtime(0.13f);
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        yield return new WaitForSecondsRealtime(0.13f);
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        yield return new WaitForSecondsRealtime(0.1f);
        Physics2D.IgnoreLayerCollision(11, 12, false);
        isInvunrable = false;
    }

}
