using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [TextArea] public string lineToSay;
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            NarrativeEngine.Instance.PlayLine(lineToSay);
            hasTriggered = true;
        }
    }

    // NEW: This fixes the "piling up" by hiding the UI when the player leaves the zone
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            NarrativeEngine.Instance.HideDialogue();
        }
    }
}