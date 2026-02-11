using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;

    [Header("Character UI Portraits (Intro)")]
    public GameObject claraUIPortrait;
    public GameObject kaiUIPortrait;

    [Header("World Characters (Gameplay)")]
    public GameObject claraNPC;       // The pixel chibi Clara
    public GameObject kaiPlayer;      // The pixel chibi Kai

    [Header("Settings & Data")]
    public IntroSceneData sceneData;
    public float typingSpeed = 0.05f;

    [Header("External Scripts")]
    public KaiUIMovement kaiMoveScript;
    public MovementTutorial tutorialScript;
    public SceneTransition transitionScript;

    //Audio Source
    [Header("Audio")]
    public AudioSource voiceSource;

    private int currentIndex = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    // We use IEnumerator Start to wait for the Fade In before starting dialogue
    IEnumerator Start()
    {
        // Hide dialogue UI initially
        dialogueBox.SetActive(false);

        // Trigger the Fade In from the SceneTransition script
        if (transitionScript != null)
        {
            yield return StartCoroutine(transitionScript.FadeIn());
        }

        // Only after the fade is done, show the dialogue
        if (sceneData != null && sceneData.lines.Length > 0)
        {
            dialogueBox.SetActive(true);
            DisplayLine();
        }
    }

    void Update()
    {
        // Progress dialogue on Click or Space
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = sceneData.lines[currentIndex].sentence;
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    public void NextLine()
    {
        currentIndex++;
        if (currentIndex < sceneData.lines.Length)
        {
            DisplayLine();
        }
        else
        {
            EndIntro();
        }
    }

    void DisplayLine()
    {
        var line = sceneData.lines[currentIndex];
        nameText.text = line.characterName;

        // ... (keep your existing portrait and sprite switching logic here) ...

        // VOICE OVER LOGIC
        if (voiceSource != null && line.voiceClip != null)
        {
            voiceSource.Stop(); // Stop the previous voice line
            voiceSource.clip = line.voiceClip;
            voiceSource.Play();
        }

        // Typewriter effect
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(line.sentence));
    }

    IEnumerator TypeText(string fullText)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in fullText.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void EndIntro()
    {
        // Hide UI
        dialogueBox.SetActive(false);
        claraUIPortrait.SetActive(false);
        kaiUIPortrait.SetActive(false);

        // Show Gameplay Characters
        claraNPC.SetActive(true);
        kaiPlayer.SetActive(true);

        // Enable Movement & Tutorial
        kaiMoveScript.canMove = true;
        if (tutorialScript != null) tutorialScript.ShowTutorial();

        Debug.Log("Intro Finished - Gameplay Started");
    }
}