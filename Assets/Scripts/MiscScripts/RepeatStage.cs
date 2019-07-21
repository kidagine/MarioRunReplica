using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatStage : MonoBehaviour
{

    [SerializeField] private bool isRepeatDestroy;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private GameObject grid;
    [SerializeField] private GameObject coins;
    [SerializeField] private GameObject midground;
    [SerializeField] private List<GameObject> listInstantiateRepeatableObjects = new List<GameObject>();

    private GameManager gameManager;
    private GameObject firstElement;
    private GameObject secondElement;
    private GameObject thirdElement;
    private Vector2 tileMapPosition;
    private Vector2 coinPosition;
    private Vector2 repeatablePosition;
    private float colliderXOffset;
    private bool hasReachedLastIndex;


    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!isRepeatDestroy && !hasReachedLastIndex)
            {
                for (int i = 0; i < listInstantiateRepeatableObjects.Capacity; i++)
                {
                    if (listInstantiateRepeatableObjects[i] == listInstantiateRepeatableObjects[0])
                    {
                        tileMapPosition = new Vector2(listInstantiateRepeatableObjects[0].transform.position.x + gameManager.GetOffset(), listInstantiateRepeatableObjects[0].transform.position.y);
                        firstElement = listInstantiateRepeatableObjects[i];
                    }
                    else if (listInstantiateRepeatableObjects[i] == listInstantiateRepeatableObjects[1])
                    {
                        coinPosition = new Vector2(listInstantiateRepeatableObjects[1].transform.position.x + gameManager.GetOffset(), listInstantiateRepeatableObjects[1].transform.position.y);
                        secondElement = listInstantiateRepeatableObjects[i];
                    }
                    else if (listInstantiateRepeatableObjects[i] == listInstantiateRepeatableObjects[2])
                    {
                        repeatablePosition = new Vector2(gameManager.GetOffset(), listInstantiateRepeatableObjects[2].transform.position.y);
                        thirdElement = listInstantiateRepeatableObjects[i];
                        hasReachedLastIndex = true;
                    }
                }
                gameManager.IncremenentOffset();
                StartCoroutine(InstantiateLevel());
            }
            else if (isRepeatDestroy)
            {
                Debug.Log("1");
                List<GameObject> listLastRepeatedObjects = gameManager.GetLastRepeatedLevel();
                for (int i = 0; i < listLastRepeatedObjects.Capacity; i++)
                {
                    if (listLastRepeatedObjects[i].gameObject.name.Contains("Clone"))
                    {
                        Debug.Log("2");
                        Destroy(listLastRepeatedObjects[i]);
                    }
                }
            }
        }
    }

    IEnumerator InstantiateLevel()
    {
        GameObject tileMap = Instantiate(firstElement, tileMapPosition, Quaternion.identity);
        gameManager.SetLastRepeatedLevel(firstElement);
        tileMap.transform.parent = grid.transform;
        yield return new WaitForSeconds(0.5f);
        GameObject coins = Instantiate(secondElement, coinPosition, Quaternion.identity);
        gameManager.SetLastRepeatedLevel(secondElement);
        coins.transform.parent = coins.transform;
        yield return new WaitForSeconds(0.5f);
        GameObject repeatable = Instantiate(thirdElement, repeatablePosition, Quaternion.identity);
        gameManager.SetLastRepeatedLevel(thirdElement);
        repeatable.transform.parent = midground.transform;
        colliderXOffset += 320;
        boxCollider.offset = new Vector2(colliderXOffset, boxCollider.offset.y);
        yield return null;
    }

}
