using UnityEngine;
using UnityEngine.UI;

public class LoreLog : MonoBehaviour
{
    [TextArea(3, 10)]
    [SerializeField] private string logContent;
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private Text dialogueText;

    private bool isNear = false;

    void Update()
    {
        if (isNear && Input.GetKeyDown(KeyCode.E))
        {
            ShowLog();
        }
    }

    void ShowLog()
    {
        dialogueUI.SetActive(true);
        dialogueText.text = logContent;
        // Optional: Pause game or disable movement
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isNear = false;
            dialogueUI.SetActive(false);
        }
    }
}