using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurpleCoin : MonoBehaviour
{

    [SerializeField] private GameObject purpleCoinUI;
    [SerializeField] private GameObject purpleCoinPause;
    [SerializeField] private Sprite purpleCoinUISprite;

    private Image purpleCoinUIImage;
    private Image purpleCoinPauseImage;

    void Start()
    {
        purpleCoinUIImage = purpleCoinUI.GetComponent<Image>();
        purpleCoinPauseImage = purpleCoinPause.GetComponent<Image>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            FindObjectOfType<AudioManager>().Play("CoinPickUp");
            purpleCoinUIImage.sprite = purpleCoinUISprite;
            purpleCoinPauseImage.sprite = purpleCoinUISprite;
            Destroy(gameObject);
        }
    }

}
