using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  
 *  Attach this script to the gameobject you want to set the timer. Subscribe to the OnTimerEnd event.
 *  In the gameobject create a reference for this script and call StartTimer() function.
 *  When timer end this event will be triggered.
 */ 

public class Timer : MonoBehaviour
{
    public static Action OnTimerEnd;

    private float timer = 0f;
    private float timeCountdown;
    private bool isTimerRunning = false;
    private bool isTimerPaused = false;
    private bool isLooping = false;

    private void Update()
    {
        if (isTimerRunning)
        {
            timer += Time.deltaTime;

            if (timer >= timeCountdown)
            {
                TimerEnd();
            }
        }
    }
    public void StartTimer(float time, bool loop = false)
    {
        timeCountdown = time;
        isTimerRunning = true;
        isLooping = loop;
    }
    public void PauseTimer()
    {
        isTimerPaused = true;
        isTimerRunning = false;
    }
    public void ResumeTimer()
    {
        if (isTimerPaused)
        {
            isTimerPaused = false;
            isTimerRunning = true;
        }
    }
    public void StopTimer()
    {
        TimerEnd();
    }
    private void TimerEnd()
    {
        OnTimerEnd?.Invoke();
        if(isLooping)
        {
            timer = 0f;
        }
        else
        {
            timer = 0f;
            isTimerRunning = false;
        }
    }
    public float GetRemainingTimeNormalized() => (timeCountdown - timer) / timeCountdown;
    public float GetRemainingTimeNormalizedInverse() => 1 - (timeCountdown - timer) / timeCountdown;
    public float GetRemainingTimeAsInt() => (int)Math.Ceiling(timeCountdown - timer);
    public float GetRemainingTimeAsFloat() => timeCountdown - timer;
    public bool GetIsTimerRunning() => isTimerRunning;
}