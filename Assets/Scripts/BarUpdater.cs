using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarUpdater : MonoBehaviour
{
    public Image bar;
    public Image barUnder;
    public SweatSpawner sweatSpawner;
    public float maxExpectedIncrease = 2;
    public float minScaleFactor = 0.1f;
    public float maxScaleFactor = 2;
    public float minShakeDegrees = 1;
    public float maxShakeDegrees = 5;
    public float minDuration = 0.2f;
    public float maxDuration = 0.5f;

    private float scaleFactor;
    private float shakeDegrees;
    private float duration = 0;
    private float shakeStartTime = -1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time <= shakeStartTime + duration)
        {
            transform.localScale = Vector3.one * scaleFactor;
            transform.eulerAngles = new Vector3(
                0,
                0,
                Random.Range(shakeDegrees * -1, shakeDegrees)
                );
        }
        else if (shakeStartTime > 0)
        {
            shakeStartTime = 0;
            transform.localScale = Vector3.one;
            transform.eulerAngles = Vector3.zero;
            bar.fillAmount = barUnder.fillAmount;
        }
    }

    public void setValue(float value, float maxValue)
    {
        float oldValue = barUnder.fillAmount;
        float newValue = value / maxValue;
        barUnder.fillAmount = newValue;
        //Make effects
        if (newValue > oldValue)
        {
            shakeStartTime = Time.time;
            float diffFactor = (newValue - oldValue) / maxExpectedIncrease;
            duration = (maxDuration - minDuration) * diffFactor + minDuration;
            scaleFactor = (maxScaleFactor - minScaleFactor) * diffFactor + minScaleFactor;
            shakeDegrees = (maxShakeDegrees - minShakeDegrees) * diffFactor + minShakeDegrees;
        }
        else if (shakeStartTime <= 0)
        {
            bar.fillAmount = newValue;
        }
        if (newValue >= 1)
        {
            bar.fillAmount = newValue;
        }
        sweatSpawner.Play = newValue >= 0.8f;
    }
}
