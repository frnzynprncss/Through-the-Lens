using UnityEngine;
using System.Collections;

public class FlickeringLight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light lightSource; // Drag your 2D/3D Light here
    [SerializeField] private SpriteRenderer bulbRenderer; // Optional: Drag the light bulb sprite

    [Header("Settings")]
    [SerializeField] private float minWaitTime = 0.1f;
    [SerializeField] private float maxWaitTime = 0.5f;

    [Tooltip("Probability that the light is ON (0.0 to 1.0)")]
    [Range(0f, 1f)]
    [SerializeField] private float flickerIntensity = 0.5f;

    [Header("Broken Light Mode")]
    [SerializeField] private bool isBroken = false;
    [SerializeField] private float brokenTurnOffDelay = 2.0f;

    private void Start()
    {
        if (lightSource == null) lightSource = GetComponent<Light>();

        StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // If the light is "Broken", it flickers wildly then stays off longer
            if (isBroken && Random.value > 0.8f)
            {
                SetLightState(false);
                yield return new WaitForSeconds(brokenTurnOffDelay);
            }

            // Randomly toggle state based on intensity
            bool newState = Random.value < flickerIntensity;
            SetLightState(newState);

            // Wait for a random amount of time before next flicker
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        }
    }

    private void SetLightState(bool state)
    {
        if (lightSource != null) lightSource.enabled = state;

        // Dim the sprite color if we have a bulbRenderer
        if (bulbRenderer != null)
        {
            bulbRenderer.color = state ? Color.white : Color.gray;
        }
    }
}