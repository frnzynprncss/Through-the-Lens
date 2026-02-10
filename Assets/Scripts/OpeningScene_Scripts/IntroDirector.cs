using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Added this

public class IntroDirector : MonoBehaviour
{
    [Header("References")]
    public SceneFader sceneFader;
    public DialogueManager dialogueManager;

    [Header("Scene Objects")]
    public GameObject vanInterior;
    public GameObject asylumExterior;
    public GameObject playerKai;

    [Header("Story Data")]
    public Dialogue introDialogue;
    public string level1SceneName = "Level_1"; // Name of your next scene

    void Start()
    {
        vanInterior.SetActive(true);
        asylumExterior.SetActive(false);
        playerKai.SetActive(false);

        StartCoroutine(RunIntroSequence());
    }

    IEnumerator RunIntroSequence()
    {
        // 1. Start Black, then Fade In
        yield return sceneFader.FadeIn(2.0f);

        // 2. Run Dialogue
        dialogueManager.StartDialogue(introDialogue);

        // 3. Wait until dialogue ends
        while (dialogueManager.isDialogueActive)
        {
            yield return null;
        }

        // 4. Fade to Black
        yield return sceneFader.FadeOut(1.5f);

        // 5. Swap Visuals
        vanInterior.SetActive(false);
        asylumExterior.SetActive(true);
        playerKai.SetActive(true);

        // 6. Fade In to Exterior
        yield return sceneFader.FadeIn(1.5f);

        // 7. WAIT a few seconds so the player can see the Asylum
        yield return new WaitForSeconds(2f);

        // 8. Final Fade Out and LOAD LEVEL_1
        yield return sceneFader.FadeOut(1.5f);
        SceneManager.LoadScene(level1SceneName);
    }
}