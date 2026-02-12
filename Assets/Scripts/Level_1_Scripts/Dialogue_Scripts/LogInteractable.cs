using UnityEngine;
using System.Collections;

public class LogInteractable : MonoBehaviour
{
    [Header("Animation Frames")]
    [Tooltip("Drag your 4 sprites here in order")]
    public Sprite[] idleFrames;
    public float framesPerSecond = 8f;

    [Header("UI Reference")]
    public GameObject asylumLogUI;
    public float fadeSpeed = 3f;

    private SpriteRenderer spriteRenderer;
    private CanvasGroup canvasGroup;
    private bool playerInRange = false;
    private int currentFrame;
    private float timer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Setup the UI for fading
        if (asylumLogUI != null)
        {
            canvasGroup = asylumLogUI.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = asylumLogUI.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0;
            asylumLogUI.SetActive(false);
        }
    }

    void Update()
    {
        // 1. THE CODE-BASED IDLE ANIMATION
        if (idleFrames != null && idleFrames.Length > 0)
        {
            timer += Time.deltaTime;
            if (timer >= 1f / framesPerSecond)
            {
                timer = 0;
                currentFrame = (currentFrame + 1) % idleFrames.Length;
                spriteRenderer.sprite = idleFrames[currentFrame];
            }
        }

        // 2. THE INTERACTION LOGIC
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleLog();
        }
    }

    void ToggleLog()
    {
        StopAllCoroutines();
        if (!asylumLogUI.activeSelf) StartCoroutine(FadeUI(1f));
        else StartCoroutine(FadeUI(0f));
    }

    IEnumerator FadeUI(float targetAlpha)
    {
        if (targetAlpha > 0) asylumLogUI.SetActive(true);

        while (!Mathf.Approximately(canvasGroup.alpha, targetAlpha))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        if (targetAlpha <= 0) asylumLogUI.SetActive(false);
    }

    // Trigger Zones
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            // Automatically close log if player walks away
            if (asylumLogUI.activeSelf)
            {
                StopAllCoroutines();
                StartCoroutine(FadeUI(0f));
            }
        }
    }
}