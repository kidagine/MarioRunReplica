using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flagpole : MonoBehaviour
{

    [SerializeField] private Transform flagTop;
    [SerializeField] private Transform flagBottom;
    [SerializeField] private GameObject[] coin1UpList;
    

    private Vector2 normalizedPosition;
    private bool isTriggered;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!isTriggered)
            {
                Vector2 flagTopNormalized = flagTop.transform.position.normalized;
                Vector2 playerNormalized = other.transform.position.normalized;
                int roundedPos = Mathf.RoundToInt(flagTopNormalized.y - playerNormalized.y);
                Debug.Log(roundedPos);
                Instantiate(coin1UpList[0], new Vector2(other.transform.position.x + 0.3f, other.transform.position.y + 0.1f), Quaternion.identity);
                isTriggered = true;
            }
        }
    }

}