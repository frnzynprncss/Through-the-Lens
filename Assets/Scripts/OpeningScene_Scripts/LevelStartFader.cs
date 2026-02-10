using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Added for Image component access

public class LevelStartFader : MonoBehaviour
{
    public SceneFader sceneFader;
    public float fadeInDelay = 0.5f;
    public float fadeInDuration = 2.0f;

    void Start()
    {
        if (sceneFader != null)
        {
            // FORCE the image to be black immediately so there is no "flash" of the level
            Image img = sceneFader.GetComponent<Image>();
            if (img != null) img.color = new Color(0, 0, 0, 1);

            StartCoroutine(BeginLevel());
        }
    }

    IEnumerator BeginLevel()
    {
        yield return new WaitForSeconds(fadeInDelay);
        yield return sceneFader.FadeIn(fadeInDuration);
        Debug.Log("Level 1 Started!");
    }
}