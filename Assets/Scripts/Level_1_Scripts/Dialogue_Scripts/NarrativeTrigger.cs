using UnityEngine;
using TMPro;
using System.Collections;

public class NarrativeTrigger : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;

    [Header("Dialogue Settings")]
    [TextArea(3, 10)]
    public string[] dialogueLines;
    public float textSpeed = 0.05f;
    public float delayBetweenLines = 2f;

    [Header("Audio Settings")]
    public string soundName; // Type the name exactly as it appears in AudioManager

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only trigger if it's the player and hasn't happened yet
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(PlaySequence());
        }
    }

    IEnumerator PlaySequence()
    {
        // 1. Play the sound via the Centralized AudioManager
        if (AudioManager.instance != null && !string.IsNullOrEmpty(soundName))
        {
            AudioManager.instance.Play(soundName);
        }
        else if (AudioManager.instance == null)
        {
            Debug.LogError("No AudioManager found in the scene!");
        }

        // 2. Show the dialogue box
        dialogueBox.SetActive(true);

        // 3. Cycle through each line of text
        foreach (string line in dialogueLines)
        {
            dialogueText.text = ""; // Clear existing text

            // Typewriter effect
            foreach (char c in line.ToCharArray())
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(textSpeed);
            }

            // Wait for the player to read the line
            yield return new WaitForSeconds(delayBetweenLines);
        }

        // 4. Hide the box when finished
        dialogueBox.SetActive(false);
    }
}