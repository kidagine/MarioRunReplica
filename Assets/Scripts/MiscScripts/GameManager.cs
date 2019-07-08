using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Animator cameraAnimator;
    [SerializeField] private GameObject circleMask;
    [SerializeField] private GameObject blackPanel;
    [SerializeField] private GameObject introStageText;
    [SerializeField] private Text coinsText;

    public static bool isScrollingOn;

    private bool isStartingCutsceneFinished;
    private float cooldownStart = 0.5f;
    private float cooldownPlayerRun = 0.7f;
    private int coinsAmount;


    void Update()
    {
        if (!isStartingCutsceneFinished)
        {
            if (cooldownStart <= 0)
            {
                StartCoroutine(ScaleCircle());
                if (cooldownPlayerRun <= 0)
                {
                    isScrollingOn = true;
                    isStartingCutsceneFinished = true;
                }
                cooldownPlayerRun -= Time.deltaTime;
            }
            cooldownStart -= Time.deltaTime;
        }
    }

    IEnumerator ScaleCircle()
    {
        bool hasReachedMaxScale = false;
        float ratio = 0.0f;
        Vector2 startingScale = new Vector2(0.0f, 0.0f);
        Vector2 targetScale = new Vector2(50.0f, 50.0f);
        while (!hasReachedMaxScale)
        {
            if (ratio <= 1.0f)
            {
                circleMask.transform.localScale = Vector2.Lerp(startingScale, targetScale, ratio);
                ratio += 0.5f * Time.deltaTime;
                yield return null;
            }
            else
            {
                hasReachedMaxScale = true;
                cameraAnimator.SetTrigger("ZoomOut");
                Destroy(blackPanel);
                Destroy(introStageText);
                yield return null;
            }
        }
    }

    public void increaseCoins(int amount)
    {
        coinsAmount += amount;
        coinsText.text = coinsAmount.ToString();
    }


}
