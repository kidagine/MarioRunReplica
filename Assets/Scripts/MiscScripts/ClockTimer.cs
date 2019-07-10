using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockTimer : MonoBehaviour
{

    [SerializeField] Text timerText;

    private int timer = 60;


    void Start()
    {
        StartCoroutine(TimerCountdown());
    }

    IEnumerator TimerCountdown()
    {
        while (timer > 0)
        {
            timer--;
            timerText.text = timer.ToString();
            yield return new WaitForSeconds(1);
        }
    }

}
