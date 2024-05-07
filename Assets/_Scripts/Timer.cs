using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Timer : MonoBehaviour
{
    public Image fillImage;

    public float totalTime;

    private float currentTime;
    private bool startTimer;
    private Transform camTransform;

    public void Init()
    {
        fillImage.fillAmount = 1f;
        currentTime = totalTime;

        camTransform = Camera.main.transform;

        StopTimer();
        HideTimer();
    }

    private void Update()
    {
        fillImage.transform.parent.forward = camTransform.forward;

        if (startTimer)
        {
            currentTime -= Time.deltaTime;

            fillImage.fillAmount = Remap(currentTime, 0, totalTime, 0f, 1f);
        }
    }

    public bool IsTimeAvailable()
    {
        if (currentTime > 0)
            return true;
        else
            return false;
    }

    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public void StartTimer()
    {
        startTimer = true;
    }

    public void StopTimer()
    {
        startTimer = false;
    }

    public void ShowTimer()
    {
        fillImage.transform.parent.gameObject.SetActive(true);
    }

    public void HideTimer()
    {
        fillImage.transform.parent.gameObject.SetActive(false);
    }
}
