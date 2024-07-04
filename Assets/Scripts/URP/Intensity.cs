//Code for set the intensity of the light to cycle between two values over time.
//The intensity of the light will cycle between the minimum and maximum intensity values over a specified duration.
//The intensity will be calculated using the PingPong function and then applied to the Light2D component.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class Intensity : MonoBehaviour
{
    private Light2D lightObject; // Reference to the Light2D component
    private float minIntensity = 0.5f; // Minimum intensity of the light
    private float maxIntensity = 1f; // Maximum intensity of the light
    private float cycleDuration = 2f; // Duration of one complete cycle in seconds

    private float currentIntensity; // Current intensity of the light
    private float timer; // Timer to keep track of time

    void Start()
    {
        // Get the Light2D component
        lightObject = GetComponent<Light2D>();
        if (lightObject == null)
        {
            // Log an error message if Light2D component is not found
            Debug.LogError("Light2D component not found on the same GameObject as LightIntensityCycle script.");
            enabled = false; // Disable the script if Light2D component is not found
        }

        // Initialize the intensity and timer
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