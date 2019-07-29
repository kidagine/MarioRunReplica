using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject brokenBlockPrefab;
    [SerializeField] private Animator animator;
    [SerializeField] private Sprite emptyBlockSprite;
    [SerializeField] private bool isDestructible;
    [SerializeField] private bool hasMultipleItems;

    private Transform itemInside;
    private Vector2 startingPosition;
    private SpriteRenderer spriteRenderer;
    private bool isEmpty;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startingPosition = new Vector2(transform.position.x, transform.position.y);
        if (transform.childCount > 0)
        {
            itemInside = transform.GetChild(0);
        }
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D point in other.contacts)
            {
                if (point.normal.y >= 0.9f)
                {
                    if (!isEmpty)
                    {
                        if (isDestructible)
                        {
                            if (other.gameObject.GetComponent<PlayerMovement>().IsPoweredUp)
                            {
                                FindObjectOfType<AudioManager>().Play("BreakBlock");
                                Instantiate(brokenBlockPrefab, transform.position, Quaternion.identity);
                                Destroy(gameObject);
                            }
                            else
                            {
                                StartCoroutine(ShakeUp());
                            }
                        }
                        else
                        {
                            if (itemInside != null)
                            {
                                if (itemInside.name.StartsWith("Mushroom"))
                                {
                                    FindObjectOfType<AudioManager>().Play("PowerUpAppears");
                                    StartCoroutine(ShakeUp());
                                }
                                else if (itemInside.name.StartsWith("Coin"))
                                {
                                    FindObjectOfType<AudioManager>().Play("CoinPickUp");
                                    FindObjectOfType<GameManager>().IncrementCoins(1);
                                    StartCoroutine(ShakeUp());
                                }
                                else if (itemInside.name.StartsWith("Star"))
                                {
                                    StartCoroutine(ShakeUp());
                                }
                            }
                            else if (hasMultipleItems)
                            {
                                StartCoroutine(ShakeUp());
                            }
                        }
                    }
                }
            }
        }
    }

    IEnumerator ShakeUp()
    {
        bool hasReachedTop = false;
        float ratio = 0.0f;
        Vector2 targetPosition = new Vector2(startingPosition.x, startingPosition.y + 0.2f);
        while (!hasReachedTop)
        {
            if (ratio <= 1.0f)
            {
                transform.position = Vector2.Lerp(startingPosition, targetPosition, ratio);
                ratio += 7.5f * Time.fixedDeltaTime;
                yield return null;
            }
            else
            {
                if (transform.childCount > 0)
                {
                    itemInside.gameObject.SetActive(true);
                    isEmpty = true;
                }
                else if (hasMultipleItems)
                {
                    InstantiateCoinsRandomly();
                    isEmpty = true;
                }
                hasReachedTop = true;
                StartCoroutine(ShakeDown());
            }
        }
    }

    IEnumerator ShakeDown()
    {
        bool hasReachedBottom = false;
        float ratio = 0.0f;
        Vector2 targetPosition = new Vector2(startingPosition.x, startingPosition.y + 0.2f);
        while (!hasReachedBottom)
        {
            if (ratio <= 1.0f)
            {
                transform.position = Vector2.Lerp(targetPosition, startingPosition, ratio);
                ratio += 5.0f * Time.fixedDeltaTime;
                yield return null;
            }
            else
            {
                if (isEmpty)
                {
                    spriteRenderer.sprite = emptyBlockSprite;
                    if (animator != null)
                    {
                        animator.SetBool("IsSpinning", false);
                    }
                }
                hasReachedBottom = true;
                yield return null;
            }
        }
    }

    private void InstantiateCoinsRandomly()
    {
        int numberOfCoins = Random.Range(3, 5);
        for (int i = 0; i < numberOfCoins; i++)
        {
            GameObject coin = Instantiate(coinPrefab, new Vector2(transform.position.x, transform.position.y + 0.5f), Quaternion.identity);
            Coin coinScript = coin.GetComponent<Coin>();
            coinScript.AddForce(true);
        }
    }

}
