using UnityEngine;
using TMPro; // Make sure you have TextMeshPro installed
using System.Collections;

public class NarrativeEngine : MonoBehaviour
{
    public static NarrativeEngine Instance;

    [Header("UI Components")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float autoCloseDelay = 2.5f;

    private Coroutine typingCoroutine;

    private void Awake()
    {
        // Simple Singleton to access this from other scripts
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    public void PlayLine(string text)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialoguePanel.SetActive(true);
        typingCoroutine = StartCoroutine(TypeText(text));
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(autoCloseDelay);
        dialoguePanel.SetActive(false);
    }

    // Add this inside the NarrativeEngine class
    public void HideDialogue()
    {
        // Stops the typewriter effect so it doesn't keep running in the background
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        // Hides the UI immediately
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }
}