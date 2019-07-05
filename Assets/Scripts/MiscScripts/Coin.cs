using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{

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
