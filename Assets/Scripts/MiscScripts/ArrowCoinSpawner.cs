using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCoinSpawner : MonoBehaviour
{

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite arrowCoinDisabled;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            spriteRenderer.sprite = arrowCoinDisabled;
            StartCoroutine(SpawnCoins());
        }
    }

    IEnumerator SpawnCoins()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.05f);
        }
    }

}
