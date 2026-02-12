using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class SmoothTransition : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private string nextLevelName = "Level2";

    [Header("UI References")]
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;

    [Header("Dialogue Settings")]
    [TextArea(3, 10)]
    public string transitionDialogue = "This place looks even worse inside...";
    public float textSpeed = 0.05f;
    public float waitBeforeLoad = 2f;

    [Header("Audio")]
    // NEW: Drag your .wav file directly into this slot in the Inspector
    [SerializeField] private AudioClip transitionSound;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(SequenceRoutine());
        }
    }

    private IEnumerator SequenceRoutine()
    {
        // 1. Play the audio clip directly via SoundManager
        if (transitionSound != null && SoundManager.instance != null)
        {
            SoundManager.instance.PlaySFX(transitionSound);
        }

        // 2. Show Dialogue Box
        dialogueBox.SetActive(true);
        dialogueText.text = "";

        // 3. Typewriter Effect
        foreach (char c in transitionDialogue.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        // 4. Wait for player to process the move
        yield return new WaitForSeconds(waitBeforeLoad);

        // 5. Hide Dialogue
        dialogueBox.SetActive(false);

        // 6. Load the Next Level
        SceneManager.LoadScene(nextLevelName);
    }
}