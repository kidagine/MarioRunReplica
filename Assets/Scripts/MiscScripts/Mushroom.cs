using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject coin5UpPrefab;

    private Rigidbody2D rb;
    private Vector2 startingPoint;
    private Vector2 targetPoint;
    private Vector2 controlPoint;
    private bool isFacingRight = true;
    private bool isCollided;
    private float ratio;
    private float runSpeed = 2.0f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startingPoint = new Vector2(transform.position.x, transform.position.y);
        targetPoint = new Vector2(transform.position.x + 2.5f, transform.position.y - 1f);
        controlPoint = startingPoint + (targetPoint - startingPoint) / 2 + Vector2.up * 2.0f;
    }

    void Update()
    {
        if (ratio < 1.0f)
        {
            LaunchItem();
        }
        else
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
    }

    private void LaunchItem()
    {
        ratio += 1.2f * Time.deltaTime;

        Vector3 m1 = Vector3.Lerp(startingPoint, controlPoint, ratio);
        Vector3 m2 = Vector3.Lerp(controlPoint, targetPoint, ratio);
        transform.position = Vector3.Lerp(m1, m2, ratio);
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Vault") || other.gameObject.CompareTag("Wall"))
        {
            isFacingRight = !isFacingRight;
        }
    }


}
