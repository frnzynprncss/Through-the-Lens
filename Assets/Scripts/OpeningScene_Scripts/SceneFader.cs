using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    void Awake()
    {
        fadeImage = GetComponent<Image>();
    }

    public IEnumerator FadeIn(float duration)
    {
        // If fadeImage is null, try to get it one last time
        if (fadeImage == null) fadeImage = GetComponent<Image>();

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / duration);

            // This is where it was crashing; now it's protected
            if (fadeImage != null)
                fadeImage.color = new Color(0, 0, 0, alpha);

            yield return null;
        }
    }

    public IEnumerator FadeOut(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1f); // Ensure it is fully black
    }
}