using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public CanvasGroup fadeGroup;
    public float fadeDuration = 1.5f;

    private void Awake()
    {
        // Start completely black and blocking clicks
        if (fadeGroup != null)
        {
            fadeGroup.alpha = 1;
            fadeGroup.blocksRaycasts = true;
        }
    }

    // This is called by IntroManager Start
    public IEnumerator FadeIn()
    {
        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            yield return null;
        }
        fadeGroup.alpha = 0;
        fadeGroup.blocksRaycasts = false; // Allow playing the game
    }

    // This is called by the Level Trigger
    public void StartTransition(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        fadeGroup.blocksRaycasts = true; // Stop player input during fade
        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            yield return null;
        }
        SceneManager.LoadScene(sceneName);
    }
}