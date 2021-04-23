using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class TimerManager : SingletonBase<TimerManager>
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image radial;
    public float countdownSpeedMultiplier = 1f;
    private float timer;
    private float startTime;
    [HideInInspector] public bool isCountingDown;


    public IEnumerator RestartTimer(float timeToRestart, float newTimerValue)
    {
        isCountingDown = false;
        float timerStartValue = timer;
        for(float loopTimer = 0; loopTimer < timeToRestart; loopTimer += Time.deltaTime)
        {
            timer = Mathf.Lerp(timerStartValue, newTimerValue, loopTimer / timeToRestart);
            radial.fillAmount = timer / newTimerValue;
            SetTimerText();
            yield return null;
        }
        StartTimer(newTimerValue);
    }


    public void StartTimer(float newTimerValue)
    {
        startTime = newTimerValue;
        timer = newTimerValue;
        isCountingDown = true;
    }


    private void SetTimerText()
    {
        timerText.text = ((int)timer + 1).ToString();
    }


    private void Update()
    {
        if(isCountingDown)
        {
            timer -= Time.deltaTime * countdownSpeedMultiplier;
            radial.fillAmount = timer / startTime;
            SetTimerText();
        }
    }
}
