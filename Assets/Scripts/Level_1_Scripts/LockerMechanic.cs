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

    [Header("Sanity System")]
    [SerializeField] private float maxSanity = 100f;
    [SerializeField] private float sanityDrainRate = 15f;

    [Header("UI References")]
    [SerializeField] private Image sanityFillImage;
    [SerializeField] private GameObject hidingIndicatorUI;

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
    [SerializeField] private GameObject hallucinationEnemy; // Drag Hallucination GameObject here
    [SerializeField] private float hallucinationThreshold = 30f; // Sanity level to trigger appearance

    private bool isPlayerNearby = false;
    private bool isHiding = false;
    private float currentSanity;
    private int currentFrame;

    private GameObject playerObject;
    private MonoBehaviour playerScript;
    private SpriteRenderer playerSprite;
    private Rigidbody2D playerRb;

    private void Start()
    {
        currentSanity = maxSanity;
        UpdateSanityUI();

        if (hidingIndicatorUI != null) hidingIndicatorUI.SetActive(false);
        if (glitchImageDisplay != null) glitchImageDisplay.gameObject.SetActive(false);

        if (lockerRenderer != null && lockerClosedSprite != null)
            lockerRenderer.sprite = lockerClosedSprite;

        // Ensure Hallucination starts OFF
        if (hallucinationEnemy != null) hallucinationEnemy.SetActive(false);
    }

    private void Update()
    {
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

            // NEW: Hallucination Trigger Logic
            if (hallucinationEnemy != null)
            {
                // Appears if sanity is equal to or below the threshold (e.g., 30)
                bool shouldShow = currentSanity <= hallucinationThreshold;
                if (hallucinationEnemy.activeSelf != shouldShow)
                {
                    hallucinationEnemy.SetActive(shouldShow);
                }
            }

            if (sanityPercentage <= 0.5f)
            {
                sanityFillImage.color = warningColor;
            }
            else
            {
                sanityFillImage.color = healthyColor;
            }

            if (eyeIconDisplay != null)
            {
                if (sanityPercentage <= 0.3f)
                {
                    eyeIconDisplay.sprite = redEyeSprite;
                }
                else
                {
                    eyeIconDisplay.sprite = whiteEyeSprite;
                }
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

    private IEnumerator EnterLockerRoutine()
    {
        if (lockerRenderer != null) lockerRenderer.sprite = lockerOpenSprite;
        yield return new WaitForSeconds(0.2f);

        isHiding = true;

        if (hidingIndicatorUI != null) hidingIndicatorUI.SetActive(true);

        if (glitchImageDisplay != null)
        {
            glitchImageDisplay.gameObject.SetActive(true);
            currentFrame = 0;
            StartCoroutine(PlayGlitchAnimation());
        }

        SetPlayerVisibility(false);

        if (lockerRenderer != null) lockerRenderer.sprite = lockerClosedSprite;
    }

    private IEnumerator ExitLockerRoutine()
    {
        if (lockerRenderer != null) lockerRenderer.sprite = lockerOpenSprite;

        if (hidingIndicatorUI != null) hidingIndicatorUI.SetActive(false);

        if (glitchImageDisplay != null)
        {
            glitchImageDisplay.gameObject.SetActive(false);
        }

        SetPlayerVisibility(true);
        isHiding = false;

        yield return new WaitForSeconds(0.2f);

        if (lockerRenderer != null) lockerRenderer.sprite = lockerClosedSprite;
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
            // Assuming your player script is named PlayerController
            playerScript = playerObject.GetComponent<MonoBehaviour>();
            playerSprite = playerObject.GetComponent<SpriteRenderer>();
            playerRb = playerObject.GetComponent<Rigidbody2D>();
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
        if (currentSanity < 0) currentSanity = 0;
        UpdateSanityUI();
    }
}