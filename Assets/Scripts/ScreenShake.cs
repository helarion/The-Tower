using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    // Desired duration of the shake effect
    private float shakeDuration = 0f;

    // A measure of magnitude for the shake. Tweak based on your preference
    private float shakeMagnitude = 0;
    [SerializeField] float lowShakeMagnitude = 0.4f;
    [SerializeField] float mediumShakeMagnitude = 0.7f;
    [SerializeField] float highShakeMagnitude = 1.2f;

    // A measure of how quickly the shake effect should evaporate
    private float dampingSpeed = 1.0f;

    // The initial position of the GameObject
    Vector3 initialPosition;

    public enum ShakeIntensity
    {
        low,
        medium,
        high
    }

    void OnEnable()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = initialPosition;
        }
    }

    public void TriggerShake(float duration, ShakeIntensity magnitudeType)
    {
        shakeDuration = duration;
        switch(magnitudeType)
        {
            case ShakeIntensity.low:
                shakeMagnitude = lowShakeMagnitude;
                break;
            case ShakeIntensity.medium:
                shakeMagnitude = mediumShakeMagnitude;
                break;
            case ShakeIntensity.high:
                shakeMagnitude = highShakeMagnitude;
                break;
        }
    }
}
