using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // Use this if you are using TextMeshPro

public class GameEnding : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup fadeScreen;      // A UI Image that covers the screen
    public GameObject victoryText;    // The text that says "You Escaped!"

    [Header("Characters to Hide")]
    public GameObject player;
    public GameObject enemy;

    [Header("Settings")]
    public float fadeDuration = 2.0f;

    private bool isEnding = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isEnding)
        {
            isEnding = true;
            StartCoroutine(PlayEndingSequence());
        }
    }

    private IEnumerator PlayEndingSequence()
    {
        // 1. Make characters disappear
        if (player != null) player.SetActive(false);
        if (enemy != null) enemy.SetActive(false);

        // 2. Start the Fade to Black
        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeScreen.alpha = timer / fadeDuration;
            yield return null;
        }
        fadeScreen.alpha = 1;

        // 3. Show the Victory Text
        if (victoryText != null)
        {
            victoryText.SetActive(true);
        }

        Debug.Log("Ending Sequence Complete!");
    }
}