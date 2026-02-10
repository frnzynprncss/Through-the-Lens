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
    public string soundName = "LevelExit_Creak"; // Name in your AudioManager

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
        // 1. Play exit sound (e.g., a heavy door creaking)
        if (AudioManager.instance != null && !string.IsNullOrEmpty(soundName))
        {
            AudioManager.instance.Play(soundName);
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

        // 5. Hide Dialogue (Optional, as the scene will change)
        dialogueBox.SetActive(false);

        // 6. Load the Next Level
        SceneManager.LoadScene(nextLevelName);
    }
}