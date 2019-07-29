using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{

    [SerializeField] private bool isInsideBlock;
    [SerializeField] private GameObject coin5UpPrefab;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private GameObject player;
    private BoxCollider2D boxCollider2D;
    private Vector2 startingPoint;
    private Vector2 targetPoint;
    private Vector2 controlPoint;
    private bool isFacingRight = true;
    private bool isInsideMainCamera;
    private bool isCollided;
    private bool isLanded;
    private float runSpeed = 1.5f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.enabled = false;
        StartCoroutine(EnableCollider());
        startingPoint = new Vector2(transform.position.x, transform.position.y);
        targetPoint = new Vector2(transform.position.x + 3.0f, transform.position.y - 1.7f);
        controlPoint = startingPoint + (targetPoint - startingPoint) / 2 + Vector2.up * 2.7f;
        if (!isInsideBlock)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
            LaunchItem();
        }
        Destroy(gameObject, 10.0f);
    }

    void Update()
    {
        if (isInsideMainCamera || isInsideBlock && isLanded)
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
        CheckForPlayerDistance();
    }

    private void CheckForPlayerDistance()
    {
        if (!isInsideMainCamera && !isInsideBlock)
        {
            float distance = Vector2.Distance(new Vector2(transform.position.x, 0.0f), new Vector2(player.transform.position.x, 0.0f));
            if (distance < 3.8f)
            {
                isInsideMainCamera = true;
            }
        }
    }

    private void LaunchItem()
    {
        float xForce = Mathf.Cos(145) * 180;
        float yForce = Mathf.Sin(90) * 235;
        rb.AddForce(new Vector2(xForce, yForce));
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!isCollided)
            {
                PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
                if (playerMovement.IsPoweredUp)
                {
                    Instantiate(coin5UpPrefab, transform.position, Quaternion.identity);
                    FindObjectOfType<GameManager>().IncrementCoins(5);
                    isCollided = true;
                }
                FindObjectOfType<AudioManager>().Play("PowerUp");
                Destroy(gameObject);
            }
        }
        else if (other.gameObject.CompareTag("Ground"))
        {
            isLanded = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Vault") || other.gameObject.CompareTag("Wall"))
        {
            isFacingRight = !isFacingRight;
        }
    }

    IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(0.5f);
        boxCollider2D.enabled = true;
    }

}