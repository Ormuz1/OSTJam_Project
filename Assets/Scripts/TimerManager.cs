using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TimerManager : MonoBehaviour
{
    [SerializeField] private float startTime;
    [SerializeField] private TextMeshProUGUI timerText;
    [HideInInspector] public float timer;
    public float countdownMultiplier = 1f;

    private void OnValidate()
    {
        if(timerText != null)
            SetTimerText((int)startTime);
    }

    private void Awake() {
        timer = startTime;
    }

    private void SetTimerText(int time)
    {
        timerText.text = time.ToString();
    }

    void Update()
    {
        timer -= Time.deltaTime * countdownMultiplier;
        SetTimerText((int)timer);
    }
}
