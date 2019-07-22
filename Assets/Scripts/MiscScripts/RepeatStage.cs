using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatStage : MonoBehaviour
{

    [SerializeField] private bool isRepeatDestroy;
    [SerializeField] private GameObject grid;
    [SerializeField] private GameObject coins;
    [SerializeField] private GameObject midground;
    [SerializeField] private List<GameObject> listInstantiateRepeatableObjects = new List<GameObject>();

    private GameManager gameManager;
    private GameObject firstElement;
    private GameObject secondElement;
    private GameObject thirdElement;
    private GameObject fourthElement;
    private GameObject fifthElement;
    private GameObject sixthElement;
    private Vector2 tileMapPosition;
    private Vector2 coinPosition;
    private Vector2 repeatablePosition;
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
                        coinPosition = new Vector2(listInstantiateRepeatableObjects[1].transform.position.x + gameManager.GetOffset() - 0.5872f, 0.0f - 0.08513826f);
                        secondElement = listInstantiateRepeatableObjects[i];
                    }
                    else if (listInstantiateRepeatableObjects[i] == listInstantiateRepeatableObjects[2])
                    {
                        repeatablePosition = new Vector2(gameManager.GetOffset(), listInstantiateRepeatableObjects[2].transform.position.y);
                        thirdElement = listInstantiateRepeatableObjects[i];
                    }
                    else if (listInstantiateRepeatableObjects[i] == listInstantiateRepeatableObjects[3])
                    {
                        fourthElement = listInstantiateRepeatableObjects[i];
                    }
                    else if (listInstantiateRepeatableObjects[i] == listInstantiateRepeatableObjects[4])
                    {
                        fifthElement = listInstantiateRepeatableObjects[i];
                    }
                    else if (listInstantiateRepeatableObjects[i] == listInstantiateRepeatableObjects[5])
                    {
                        sixthElement = listInstantiateRepeatableObjects[5];
                        hasReachedLastIndex = true;
                    }
                }
                gameManager.IncremenentOffset();
                StartCoroutine(InstantiateLevel());
            }
        }
    }

    IEnumerator InstantiateLevel()
    {
        GameObject tileMap = Instantiate(firstElement, tileMapPosition, Quaternion.identity);
        tileMap.transform.parent = grid.transform;
        yield return new WaitForSeconds(0.2f);
        GameObject coins = Instantiate(secondElement, coinPosition, Quaternion.identity);
        coins.transform.parent = coins.transform;
        yield return new WaitForSeconds(0.2f);
        GameObject repeatable = Instantiate(thirdElement, repeatablePosition, Quaternion.identity);
        repeatable.transform.parent = midground.transform;
        yield return new WaitForSeconds(0.2f);
        GameObject breakablePlatforms = Instantiate(fourthElement, coinPosition, Quaternion.identity);
        breakablePlatforms.transform.parent = midground.transform;
        yield return new WaitForSeconds(0.2f);
        GameObject firebars = Instantiate(fifthElement, coinPosition, Quaternion.identity);
        firebars.transform.parent = midground.transform;
        yield return new WaitForSeconds(0.2f);
        GameObject mushrooms = Instantiate(sixthElement, coinPosition, Quaternion.identity);
        mushrooms.transform.parent = midground.transform;
        yield return null;
    }

}
