using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class FlashlightFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    [Tooltip("Minimum time the light stays fully ON.")]
    [SerializeField] private float minStableTime = 2.0f;

    [Tooltip("Maximum time the light stays fully ON.")]
    [SerializeField] private float maxStableTime = 5.0f;

    [Tooltip("How dim the text gets when it flickers 'off' (0 is invisible, 1 is visible).")]
    [Range(0f, 1f)]
    [SerializeField] private float dimAlpha = 0.1f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        // Get the CanvasGroup component (we will add this in the editor)
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        if (canvasGroup != null)
        {
            StartCoroutine(FlickerRoutine());
        }
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // 1. STABLE STATE: Keep text visible for a random duration
            canvasGroup.alpha = 1f;
            float waitTime = Random.Range(minStableTime, maxStableTime);
            yield return new WaitForSeconds(waitTime);

            // 2. FLICKER BURST: Rapidly toggle on and off
            int flickerCount = Random.Range(3, 8); // How many times it blinks in this burst

            for (int i = 0; i < flickerCount; i++)
            {
                // Dim the light
                canvasGroup.alpha = dimAlpha;
                yield return new WaitForSeconds(Random.Range(0.05f, 0.1f)); // Very short delay

                // Turn light back on
                canvasGroup.alpha = 1f;
                yield return new WaitForSeconds(Random.Range(0.05f, 0.2f)); // Short delay
            }
        }
    }
}