using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LockerMechanic : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Locker Visuals")]
    [SerializeField] private SpriteRenderer lockerRenderer;
    [SerializeField] private Sprite lockerClosedSprite;
    [SerializeField] private Sprite lockerOpenSprite;
    [Tooltip("The 4 frames for the locker opening/closing animation")]
    [SerializeField] private Sprite[] lockerAnimFrames;
    [SerializeField] private float animFrameRate = 0.05f;

    [Header("Sanity System")]
    [SerializeField] private float maxSanity = 100f;
    [SerializeField] private float sanityDrainRate = 15f;

    [Header("UI References")]
    [SerializeField] private Image sanityFillImage;
    [SerializeField] private GameObject hidingIndicatorUI;
    [SerializeField] private GameObject interactionPromptUI;

    [Header("Glitch Animation (Coded)")]
    [SerializeField] private Image glitchImageDisplay;
    [SerializeField] private Sprite[] glitchFrames;
    [SerializeField] private float frameRate = 0.1f;

    [Header("Sanity Visuals")]
    [SerializeField] private Color healthyColor = Color.white;
    [SerializeField] private Color warningColor = new Color(0.5f, 0f, 0f);
    [SerializeField] private Image eyeIconDisplay;
    [SerializeField] private Sprite whiteEyeSprite;
    [SerializeField] private Sprite redEyeSprite;

    [Header("Hallucination Logic (NEW)")]
    [SerializeField] private GameObject hallucinationEnemy;
    [SerializeField] private float hallucinationThreshold = 30f;

    private bool isPlayerNearby = false;
    private bool isHiding = false;
    private float currentSanity;
    private int currentFrame;

    private GameObject playerObject;
    private MonoBehaviour playerScript;
    private SpriteRenderer playerSprite;
    private Rigidbody2D playerRb;
    private FlashlightSystem playerFlashlight;

    private void Start()
    {
        currentSanity = maxSanity;
        UpdateSanityUI();

        if (hidingIndicatorUI != null) hidingIndicatorUI.SetActive(false);
        if (glitchImageDisplay != null) glitchImageDisplay.gameObject.SetActive(false);
        if (interactionPromptUI != null) interactionPromptUI.SetActive(false);

        if (lockerRenderer != null && lockerClosedSprite != null)
            lockerRenderer.sprite = lockerClosedSprite;

        if (hallucinationEnemy != null) hallucinationEnemy.SetActive(false);
    }

    private void Update()
    {
        if (interactionPromptUI != null)
        {
            interactionPromptUI.SetActive(isPlayerNearby && !isHiding);
        }

        if (isPlayerNearby && playerObject != null && Input.GetKeyDown(interactKey))
        {
            if (isHiding) StartCoroutine(ExitLockerRoutine());
            else StartCoroutine(EnterLockerRoutine());
        }

        if (isHiding)
        {
            currentSanity -= sanityDrainRate * Time.deltaTime;
            UpdateSanityUI();

            if (currentSanity <= 0)
            {
                StartCoroutine(ExitLockerRoutine());
                currentSanity = 25f;
            }
        }
    }

    private void UpdateSanityUI()
    {
        if (sanityFillImage != null)
        {
            float sanityPercentage = currentSanity / maxSanity;
            sanityFillImage.fillAmount = sanityPercentage;

            if (hallucinationEnemy != null)
            {
                bool shouldShow = currentSanity <= hallucinationThreshold;
                if (hallucinationEnemy.activeSelf != shouldShow)
                {
                    hallucinationEnemy.SetActive(shouldShow);
                }
            }

            sanityFillImage.color = (sanityPercentage <= 0.5f) ? warningColor : healthyColor;

            if (eyeIconDisplay != null)
            {
                eyeIconDisplay.sprite = (sanityPercentage <= 0.3f) ? redEyeSprite : whiteEyeSprite;
            }
        }
    }

    private IEnumerator PlayGlitchAnimation()
    {
        while (isHiding)
        {
            if (glitchFrames.Length > 0 && glitchImageDisplay != null)
            {
                glitchImageDisplay.sprite = glitchFrames[currentFrame];
                currentFrame = (currentFrame + 1) % glitchFrames.Length;
            }
            yield return new WaitForSeconds(frameRate);
        }
    }

    // New logic to play the 4-frame locker animation
    private IEnumerator PlayLockerAnimation(bool opening)
    {
        if (lockerAnimFrames == null || lockerAnimFrames.Length == 0) yield break;

        if (opening)
        {
            for (int i = 0; i < lockerAnimFrames.Length; i++)
            {
                lockerRenderer.sprite = lockerAnimFrames[i];
                yield return new WaitForSeconds(animFrameRate);
            }
        }
        else
        {
            for (int i = lockerAnimFrames.Length - 1; i >= 0; i--)
            {
                lockerRenderer.sprite = lockerAnimFrames[i];
                yield return new WaitForSeconds(animFrameRate);
            }
        }
    }

    private IEnumerator EnterLockerRoutine()
    {
        // Play opening animation instead of just switching sprites
        yield return StartCoroutine(PlayLockerAnimation(true));

        yield return new WaitForSeconds(0.1f);

        isHiding = true;

        if (hidingIndicatorUI != null) hidingIndicatorUI.SetActive(true);
        if (glitchImageDisplay != null)
        {
            glitchImageDisplay.gameObject.SetActive(true);
            currentFrame = 0;
            StartCoroutine(PlayGlitchAnimation());
        }

        SetPlayerVisibility(false);
        if (playerFlashlight != null) playerFlashlight.SetFlashlightHidden(true);

        // Close it after entering
        yield return StartCoroutine(PlayLockerAnimation(false));
    }

    private IEnumerator ExitLockerRoutine()
    {
        // Play opening animation to let player out
        yield return StartCoroutine(PlayLockerAnimation(true));

        if (hidingIndicatorUI != null) hidingIndicatorUI.SetActive(false);
        if (glitchImageDisplay != null)
        {
            glitchImageDisplay.gameObject.SetActive(false);
        }

        SetPlayerVisibility(true);
        if (playerFlashlight != null) playerFlashlight.SetFlashlightHidden(false);

        isHiding = false;

        yield return new WaitForSeconds(0.1f);

        // Final Close
        yield return StartCoroutine(PlayLockerAnimation(false));
    }

    private void SetPlayerVisibility(bool isVisible)
    {
        if (playerObject == null) return;
        if (playerScript != null) playerScript.enabled = isVisible;
        if (playerSprite != null) playerSprite.enabled = isVisible;
        if (playerRb != null)
        {
            playerRb.velocity = Vector2.zero;
            playerRb.isKinematic = !isVisible;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            playerObject = collision.gameObject;
            playerScript = playerObject.GetComponent<MonoBehaviour>();
            playerSprite = playerObject.GetComponent<SpriteRenderer>();
            playerRb = playerObject.GetComponent<Rigidbody2D>();
            playerFlashlight = playerObject.GetComponentInChildren<FlashlightSystem>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    public void ApplyExternalSanityDrain(float amount)
    {
        currentSanity -= amount;
        currentSanity = Mathf.Clamp(currentSanity, 0, maxSanity);
        UpdateSanityUI();
    }
}