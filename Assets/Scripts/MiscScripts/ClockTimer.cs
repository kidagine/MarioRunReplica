using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockTimer : MonoBehaviour
{

    [SerializeField] private Animator animator;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private GameObject timeOut;
    [SerializeField] private Text timerText;
    [SerializeField] private int timer;

    private Text timeOutText;
    private int defaultTimer;


    void Start()
    {
        timeOutText = timeOut.GetComponent<Text>();
        defaultTimer = timer;
        StartCoroutine(TimerCountdown());
    }

    void Update()
    {
        if (GameManager.isPausered)
        {
            animator.SetBool("FadeIn", false);
            animator.SetBool("FadeOut", true);
        }
        else
        {
            animator.SetBool("FadeOut", false);
            animator.SetBool("FadeIn", true);
        }
    }

    IEnumerator TimerCountdown()
    {
        while (timer > 0)
        {
            if (!GameManager.isPausered && !GameManager.hasWon && !GameManager.isBubbled)
            {
                timer--;
                timerText.text = timer.ToString();
                if (timer <= 5)
                {
                    FindObjectOfType<AudioManager>().Play("Kick");
                    timeOut.SetActive(true);
                    timeOutText.text = timer.ToString();
                    if (timer <= 0)
                    {
                        timeOutText.text = "TIME'S OUT";
                        Destroy(timeOut, 0.7f);
                    }
                }
                yield return new WaitForSeconds(1);
            }
            else
            {
                yield return null;
            }
        }
        FindObjectOfType<AudioManager>().Play("Death");
        FindObjectOfType<AudioManager>().Pause("FirstStageBGM");
        FindObjectOfType<AudioManager>().Pause("Jump");
        playerRb.constraints = RigidbodyConstraints2D.FreezeAll;
        playerAnimator.SetTrigger("Death");
        yield return null;
    }

    public void ResetTimer()
    {
        timer = defaultTimer;
    }

}
