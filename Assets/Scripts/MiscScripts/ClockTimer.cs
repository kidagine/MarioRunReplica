﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockTimer : MonoBehaviour
{

    [SerializeField] Animator animator;
    [SerializeField] Animator playerAnimator;
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] Text timerText;
    [SerializeField] private int timer;

    private int defaultTimer;


    void Start()
    {
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
