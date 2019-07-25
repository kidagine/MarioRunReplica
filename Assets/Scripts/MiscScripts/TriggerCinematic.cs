using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCinematic : MonoBehaviour
{

    [SerializeField] private Animator bowserAnimator;
    [SerializeField] private GameObject bossCamera;
    [SerializeField] private GameObject IngameCamera;
    [SerializeField] private GameObject bossIngameCamera;
    [HideInInspector] public bool isCameraSwitchOver;

    private int collisions;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            collisions++;
            if (collisions == 1)
            {
                GameManager.isScrollingOn = false;
                StartCoroutine(SwitchCamerasForTime(IngameCamera, bossCamera, 3.0f));
            }
        }
    }

    IEnumerator SwitchCamerasForTime(GameObject currentCamera, GameObject toSwitchCamera, float time)
    {
        IngameCamera.SetActive(false);
        toSwitchCamera.SetActive(true);
        bowserAnimator.SetBool("IsScreaming", true);
        yield return new WaitForSeconds(time);
        bowserAnimator.SetBool("IsScreaming", false);
        toSwitchCamera.SetActive(false);
        GameManager.isScrollingOn = true;
        isCameraSwitchOver = true;
        Destroy(gameObject, 0.5f);
        yield return null;
    }

}
