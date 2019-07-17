using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject restartMenu;
    [SerializeField] private GameObject quitMenu;
    [SerializeField] private GameObject runUI;
    [SerializeField] private GameObject circleMask;
    [SerializeField] private GameObject blackPanel;
    [SerializeField] private GameObject whitePanel;
    [SerializeField] private GameObject whiteCircleMask;
    [SerializeField] private GameObject bowserPanel;
    [SerializeField] private GameObject bowserMask;
    [SerializeField] private GameObject introStageText;
    [SerializeField] private GameObject endStageText;
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject cmvIngameCam;
    [SerializeField] private GameObject introStageCin;
    [SerializeField] private GameObject endStageCin;
    [SerializeField] private CinemachineVirtualCamera cinemachineEndStageCin;
    [SerializeField] private Animator playerUIAnimator;
    [SerializeField] private AnimationCurve bowserAnimationCurve;
    [SerializeField] private Text coinsText;
    [SerializeField] private Text coinsPauseText;

    public static bool isScrollingOn;
    public static bool isPausered;
    public static bool hasWon;

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
                StartCoroutine(ScaleUpCircle());
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

    IEnumerator ScaleUpCircle()
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

    public int GetCoinsAmount()
    {
        return coinsAmount;
    }

    public void DecrementCoins(int amount)
    {
        coinsAmount -= amount;
        coinsText.text = coinsAmount.ToString();
    }

    public void IncrementCoins(int amount)
    {
        coinsAmount += amount;
        coinsText.text = coinsAmount.ToString();
    }

    public void EndStageCamera()
    {
        endStageCin.SetActive(true);
    }

    public void CourseCompleted()
    {
        cinemachineEndStageCin.GetCinemachineComponent<CinemachineFramingTransposer>().m_DeadZoneWidth = 1.0f;
        endStageText.SetActive(true);
    }

    public void StartScaleDownCircle()
    {
        playerUIAnimator.SetTrigger("FadeOut");
        whitePanel.SetActive(true);
        whiteCircleMask.SetActive(true);
        StartCoroutine(ScaleDownCircle());
    }

    IEnumerator ScaleDownCircle()
    {
        float ratio = 0.0f;
        Vector2 startingScale = new Vector2(50.0f, 50.0f);
        Vector2 targetScale = new Vector2(0.0f, 0.0f);
        bool hasWhiteCircleMaskReachedMaxScale = false;
        while (!hasWhiteCircleMaskReachedMaxScale)
        {
            if (ratio <= 1.0f)
            {
                whiteCircleMask.transform.localScale = Vector2.Lerp(startingScale, targetScale, ratio);
                ratio += 0.5f * Time.fixedDeltaTime;
                yield return null;
            }
            else
            {
                hasWhiteCircleMaskReachedMaxScale = true;
                yield return null;
            }
        }
    }

    public void GameOver()
    {
        playerUIAnimator.SetTrigger("FadeOut");
        bowserMask.SetActive(true);
        bowserPanel.SetActive(true);
        bowserMask.transform.position = new Vector2(player.transform.position.x + 1, 1.7f);
        bowserPanel.transform.position = new Vector2(player.transform.position.x + 1, 1.7f);
        gameOverUI.SetActive(true);
        StartCoroutine(ScaleUpBowserEmblem());
    }

    IEnumerator ScaleUpBowserEmblem()
    {
        bool hasBowserMaskReachedMaxScale = false;
        float ratio = 0.0f;
        float curveAmount = bowserAnimationCurve.Evaluate(ratio);
        Vector2 startingScale = new Vector2(3.0f, 3.0f);
        Vector2 targetScale = new Vector2(0.0f, 0.0f);
        while (!hasBowserMaskReachedMaxScale)
        {
            if (curveAmount <= 1.0f)
            {
                bowserMask.transform.localScale = Vector2.Lerp(startingScale, targetScale, curveAmount);
                curveAmount = bowserAnimationCurve.Evaluate(ratio);
                ratio += 0.3f * Time.fixedDeltaTime;
                yield return null;
            }
            else
            {
                hasBowserMaskReachedMaxScale = true;
                yield return null;
            }
        }
    }

    public void Pause()
    {
        FindObjectOfType<AudioManager>().Pause("FirstStageBGM");
        FindObjectOfType<AudioManager>().Pause("Jump");
        FindObjectOfType<AudioManager>().Play("Pause");
        coinsPauseText.text = coinsText.text;   
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
        Time.timeScale = 1.0f;
        isScrollingOn = false;
        isPausered = false;
        hasWon = false;
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
