using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance = null;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    private float slowDownFactor;
    private float slowDownLength;

    public void DoSlowMotion(float slowDownFactorRef, float slowDownDurationRef)
    {
        CameraFollow.instance.ZoomInFunc();

        slowDownFactor = slowDownFactorRef;
        slowDownLength = slowDownDurationRef;

        Time.timeScale = slowDownFactor;
        Time.fixedDeltaTime = Time.deltaTime * .02f;
        StartCoroutine(ResetToNoramlRoutine());
    }

    IEnumerator ResetToNoramlRoutine()
    {
        float counter = 0f;

        while (counter < slowDownLength)
        {
            counter += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = .02f;
        CameraFollow.instance.ZoomOutFunc();
    }

    public void ResetTimeScale()
    {
        StopAllCoroutines();
        Time.timeScale = 1f;
        Time.fixedDeltaTime = .02f;
        CameraFollow.instance.ZoomOutFunc();
    }
}
