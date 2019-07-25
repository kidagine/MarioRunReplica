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
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private GameObject playerCam;
    [SerializeField] private CinemachineVirtualCamera cinemachineEndStageCin;
    [SerializeField] private Animator playerUIAnimator;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator bubbleTextAnimator;
    [SerializeField] private AnimationCurve bowserAnimationCurve;
    [SerializeField] private Image pauseImage;
    [SerializeField] private Image bubbleImage;
    [SerializeField] private Text coinsText;
    [SerializeField] private Text coinsPauseText;
    [SerializeField] private Text bubblesText;

    public static bool isBubbled;
    public static bool isScrollingOn;
    public static bool isPausered;
    public static bool hasWon;

    private List<GameObject> listLastRepeatedObjects = new List<GameObject>();
    private bool isStartingCutsceneFinished;
    private bool hasCircleMaskReachedMaxScale;
    private bool areUIButtonsDisabled;
    private bool wasStarMusicPlaying;
    private float cooldownStart = 2.0f;
    private float cooldownPlayerRun = 3.0f;
    private float repeatableOffset = 104.33f;
    private int coinsAmount;
    private int bubblesAmount = 2;

    
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
                FindObjectOfType<AudioManager>().Play("FirstStageBGM");
                playerUI.SetActive(true);
                cmvIngameCam.SetActive(false);
                isStartingCutsceneFinished = true;
                isScrollingOn = true;
                Destroy(blackPanel);
                Destroy(introStageText);
                yield return null;
            }
        }
    }

    public float GetOffset()
    {
        return repeatableOffset;
    }

    public void IncremenentOffset()
    {
        repeatableOffset += 104.33f;
    }

    public List<GameObject> GetLastRepeatedLevel()
    {
        return listLastRepeatedObjects;
    }

    public void SetLastRepeatedLevel(GameObject lastRepeatedObject)
    {
        listLastRepeatedObjects.Add(lastRepeatedObject);
    }

    public int GetBubblesAmount()
    {
        return bubblesAmount;
    }

    public void DecrementBubbles(int amount)
    {
        bubbleTextAnimator.SetBool("IsRed", false);
        bubblesAmount -= amount;
        bubblesText.text = bubblesAmount.ToString();
        if (bubblesAmount == 0)
        {
            bubbleImage.color = new Color(0.7f, 0.7f, 0.7f, 1.0f);
            bubblesText.color = new Color(0.7f, 0.7f, 0.7f, 1.0f);
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

    public void SwitchScene()
    {
        isScrollingOn = false;
        isPausered = false;
        hasWon = false; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StartScaleDownCircle()
    {
        playerUIAnimator.SetTrigger("FadeOut");
        whitePanel.SetActive(true);
        whiteCircleMask.SetActive(true);
        Vector3 centeredPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));
        whiteCircleMask.transform.position = new Vector2(centeredPosition.x, centeredPosition.y);
        whitePanel.transform.position = new Vector2(centeredPosition.x, centeredPosition.y); StartCoroutine(ScaleDownCircle());
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
        Vector3 centeredPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));
        bowserMask.transform.position = new Vector2(centeredPosition.x, centeredPosition.y);
        bowserPanel.transform.position = new Vector2(centeredPosition.x, centeredPosition.y);
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
                ratio += 0.5f * Time.fixedDeltaTime;
                yield return null;
            }
            else
            {
                hasBowserMaskReachedMaxScale = true;
                yield return null;
            }
        }
    }

    public void DisableUIButtons()
    {
        areUIButtonsDisabled = true;
        pauseImage.color = new Color(0.7f, 0.7f, 0.7f, 1.0f);
        bubbleImage.color = new Color(0.7f, 0.7f, 0.7f, 1.0f);
        bubblesText.color = new Color(0.7f, 0.7f, 0.7f, 1.0f);
    }

    public void PlayerWon()
    {
        StartCoroutine(Won());
    }

    IEnumerator Won()
    {
        FindObjectOfType<AudioManager>().Play("Win");
        FindObjectOfType<AudioManager>().Pause("FirstStageBGM");
        FindObjectOfType<AudioManager>().Pause("Jump");
        yield return new WaitForSeconds(0.3f);
        playerCam.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        playerAnimator.SetTrigger("WonBowser");
        yield return new WaitForSeconds(3.0f);
        playerUIAnimator.SetTrigger("FadeOutScene");
        yield return new WaitForSeconds(1.5f);
        SwitchScene();
    }

    public void Pause()
    {
        if (!areUIButtonsDisabled)
        {
            if (FindObjectOfType<AudioManager>().IsPlaying("Star"))
            {
                FindObjectOfType<AudioManager>().Pause("Star");
                wasStarMusicPlaying = true;
            }
            else
            {
                FindObjectOfType<AudioManager>().Pause("FirstStageBGM");
                wasStarMusicPlaying = false;
            }
            FindObjectOfType<AudioManager>().Pause("Jump");
            FindObjectOfType<AudioManager>().Play("Pause");
            coinsPauseText.text = coinsText.text;
            Time.timeScale = 0.0f;
            runUI.SetActive(false);
            pause.SetActive(true);
        }
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
        if (wasStarMusicPlaying)
        {
            FindObjectOfType<AudioManager>().UnPause("Star");
        }
        else
        {
            FindObjectOfType<AudioManager>().UnPause("FirstStageBGM");
        }
        wasStarMusicPlaying = false;
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

    public void CreateBubble()
    {
        if (!isBubbled && !areUIButtonsDisabled)
        {
            if (bubblesAmount > 0)
            {
                isPausered = false;
                bubbleTextAnimator.SetBool("IsRed", true);
                Instantiate(bubblePrefab, player.transform.position, Quaternion.identity);
            }
        }
    }

}
