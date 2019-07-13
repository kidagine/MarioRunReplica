using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin1Up : MonoBehaviour
{

    private readonly float speed = 0.03f;
    private float coinTimer = 0.5f;
    private bool isFlagCoin;

    void Update()
    {
        if (coinTimer >= 0.3f)
        {
            transform.Translate(Vector2.up * speed);
        }
        else if (coinTimer >= 0.0f)
        {
            transform.Translate(Vector2.up * 0.0f);
        }
        else
        {
            if (!isFlagCoin)
            {
                Destroy(gameObject);
            }
        }
        coinTimer -= Time.deltaTime;
    }

    public void SetFlagCoin(bool value)
    {
        isFlagCoin = value;
    }

}
