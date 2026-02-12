using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class NarrativeEngine : MonoBehaviour
{
    public static NarrativeEngine Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image portraitDisplay;
    [SerializeField] private GameObject continuePrompt; // Add a "Press Space" text object here

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private AudioSource voiceSource;

    private Coroutine typingCoroutine;
    private bool isDialogueActive = false;
    private bool isTyping = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (continuePrompt != null) continuePrompt.SetActive(false);
    }

    void Update()
    {
        // Listen for Space key only when dialogue is active and NOT typing
        if (isDialogueActive && !isTyping && Input.GetKeyDown(KeyCode.Space))
        {
            HideDialogue();
        }
    }

    public void PlayLine(string text, AudioClip clip, Sprite portrait)
    {
        isDialogueActive = true;
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (continuePrompt != null) continuePrompt.SetActive(false);

        if (portraitDisplay != null)
        {
            portraitDisplay.sprite = portrait;
            portraitDisplay.gameObject.SetActive(portrait != null);
        }

        if (voiceSource != null)
        {
            voiceSource.Stop();
            if (clip != null)
            {
                voiceSource.ignoreListenerPause = true;
                voiceSource.clip = clip;
                voiceSource.Play();
            }
        }

        if (dialogueText != null)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(text));
        }
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        isTyping = false;

        // Show the "Press Space" prompt once typing is done
        if (continuePrompt != null) continuePrompt.SetActive(true);
    }

    public void HideDialogue()
    {
        isDialogueActive = false;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (continuePrompt != null) continuePrompt.SetActive(false);
        if (voiceSource != null) voiceSource.Stop();

        // Resume the game world
        Time.timeScale = 1f;
    }
}