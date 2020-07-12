using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public TMP_Text text;

    private float startTime;
    private float freezeStartTime;
    private float freezeDuration;

    private bool _play;
    public bool Play
    {
        get => _play;
        set
        {
            _play = value;
        }
    }

    public float TimeElapsed
    => Time.time - startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (Play)
        {
            float time = Time.time - startTime;
            float roundedTime = Mathf.Round(time * 10) / 10;
            text.text = "" + roundedTime;
            if (roundedTime % 1 == 0)
            {
                text.text += ".0";
            }
        }
        if (freezeStartTime > 0)
        {
            if (Time.unscaledTime > freezeStartTime + freezeDuration)
            {
                Time.timeScale = 1;
                freezeStartTime = 0;
            }
        }
    }

    public void startTimer()
    {
        startTime = Time.time;
        Play = true;
    }

    public void resetTimer()
    {
        startTime = 0;
        Play = false;
        text.text = "0.0";
    }

    public void freezeTime(float duration)
    {
        freezeStartTime = Time.unscaledTime;
        freezeDuration = duration;
        Time.timeScale = 0;
    }
}
