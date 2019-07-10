using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneHandler : MonoBehaviour
{

    [SerializeField] private GameObject fadeOutPanel;
    [SerializeField] private GameObject tapToBeginText;
    [SerializeField] private AudioSource tapSound;

    private bool isIntroAnimFinished;

    void Update()
    {
        if (isIntroAnimFinished)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Destroy(tapToBeginText);
                tapSound.Play();
                StartCoroutine(LoadNextScene());
            }
        }
    }

    public void FinishedIntroAnim()
    {
        isIntroAnimFinished = true;
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(0.5f);
        fadeOutPanel.SetActive(true);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
