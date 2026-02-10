using UnityEngine;
using System.Collections;
using TMPro;

public class WindowInteraction : MonoBehaviour
{
    public string interactionMessage = "Nothing inside...";
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;
    public float displayDuration = 2f;

    private bool isPlayerNear = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(ShowObservation());
        }
    }

    IEnumerator ShowObservation()
    {
        dialogueBox.SetActive(true);
        dialogueText.text = interactionMessage;
        yield return new WaitForSeconds(displayDuration);
        dialogueBox.SetActive(false);

        // Optional: Logic to allow Kai to "leave" or move to next objective
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerNear = false;
    }
}