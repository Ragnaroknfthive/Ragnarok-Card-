using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class Intensity : MonoBehaviour
{
    private Light2D lightObject;
    private float minIntensity = 0.5f;
    private float maxIntensity = 1f;
    private float cycleDuration = 2f; // Duration of one complete cycle in seconds

    private float currentIntensity;
    private float timer;

    void Start()
    {
        lightObject = GetComponent<Light2D>();
        if (lightObject == null)
        {
            Debug.LogError("Light2D component not found on the same GameObject as LightIntensityCycle script.");
            enabled = false; // Disable the script if Light2D component is not found
        }

        currentIntensity = minIntensity;
        timer = 0f;
    }

    void Update()
    {
        // Increment the timer
        timer += Time.deltaTime;

        // Calculate the new intensity based on time
        float t = Mathf.PingPong(timer / cycleDuration, 1f); // PingPong function to cycle between 0 and 1
        currentIntensity = Mathf.Lerp(minIntensity, maxIntensity, t);

        // Apply the intensity to the Light2D object
        lightObject.intensity = currentIntensity;
    }
}