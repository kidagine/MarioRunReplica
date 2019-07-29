using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenPart : MonoBehaviour
{

    [SerializeField] private Sprite[] brokenPartsSpriteList;

    private SpriteRenderer spriteRenderer;
    private bool isSameSprite;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(SwapSprites());
    }

    IEnumerator SwapSprites()
    {
        while (true)
        {
            for (int i = 0; i < brokenPartsSpriteList.Length; i++)
            {
                if (spriteRenderer.sprite == brokenPartsSpriteList[i])
                isSameSprite = true;
                if (isSameSprite)
                {
                    spriteRenderer.sprite = brokenPartsSpriteList[i];
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }

}
