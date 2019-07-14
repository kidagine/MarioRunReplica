using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flagpole : MonoBehaviour
{

    [SerializeField] private Animator playerAnimator;
    [SerializeField] private GameObject smoke;
    [SerializeField] private GameObject flagTop;
    [SerializeField] private GameObject flagBottom;
    [SerializeField] private GameObject bowserFlag;
    [SerializeField] private GameObject bulbaFlag;
    [SerializeField] private GameObject[] coin1UpList;

    private GameObject player;
    private GameObject coin1Up;
    private Vector2 playerTouchFlagPosition;
    private bool isTriggered;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!isTriggered)
            {
                GameManager.hasWon = true;
                player = other.gameObject;
                playerTouchFlagPosition = other.transform.position;

                int playerFlagPosition = ConvertToZeroTenRange(other.transform.position);
                int coinIndex= GetCoinValueIndex(playerFlagPosition);
                coin1Up = Instantiate(coin1UpList[coinIndex], new Vector2(other.transform.position.x + 0.2f, other.transform.position.y + 0.1f), Quaternion.identity);
                coin1Up.GetComponent<Coin1Up>().SetFlagCoin(true);
                IncrementCoin(coinIndex);
                isTriggered = true;
            }
        }
    }

    private int ConvertToZeroTenRange(Vector3 value)
    {
        float normal = Mathf.InverseLerp(flagBottom.transform.position.y, flagTop.transform.position.y, value.y);
        int result = Mathf.RoundToInt(normal * 10);
        return result;
    }

    private int GetCoinValueIndex(int playerFlagPosition)
    {
        int coinIndex = 0;
        if (playerFlagPosition > 8 && playerFlagPosition <= 10)
        {
            coinIndex = 5;
        }
        else if (playerFlagPosition > 6 && playerFlagPosition <= 8)
        {
            coinIndex = 4;
        }
        else if (playerFlagPosition > 4 && playerFlagPosition <= 6)
        {
            coinIndex = 3;
        }
        else if (playerFlagPosition > 2 && playerFlagPosition <= 4)
        {
            coinIndex = 2;
        }
        else if (playerFlagPosition > 1 && playerFlagPosition <= 2)
        {
            coinIndex = 1;
        }
        else if (playerFlagPosition > 0 && playerFlagPosition <= 1)
        {
            coinIndex = 0;
        }
        return coinIndex;
    }

    private void IncrementCoin(int coinIndex)
    {
        int coinValue = 0;
        switch (coinIndex)
        {
            case 0:
                coinValue = 1;
                break;
            case 1:
                coinValue = 2;
                break;
            case 2:
                coinValue = 4;
                break;
            case 3:
                coinValue = 6;
                break;
            case 4:
                coinValue = 8;
                break;
            case 5:
                coinValue = 10;
                break;
        }
        FindObjectOfType<GameManager>().IncrementCoins(coinValue);
    }

    public void SwapFlags()
    {
        bulbaFlag.SetActive(true);
        Instantiate(smoke, bulbaFlag.transform.position, Quaternion.identity);
        Destroy(coin1Up);
        StartCoroutine(MoveFlags());
        StartCoroutine(PlayerSlideDown());
    }

    IEnumerator MoveFlags()
    {
        float ratio = 0.0f;
        float bulbaFlagTargetPosition = 0.0f;
        bool haveFlagsSwapped = false;
        Vector2 bowserFlagPosition = bowserFlag.transform.position;
        Vector2 bulbaFlagPosition = bulbaFlag.transform.position;
        while (!haveFlagsSwapped)
        {
            if (ratio <= 1.0f)
            {
                if (playerTouchFlagPosition.y < flagBottom.transform.position.y + 0.3f)
                {
                    bulbaFlagTargetPosition = flagBottom.transform.position.y + 0.3f;
                }
                else if (playerTouchFlagPosition.y > flagTop.transform.position.y)
                {
                    bulbaFlagTargetPosition = flagTop   .transform.position.y + 0.3f;
                }
                else
                {
                    bulbaFlagTargetPosition = playerTouchFlagPosition.y;
                }
                bulbaFlag.transform.position = Vector2.Lerp(bulbaFlagPosition, new Vector2(bulbaFlag.transform.position.x, bulbaFlagTargetPosition), ratio);
                bowserFlag.transform.position = Vector2.Lerp(bowserFlagPosition, bulbaFlagPosition, ratio);
                ratio += 0.6f * Time.fixedDeltaTime;
                yield return null;
            }
            else
            {
                haveFlagsSwapped = true;
                yield return null;
            }
        }
    }

    IEnumerator PlayerSlideDown()
    {
        float ratio = 0.0f;
        bool hasReachedBottom = false;
        Vector2 playerStartingPosition = player.transform.position;
        Vector2 playerTargetPosition = flagBottom.transform.position;
        while (!hasReachedBottom)
        {
            if (ratio <= 1.0f)
            {
                player.transform.position = Vector2.Lerp(playerStartingPosition, new Vector2(player.transform.position.x, flagBottom.transform.position.y + 0.3f), ratio);
                ratio += 0.65f * Time.fixedDeltaTime;
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(0.7f);
                Instantiate(smoke, bowserFlag.transform.position, Quaternion.identity);
                Destroy(bowserFlag);
                yield return new WaitForSeconds(0.5f);
                playerAnimator.SetTrigger("Won");
                hasReachedBottom = true;
                yield return null;
            }
        }
    }

}