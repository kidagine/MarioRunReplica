using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockTimer : MonoBehaviour
{

    [SerializeField] Animator animator;
    [SerializeField] Text timerText;

    private int timer = 60;


    void Start()
    {
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
            if (!GameManager.isPausered && !GameManager.hasWon)
            {
                timer--;
                timerText.text = timer.ToString();
                yield return new WaitForSeconds(1);
            }
            else
            {
                yield return null;
            }
        }
    }

}
