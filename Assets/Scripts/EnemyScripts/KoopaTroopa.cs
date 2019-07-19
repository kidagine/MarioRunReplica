using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoopaTroopa : MonoBehaviour, IEnemy
{

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject impactEffectPrefab;
    [SerializeField] private GameObject coin1UpPrefab;
    [SerializeField] private GameObject coin2UpPrefab;
    [SerializeField] private GameObject coin4UpPrefab;
    [SerializeField] private GameObject player;
    [SerializeField] private bool isFacingRight;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private CircleCollider2D circleCollider;
    private float runSpeed = 0.8f;
    private int killStreak;
    private bool isSpinning;
    private bool isInsideMainCamera;
    private bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        if (isFacingRight)
        {
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    void Update()
    {
        Walk();
        CheckForPlayerDistance();
    }

    public void Walk()
    {
        if (canMove && isInsideMainCamera)
        {
            if (!isFacingRight)
            {
                rb.velocity = new Vector2(runSpeed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(-runSpeed, rb.velocity.y);
            }
        }
    }

    public void Stomped(int killStreak)
    {
        if (!isSpinning)
        {
            FindObjectOfType<AudioManager>().Play("Kick");
            animator.SetTrigger("Stomped");
            Instantiate(impactEffectPrefab, new Vector2(transform.position.x, transform.position.y + 0.05f), Quaternion.identity);
            canMove = false;
            transform.Translate(Vector2.right * 0.0f);
        }
    }

    public void Spin()
    {
        animator.SetTrigger("Spin");
        canMove = true;
        isSpinning = true;
        if (isFacingRight)
        {
            runSpeed = -runSpeed * 4f;
        }
        else
        {
            runSpeed = runSpeed * 4f;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
            isFacingRight = !isFacingRight;
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            if (isSpinning)
            {
                killStreak++;
                other.gameObject.GetComponent<Goomba>().Hit(killStreak);
            }
        }
    }

    private void CheckForPlayerDistance()
    {
        if (!isInsideMainCamera)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < 3.5f)
            {
                isInsideMainCamera = true;
            }
        }
    }

    public void Hit(int killstreak)
    {
        FindObjectOfType<AudioManager>().Play("Stomp");
        animator.SetTrigger("Stomped");
        HandleKillStreak(killstreak);
        Instantiate(impactEffectPrefab, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
        rb.gravityScale = 0.0f;
        boxCollider.enabled = false;
        circleCollider.enabled = false;
        Vector2 startingPoint = new Vector2(transform.position.x, transform.position.y);
        Vector2 targetPoint = new Vector2(transform.position.x + 2.5f, transform.position.y - 3f);
        Vector2 controlPoint = startingPoint + (targetPoint - startingPoint) / 2 + Vector2.up * 5.0f;
        StartCoroutine(Launch(startingPoint, targetPoint, controlPoint));
    }

    IEnumerator Launch(Vector2 startingPoint, Vector2 targetPoint, Vector2 controlPoint)
    {
        bool isDestroyed = false;
        float ratio = 0.0f;
        float rotationValue = 0.0f;
        float timeUntilDestroy = 2.0f;

        while (!isDestroyed)
        {
            if (timeUntilDestroy >= 0.0f)
            {
                Vector3 m1 = Vector3.Lerp(startingPoint, controlPoint, ratio);
                Vector3 m2 = Vector3.Lerp(controlPoint, targetPoint, ratio);
                transform.position = Vector3.Lerp(m1, m2, ratio);

                transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y, rotationValue));

                timeUntilDestroy -= Time.deltaTime;
                rotationValue -= 35 * Time.deltaTime;
                ratio += 1.0f * Time.deltaTime;
                yield return null;
            }
            else
            {
                isDestroyed = true;
                Destroy(gameObject);
            }
        }
    }

    private void HandleKillStreak(int playerKillStreak)
    {
        if (playerKillStreak == 1)
        {
            Instantiate(coin1UpPrefab, new Vector2(transform.position.x, transform.position.y + 0.05f), Quaternion.identity);
            FindObjectOfType<GameManager>().IncrementCoins(1);
        }
        else if (playerKillStreak == 2)
        {
            Instantiate(coin2UpPrefab, new Vector2(transform.position.x, transform.position.y + 0.05f), Quaternion.identity);
            FindObjectOfType<GameManager>().IncrementCoins(2);
        }
        else if (playerKillStreak >= 3)
        {
            Instantiate(coin4UpPrefab, new Vector2(transform.position.x, transform.position.y + 0.05f), Quaternion.identity);
            FindObjectOfType<GameManager>().IncrementCoins(3);
        }
    }

    }
