using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AsylumLogTrigger : MonoBehaviour
{
    [Header("UI Assignments")]
    [SerializeField] private CanvasGroup eyeCanvasGroup;
    [SerializeField] private GameObject interactPrompt;
    [SerializeField] private CanvasGroup logPanelGroup;
    [SerializeField] private Button closeButton;

    [Header("Content")]
    [SerializeField] private Sprite logSprite;
    [SerializeField] private Image logDisplayImage;
    [TextArea]
    [SerializeField] private string dialogueContent = "Day 1: The patients are getting restless...";
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Animation Settings")]
    [SerializeField] private float fadeSpeed = 5f;
    [SerializeField] private float scaleSpeed = 10f;
    [SerializeField] private float typeSpeed = 0.05f;

    private bool isPlayerNearby = false;
    private bool isLogOpen = false;
    private PlayerController playerScript;
    private Coroutine typewriterCoroutine;

    // Stores the exact scales you set in the Inspector
    private Vector3 originalEyeScale;
    private Vector3 originalLogScale;

    private void Awake()
    {
        // Capture your custom Inspector scales
        if (eyeCanvasGroup != null) originalEyeScale = eyeCanvasGroup.transform.localScale;
        if (logPanelGroup != null) originalLogScale = logPanelGroup.transform.localScale;
    }

    private void Start()
    {
        // Initial setup: Hide everything smoothly
        if (eyeCanvasGroup != null)
        {
            eyeCanvasGroup.alpha = 0;
            eyeCanvasGroup.transform.localScale = originalEyeScale * 0.1f;
            eyeCanvasGroup.gameObject.SetActive(false);
        }

        if (interactPrompt != null) interactPrompt.SetActive(false);

        if (logPanelGroup != null)
        {
            logPanelGroup.alpha = 0;
            logPanelGroup.transform.localScale = originalLogScale;
            logPanelGroup.gameObject.SetActive(false);
        }

        if (closeButton != null) closeButton.onClick.AddListener(OnCloseClicked);
    }

    private void Update()
    {
        // Toggle Log with E key
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            ShowLog(!isLogOpen);
        }
    }

    private void OnCloseClicked() { ShowLog(false); }

    public void ShowLog(bool isOpen)
    {
        isLogOpen = isOpen;

        // Safety: Don't start animations if the object is being disabled
        if (!gameObject.activeInHierarchy) return;

        StopAllCoroutines();

        if (isOpen)
        {
            if (logDisplayImage != null && logSprite != null) logDisplayImage.sprite = logSprite;

            StartCoroutine(FadeUI(logPanelGroup, true, originalLogScale));
            StartCoroutine(FadeAndScaleUI(eyeCanvasGroup, false, originalEyeScale));

            if (interactPrompt != null) interactPrompt.SetActive(false);
            typewriterCoroutine = StartCoroutine(TypeText(dialogueContent));
        }
        else
        {
            StartCoroutine(FadeUI(logPanelGroup, false, originalLogScale));
            if (isPlayerNearby) StartCoroutine(FadeAndScaleUI(eyeCanvasGroup, true, originalEyeScale));
        }

        // --- FREEZE EVERYTHING ---
        if (playerScript != null)
        {
            // 1. Freeze Movement
            playerScript.enabled = !isOpen;

            // 2. Freeze Sanity (LockerMechanic)
            LockerMechanic locker = playerScript.GetComponentInChildren<LockerMechanic>();
            if (locker != null) locker.enabled = !isOpen;

            // 3. Freeze Flashlight/Battery Drain (Universal Search)
            MonoBehaviour[] allScripts = playerScript.GetComponentsInChildren<MonoBehaviour>();
            foreach (MonoBehaviour s in allScripts)
            {
                string scriptName = s.GetType().Name.ToLower();
                if (scriptName.Contains("flashlight") || scriptName.Contains("battery"))
                {
                    s.enabled = !isOpen;
                }
            }

            // Stop sliding physics
            Rigidbody2D rb = playerScript.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;
        }
    }

    private IEnumerator TypeText(string textToType)
    {
        if (dialogueText == null) yield break;
        dialogueText.text = "";
        yield return new WaitForSeconds(0.2f);

        foreach (char letter in textToType.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    private IEnumerator FadeUI(CanvasGroup cg, bool show, Vector3 targetScale)
    {
        if (cg == null) yield break;
        if (show) cg.gameObject.SetActive(true);

        float targetAlpha = show ? 1 : 0;
        cg.transform.localScale = targetScale;

        while (Mathf.Abs(cg.alpha - targetAlpha) > 0.01f)
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
            yield return null;
        }

        cg.alpha = targetAlpha;
        if (!show) cg.gameObject.SetActive(false);
    }

    private IEnumerator FadeAndScaleUI(CanvasGroup cg, bool show, Vector3 defaultScale)
    {
        if (cg == null) yield break;
        if (show) cg.gameObject.SetActive(true);

        float targetAlpha = show ? 1 : 0;
        Vector3 targetScale = show ? defaultScale : defaultScale * 0.1f;

        while (Mathf.Abs(cg.alpha - targetAlpha) > 0.01f)
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
            cg.transform.localScale = Vector3.Lerp(cg.transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
            yield return null;
        }

        if (!show) cg.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            playerScript = collision.GetComponent<PlayerController>();

            if (!isLogOpen && gameObject.activeInHierarchy)
            {
                StartCoroutine(FadeAndScaleUI(eyeCanvasGroup, true, originalEyeScale));
                if (interactPrompt != null) interactPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;

            // Safe exit check to prevent Coroutine errors
            if (gameObject.activeInHierarchy)
            {
                ShowLog(false);
            }
            else
            {
                // Immediate snap-close if object is being disabled
                if (logPanelGroup != null) logPanelGroup.gameObject.SetActive(false);
                if (eyeCanvasGroup != null) eyeCanvasGroup.gameObject.SetActive(false);
                if (interactPrompt != null) interactPrompt.SetActive(false);
            }
        }
    }
}