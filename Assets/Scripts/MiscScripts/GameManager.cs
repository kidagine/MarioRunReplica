using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject restartMenu;
    [SerializeField] private GameObject quitMenu;
    [SerializeField] private GameObject runUI;
    [SerializeField] private GameObject circleMask;
    [SerializeField] private GameObject blackPanel;
    [SerializeField] private GameObject introStageText;
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject cmvIngameCam;
    [SerializeField] private GameObject introStageCin;
    [SerializeField] private Text coinsText;

    public static bool isScrollingOn;
    public static bool isPausered;

    private bool isStartingCutsceneFinished;
    private bool hasCircleMaskReachedMaxScale;
    private float cooldownStart = 2.0f;
    private float cooldownPlayerRun = 3.0f;
    private int coinsAmount;

    
    void Awake()
    {
        introStageCin.SetActive(true);
    }

    void Update()
    {
        if (!isStartingCutsceneFinished)
        {
            if (cooldownStart <= 0)
            {
                StartCoroutine(ScaleCircle());
                if (hasCircleMaskReachedMaxScale)
                {
                    FindObjectOfType<AudioManager>().Play("FirstStageBGM");
                    playerUI.SetActive(true);
                    isScrollingOn = true;
                    isStartingCutsceneFinished = true;
                    cmvIngameCam.SetActive(false);
                }
                cooldownPlayerRun -= Time.deltaTime;
            }
            cooldownStart -= Time.deltaTime;
        }
    }

    IEnumerator ScaleCircle()
    {
        float ratio = 0.0f;
        Vector2 startingScale = new Vector2(0.0f, 0.0f);
        Vector2 targetScale = new Vector2(50.0f, 50.0f);
        while (!hasCircleMaskReachedMaxScale)
        {
            if (ratio <= 1.0f)
            {
                circleMask.transform.localScale = Vector2.Lerp(startingScale, targetScale, ratio);
                ratio += 1.5f * Time.deltaTime;
                yield return null;
            }
            else
            {
                hasCircleMaskReachedMaxScale = true;
                Destroy(blackPanel);
                Destroy(introStageText);
                yield return null;
            }
        }
    }

    public void IncrementCoins(int amount)
    {
        coinsAmount += amount;
        coinsText.text = coinsAmount.ToString();
    }

    public void Pause()
    {
        FindObjectOfType<AudioManager>().Pause("FirstStageBGM");
        FindObjectOfType<AudioManager>().Play("Pause");
        Time.timeScale = 0.0f;
        runUI.SetActive(false);
        pause.SetActive(true);
    }

    public void Resume()
    {
        StartCoroutine(ResumeTimer());
    }

    IEnumerator ResumeTimer()
    {
        FindObjectOfType<AudioManager>().Play("Pause");
        pause.SetActive(false);
        runUI.SetActive(true);
        yield return new WaitForSecondsRealtime(0.55f);
        FindObjectOfType<AudioManager>().UnPause("FirstStageBGM");
        Time.timeScale = 1.0f;
    }

    public void Restart()
    {
        FindObjectOfType<AudioManager>().Play("Click");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        FindObjectOfType<AudioManager>().Play("Click");
        Application.Quit();
    }

    public void ShowPauseMenu()
    {
        FindObjectOfType<AudioManager>().Play("Click");
        restartMenu.SetActive(false);
        quitMenu.SetActive(false);
    }

    public void ShowRetryMenu()
    {
        FindObjectOfType<AudioManager>().Play("Click");
        restartMenu.SetActive(true);
    }

    public void ShowQuitMenu()
    {
        FindObjectOfType<AudioManager>().Play("Click");
        quitMenu.SetActive(true);
    }

}
