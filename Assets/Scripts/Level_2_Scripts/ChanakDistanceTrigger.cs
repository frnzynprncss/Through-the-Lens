using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChanakDistanceTrigger : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform chanak;
    [SerializeField] private float triggerDistance = 3f;

    [Header("Dialogue Content")]
    [TextArea][SerializeField] private string lineToSay = "HALA ANO YAN CHANAK???";
    [SerializeField] private AudioClip dialogueAudio;
    [SerializeField] private Sprite expressionA;
    [SerializeField] private Sprite expressionB;
    [SerializeField] private bool useExpressionB = false;

    [Header("Shake Settings")]
    [SerializeField] private float shakeIntensity = 0.2f;
    [SerializeField] private float shakeDuration = 0.15f;

    private bool hasTriggered = false;

    void Update()
    {
        if (hasTriggered || player == null || chanak == null) return;

        float distance = Vector2.Distance(player.position, chanak.position);

        if (distance <= triggerDistance)
        {
            TriggerChanakDialogue();
        }
    }

    private void TriggerChanakDialogue()
    {
        hasTriggered = true;

        // 1. Shake the screen (See the Coroutine below)
        StartCoroutine(ShakeScreen());

        // 2. Pick expression
        Sprite selectedSprite = useExpressionB ? expressionB : expressionA;

        // 3. Play via NarrativeEngine
        if (NarrativeEngine.Instance != null)
        {
            NarrativeEngine.Instance.PlayLine(lineToSay, dialogueAudio, selectedSprite);
        }

        // 4. Freeze the game world
        Time.timeScale = 0f;
    }

    private System.Collections.IEnumerator ShakeScreen()
    {
        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0f;

        // We use 'WaitForSecondsRealtime' so it works even if Time.timeScale is 0
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;

            Camera.main.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        Camera.main.transform.localPosition = originalPos;
    }

    // Call this from a Button or your NarrativeEngine to unfreeze the game
    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}