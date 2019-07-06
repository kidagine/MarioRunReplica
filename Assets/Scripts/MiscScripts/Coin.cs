using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{

    [SerializeField] private bool isCoinBlock;

    private Animator animator;

    void Start()
    {
        if (isCoinBlock)
        {
            animator = gameObject.GetComponent<Animator>();
            animator.SetTrigger("CoinBlockFlip");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            FindObjectOfType<AudioManager>().Play("CoinPickUp");
            FindObjectOfType<GameManager>().increaseCoins(1);
            Destroy(gameObject);
        }
    }

}
