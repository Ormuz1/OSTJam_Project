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
    private AudioReverbFilter reverbFilter;
    [HideInInspector] public float countdownSpeedMultiplier = 1f;
    private float timer;
    private float startTime;
    [HideInInspector] public bool isCountingDown;
    private int lastSecond;
    public override void Awake() 
    {
        base.Awake();
        sfxPlayer = GetComponent<SoundEffectPlayer>();
        reverbFilter = GetComponent<AudioReverbFilter>();
        reverbFilter.reverbLevel = -2000;
    }


    public IEnumerator RestartTimer(float timeToRestart, float newTimerValue)
    {
        sfxPlayer.StopAllCoroutines();
        isCountingDown = false;
        float timerStartValue = timer;
        countdownSpeedMultiplier = 1f;
        for(float loopTimer = 0; loopTimer < timeToRestart; loopTimer += Time.deltaTime)
        {
            timer = Mathf.Lerp(timerStartValue, newTimerValue, loopTimer / timeToRestart);
            radial.fillAmount = Mathf.Clamp01(timer / newTimerValue);
            SetTimerText();
            yield return null;
        }
        StartTimer(newTimerValue);
    }


    public void StartTimer(float newTimerValue)
    {
        startTime = newTimerValue;
        lastSecond = (int)startTime;
        timer = newTimerValue;
        isCountingDown = true;
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
            if((int) timer < lastSecond)
            {
                sfxPlayer.PlayRandom(clockTickingSoundEffects);
                lastSecond = (int)timer;
            }
            reverbFilter.reverbLevel = Mathf.Lerp(600, -2000, timer / startTime);
            timer -= Time.deltaTime * countdownSpeedMultiplier;
            radial.fillAmount = Mathf.Clamp01(timer / startTime);
            SetTimerText();
            if(timer <= 0)
            {
                GameManager.Instance.Lose();
            }
        }
    }
}
