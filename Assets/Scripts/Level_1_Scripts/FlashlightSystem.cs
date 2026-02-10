using UnityEngine;
using UnityEngine.UI;

public class FlashlightSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.F;
    [SerializeField] private float maxBattery = 100f;
    [SerializeField] private float drainRate = 2f;

    [Header("Flicker Settings")]
    [Range(0, 100)]
    [SerializeField] private float flickerThreshold = 50f;
    [Tooltip("Extra sanity drain per second when light is flickering")]
    [SerializeField] private float anxietyDrainRate = 5f;

    [Header("References")]
    [SerializeField] private GameObject lightObject;
    [SerializeField] private Image batteryFillImage;
    [SerializeField] private LockerMechanic sanitySystem; // Drag your Locker/Sanity script here

    private float currentBattery;
    private bool isOn = true;

    void Start()
    {
        currentBattery = maxBattery;
        UpdateUI();

        // Auto-find sanity system if not assigned
        if (sanitySystem == null)
            sanitySystem = GetComponent<LockerMechanic>();
    }

    void Update()
    {
        // Toggle light
        if (Input.GetKeyDown(toggleKey) && currentBattery > 0)
        {
            isOn = !isOn;
            lightObject.SetActive(isOn);
        }

        // --- CASE 1: LIGHT IS ON ---
        if (isOn && currentBattery > 0)
        {
            currentBattery -= drainRate * Time.deltaTime;
            UpdateUI();

            float batteryPercent = (currentBattery / maxBattery) * 100f;

            if (batteryPercent <= flickerThreshold)
            {
                HandleDynamicFlicker(batteryPercent);
                ApplyAnxiety(); // Drain sanity because of flickering
            }
            else
            {
                if (!lightObject.activeSelf) lightObject.SetActive(true);
            }

            // Auto-shutoff
            if (currentBattery <= 0)
            {
                currentBattery = 0;
                isOn = false;
                lightObject.SetActive(false);
            }
        }
        // --- CASE 2: LIGHT IS OFF (Darkness Anxiety) ---
        else
        {
            // If the player turned it off OR battery died
            ApplyAnxiety();
        }
    }

    private void HandleDynamicFlicker(float percent)
    {
        // Math: As percent goes to 0, the threshold gets lower, making 'Random.value > threshold' happen more often.
        // At 50% battery, threshold is 0.98 (rare flicker)
        // At 5% battery, threshold is 0.85 (chaotic flicker)
        float threshold = Mathf.Lerp(0.80f, 0.99f, percent / flickerThreshold);

        if (Random.value > threshold)
        {
            lightObject.SetActive(!lightObject.activeSelf);
        }
    }

    private void ApplyAnxiety()
    {
        if (sanitySystem != null)
        {
            float finalDrain = anxietyDrainRate;

            // Optional: Make darkness even scarier than flickering?
            if (!isOn)
            {
                finalDrain = anxietyDrainRate * 1.5f; // 50% faster drain in total darkness
            }

            sanitySystem.ApplyExternalSanityDrain(finalDrain * Time.deltaTime);
        }
    }

    private void UpdateUI()
    {
        if (batteryFillImage != null)
            batteryFillImage.fillAmount = currentBattery / maxBattery;
    }

    public void Recharge()
    {
        currentBattery = maxBattery;
        UpdateUI();
        isOn = true;
        lightObject.SetActive(true);
    }
}