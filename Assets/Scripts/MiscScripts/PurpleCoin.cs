using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurpleCoin : MonoBehaviour
{

    [SerializeField] private GameObject purpleCoinUI;
    [SerializeField] private Sprite purpleCoinUISprite;

    private Image purpleCoinUIImage;

    void Start()
    {
        purpleCoinUIImage = purpleCoinUI.GetComponent<Image>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            FindObjectOfType<AudioManager>().Play("CoinPickUp");
            purpleCoinUIImage.sprite = purpleCoinUISprite;
            Destroy(gameObject);
        }
    }

}
