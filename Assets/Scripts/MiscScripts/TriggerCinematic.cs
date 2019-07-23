using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCinematic : MonoBehaviour
{

    [SerializeField] private GameObject bossCamera;
    [SerializeField] private GameObject IngameCamera;
    [SerializeField] private GameObject bossIngameCamera;
    [HideInInspector] public bool isCameraSwitchOver;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.isScrollingOn = false;
            StartCoroutine(SwitchCamerasForTime(IngameCamera, bossCamera, 2.0f));
        }
    }

    IEnumerator SwitchCamerasForTime(GameObject currentCamera, GameObject toSwitchCamera, float time)
    {
        IngameCamera.SetActive(false);
        toSwitchCamera.SetActive(true);
        yield return new WaitForSeconds(time);
        toSwitchCamera.SetActive(false);
        GameManager.isScrollingOn = true;
        yield return new WaitForSeconds(0.05f);
        isCameraSwitchOver = true;
        yield return null;
    }

}
