using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{

    [SerializeField] private bool isCoinBlock;

    private Animator animator;
    private Rigidbody2D rb;


    void Start()
    {
        if (isCoinBlock)
        {
            animator = gameObject.GetComponent<Animator>();
            animator.SetTrigger("CoinBlockFlip");
        }
    }

    public void AddForce(bool isPositive)
    {
        rb = GetComponent<Rigidbody2D>();
        if (isPositive)
        {
            float randomXForce = Random.Range(155.0f, 225.0f);
            float randomYForce = Random.Range(40.0f, 75.0f);
            rb.AddForce(new Vector2(randomXForce, randomYForce));
        }
        else
        {
            float randomXForce = Random.Range(-100.0f, -120.0f);
            float randomYForce = Random.Range(40.0f, 60.0f);
            rb.AddForce(new Vector2(randomXForce, randomYForce));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            FindObjectOfType<AudioManager>().Play("CoinPickUp");
            FindObjectOfType<GameManager>().IncrementCoins(1);
            Destroy(gameObject);
        }
    }

}
