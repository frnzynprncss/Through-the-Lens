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
    [Tooltip("Sanity regained per second when light is stable and ON")]
    [SerializeField] private float sanityRegenRate = 3f; // NEW: Regain logic

    [Header("References")]
    [SerializeField] private GameObject lightObject;
    [SerializeField] private Image batteryFillImage;
    [SerializeField] private LockerMechanic sanitySystem;

    private float currentBattery;
    private bool isOn = true;
    private bool isForcedHidden = false;

    void Start()
    {
        currentBattery = maxBattery;
        UpdateUI();

        if (sanitySystem == null)
            sanitySystem = GetComponent<LockerMechanic>();
    }

    void Update()
    {
        if (!isForcedHidden && Input.GetKeyDown(toggleKey) && currentBattery > 0)
        {
            isOn = !isOn;
            lightObject.SetActive(isOn);
        }

        if (isOn && currentBattery > 0)
        {
            if (!isForcedHidden)
            {
                currentBattery -= drainRate * Time.deltaTime;
                UpdateUI();

                float batteryPercent = (currentBattery / maxBattery) * 100f;

                if (batteryPercent <= flickerThreshold)
                {
                    HandleDynamicFlicker(batteryPercent);
                    ApplyAnxiety(true); // Is flickering
                }
                else
                {
                    if (!lightObject.activeSelf) lightObject.SetActive(true);
                    ApplyAnxiety(false); // Stable ON (Regen)
                }
            }

            if (currentBattery <= 0)
            {
                currentBattery = 0;
                isOn = false;
                lightObject.SetActive(false);
            }
        }
        else
        {
            ApplyAnxiety(false); // Light is OFF (Darkness drain)
        }
    }

    public void SetFlashlightHidden(bool hide)
    {
        isForcedHidden = hide;
        if (hide) lightObject.SetActive(false);
        else lightObject.SetActive(isOn);
    }

    private void HandleDynamicFlicker(float percent)
    {
        float threshold = Mathf.Lerp(0.80f, 0.99f, percent / flickerThreshold);
        if (Random.value > threshold)
        {
            lightObject.SetActive(!lightObject.activeSelf);
        }
    }

    private void ApplyAnxiety(bool isFlickering)
    {
        if (sanitySystem == null) return;

        float finalEffect = 0;

        if (isOn && !isFlickering)
        {
            // CASE 1: Light is stable and ON -> REGAIN SANITY
            // We pass a negative value to "drain" to act as a regain
            finalEffect = -sanityRegenRate;
        }
        else if (isOn && isFlickering)
        {
            // CASE 2: Light is flickering -> NORMAL DRAIN
            finalEffect = anxietyDrainRate;
        }
        else
        {
            // CASE 3: Light is OFF -> HEAVY DRAIN
            finalEffect = anxietyDrainRate * 1.5f;
        }

        sanitySystem.ApplyExternalSanityDrain(finalEffect * Time.deltaTime);
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
        if (!isForcedHidden) lightObject.SetActive(true);
    }

    public void AddBattery(float amount)
    {
        currentBattery += amount;

        // Ensure we don't go over the maximum battery limit
        if (currentBattery > maxBattery)
            currentBattery = maxBattery;

        UpdateUI();

        // If the light was dead (off), this turns it back on automatically
        if (currentBattery > 0 && !isOn)
        {
            isOn = true;
            if (!isForcedHidden) lightObject.SetActive(true);
        }
    }
}