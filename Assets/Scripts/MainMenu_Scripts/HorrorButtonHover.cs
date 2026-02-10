using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Required for detecting mouse hover

public class HorrorButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Visual Settings")]
    [Tooltip("The color the text turns when hovered.")]
    [SerializeField] private Color hoverColor = new Color(0.7f, 0f, 0f, 1f); // Dark Red default
    [Tooltip("How much larger the button gets (1.0 is normal size).")]
    [SerializeField] private float hoverScale = 1.1f;

    [Header("Horror Jitter")]
    [Tooltip("Enable to make the button shake nervously when hovered.")]
    [SerializeField] private bool enableJitter = true;
    [Tooltip("How violently the button shakes.")]
    [SerializeField] private float jitterIntensity = 2.0f;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverSound;

    // Internal variables to remember original state
    private Color originalColor;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Text textComponent;
    private bool isHovering = false;

    private void Awake()
    {
        // Cache the text component so we can change its color
        textComponent = GetComponentInChildren<Text>();

        // Save the starting values
        if (textComponent != null) originalColor = textComponent.color;
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
    }

    // Triggered when mouse enters the button area
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;

        // Change Color
        if (textComponent != null) textComponent.color = hoverColor;

        // Change Scale
        transform.localScale = originalScale * hoverScale;

        // Play Sound
        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    // Triggered when mouse leaves the button area
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;

        // Reset everything to normal
        if (textComponent != null) textComponent.color = originalColor;
        transform.localScale = originalScale;
        transform.localPosition = originalPosition; // Snap back to exact center
    }

    private void Update()
    {
        // This creates the vibrating "horror" effect
        if (isHovering && enableJitter)
        {
            float x = Random.Range(-jitterIntensity, jitterIntensity);
            float y = Random.Range(-jitterIntensity, jitterIntensity);

            // Apply the shake relative to the original position
            transform.localPosition = originalPosition + new Vector3(x, y, 0);
        }
    }
}