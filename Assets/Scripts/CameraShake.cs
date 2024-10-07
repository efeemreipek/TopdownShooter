using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : Singleton<CameraShake>
{
    private Vector3 originalPos;
    private bool isShaking = false;
    private float shakeAmount;

    public void Shake(float duration, float amount)
    {
        if (!isShaking)
        {
            originalPos = transform.localPosition;
            shakeAmount = amount;
            StartCoroutine(ShakeCoroutine(duration));
        }
    }

    private IEnumerator ShakeCoroutine(float duration)
    {
        isShaking = true;
        float endTime = Time.unscaledTime + duration;

        while (Time.unscaledTime < endTime)
        {
            yield return null;
        }

        isShaking = false;
    }

    public Vector3 GetShakeOffset()
    {
        if (isShaking) return Random.insideUnitSphere * shakeAmount;

        return Vector3.zero;
    }

    public bool IsShaking() => isShaking;

}
