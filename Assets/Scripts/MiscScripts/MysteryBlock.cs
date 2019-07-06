using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBlock : MonoBehaviour
{

    [SerializeField] Animator animator;
    [SerializeField] Sprite emptyBlockSprite;
    [SerializeField] private bool isDestructible;

    private Transform itemInside;
    private Vector2 startingPosition;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        itemInside = transform.GetChild(0);
        spriteRenderer = GetComponent<SpriteRenderer>();
        startingPosition = new Vector2(transform.position.x, transform.position.y);
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D point in other.contacts)
            {
                if (point.normal.y >=    0.9f)
                {
                    if (isDestructible)
                    {
                        Destroy(gameObject);
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
                                FindObjectOfType<GameManager>().increaseCoins(1);
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
        if (ratio <= 1.0f)
        {
            transform.position = Vector2.Lerp(startingPosition, targetPosition, ratio);
            ratio += 10.0f * Time.deltaTime;
            yield return null;
        }
        else
        {
            hasReachedTop = true;
            itemInside.gameObject.SetActive(true);
            StartCoroutine(ShakeDown());
        }
    }

    IEnumerator ShakeDown()
    {
        bool hasReachedBottom = false;
        float ratio = 0.0f;
        Vector2 targetPosition = new Vector2(startingPosition.x, startingPosition.y + 0.2f);
        while (!hasReachedBottom)
            if (ratio <= 1.0f)
            {
                transform.position = Vector2.Lerp(targetPosition, startingPosition, ratio);
                ratio += 10.0f * Time.deltaTime;
                yield return null;
            }
            else
            {
                hasReachedBottom = true;
                animator.SetBool("IsSpinning", false);
                spriteRenderer.sprite = emptyBlockSprite;
                yield return null;
            }
    }

}
