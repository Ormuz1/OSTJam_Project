using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class TimerManager : SingletonBase<TimerManager>
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI timerMultiplierText;
    [SerializeField] private Image radial;
    [SerializeField] private SoundEffect[] clockTickingSoundEffects;
    
    private SoundEffectPlayer sfxPlayer;
    [HideInInspector] public float countdownSpeedMultiplier = 1f;
    private float timer;
    private float startTime;
    [HideInInspector] public bool isCountingDown;


    public override void Awake() 
    {
        base.Awake();
        sfxPlayer = GetComponent<SoundEffectPlayer>();    
    }


    public IEnumerator RestartTimer(float timeToRestart, float newTimerValue)
    {
        sfxPlayer.StopCoroutine("PlayRepeatedly");
        isCountingDown = false;
        float timerStartValue = timer;
        countdownSpeedMultiplier = 1f;
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
        sfxPlayer.StartCoroutine(sfxPlayer.PlayRepeatedly(clockTickingSoundEffects, 1f));
    }


    private void SetTimerText()
    {
        timerText.text = ((int)timer + 1).ToString();
        timerMultiplierText.text = "x " + countdownSpeedMultiplier.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
    }


    private void Update()
    {
        if(isCountingDown)
        {
            timer -= Time.deltaTime * countdownSpeedMultiplier;
            radial.fillAmount = timer / startTime;
            SetTimerText();
            if(timer <= 0)
            {
                GameManager.Instance.Lose();
            }
        }
    }
}
