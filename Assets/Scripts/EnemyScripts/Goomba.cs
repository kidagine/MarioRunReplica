using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : MonoBehaviour, IEnemy
{

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject impactEffectPrefab;
    [SerializeField] private GameObject coin1UpPrefab;
    [SerializeField] private GameObject coin2UpPrefab;
    [SerializeField] private GameObject coin4UpPrefab;
    [SerializeField] private GameObject player;

    [HideInInspector] public bool isFacingRight = false;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Vector2 startingPosition;
    private bool isInsideMainCamera;
    private bool canMove = true;
    private bool hitOnce;
    private float runSpeed = 0.75f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        startingPosition = transform.position;
        Physics2D.IgnoreCollision(boxCollider, player.GetComponent<CircleCollider2D>());
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
            if (isFacingRight)
            {
                rb.velocity = new Vector2(runSpeed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(-runSpeed, rb.velocity.y);
            }

        }
        else if (!isInsideMainCamera)
        {
            rb.velocity = Vector2.zero;
            transform.position = startingPosition;
        }
    }

    private void CheckForPlayerDistance()
    {
        float distance = Vector2.Distance(new Vector2(transform.position.x, 0.0f), new Vector2(player.transform.position.x, 0.0f));
        if (distance < 3.8f)
        {
            isInsideMainCamera = true;
        }
        else if (distance > 5.0f)
        {
            isInsideMainCamera = false;
        }
    }

    public void Stomped(int killStreak)
    {
        FindObjectOfType<AudioManager>().Play("Stomp");
        animator.SetTrigger("Stomped");
        HandleKillStreak(killStreak, 0);
        Instantiate(impactEffectPrefab, new Vector2(transform.position.x, transform.position.y + 0.05f), Quaternion.identity);
        canMove = false;
        transform.Translate(Vector2.right * 0.0f);
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        foreach (Collider2D i in GetComponents<Collider2D>())
        {
            i.enabled = false;
        }
        Destroy(gameObject, 0.25f);
    }

    public void Hit(int killStreak)
    {
        FindObjectOfType<AudioManager>().Play("Stomp");
        foreach (Collider2D i in GetComponents<Collider2D>())
        {
            i.enabled = false;
        }
        HandleKillStreak(0, killStreak);
        Instantiate(impactEffectPrefab, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
        rb.gravityScale = 0.0f;
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

    private void HandleKillStreak(int playerKillStreak, int KoopaTroopaKillStreak)
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

        if (KoopaTroopaKillStreak == 1)
        {
            Instantiate(coin1UpPrefab, new Vector2(transform.position.x, transform.position.y + 0.05f), Quaternion.identity);
            FindObjectOfType<GameManager>().IncrementCoins(1);
        }
        else if (KoopaTroopaKillStreak == 2)
        {
            Instantiate(coin2UpPrefab, new Vector2(transform.position.x, transform.position.y + 0.05f), Quaternion.identity);
            FindObjectOfType<GameManager>().IncrementCoins(2);
        }
        else if (KoopaTroopaKillStreak >= 3)
        {
            Instantiate(coin4UpPrefab, new Vector2(transform.position.x, transform.position.y + 0.05f), Quaternion.identity);
            FindObjectOfType<GameManager>().IncrementCoins(3);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            if (!hitOnce)
            {
                hitOnce = true;
                Vector3 theScale = transform.localScale;
                theScale.x *= -1;
                transform.localScale = theScale;
                isFacingRight = !isFacingRight;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            hitOnce = false;
        }
    }

    public void IsHopedOn(bool value)
    {
        if (value == true)
        {
            canMove = false;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            canMove = true;
            rb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
        }
    }

}
