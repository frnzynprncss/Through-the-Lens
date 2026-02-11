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
    public GameObject claraNPC;
    public GameObject kaiPlayer;

    [Header("Settings & Data")]
    public IntroSceneData sceneData;
    public float typingSpeed = 0.05f;

    [Header("External Scripts")]
    public KaiUIMovement kaiMoveScript;
    public MovementTutorial tutorialScript;
    public SceneTransition transitionScript;

    [Header("Audio")]
    public AudioSource voiceSource;

    private int currentIndex = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    IEnumerator Start()
    {
        if (dialogueBox) dialogueBox.SetActive(false);
        if (claraNPC) claraNPC.SetActive(false);
        if (kaiPlayer) kaiPlayer.SetActive(false);
        if (kaiMoveScript) kaiMoveScript.canMove = false;

        if (transitionScript != null)
        {
            yield return StartCoroutine(transitionScript.FadeIn());
        }

        if (sceneData != null && sceneData.lines.Length > 0)
        {
            if (dialogueBox) dialogueBox.SetActive(true);
            DisplayLine();
        }
    }

    void Update()
    {
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

        // Safety check character name
        bool isClara = line.characterName.Trim().ToLower() == "clara";

        // ACTIVATE PORTRAITS - ONLY if they are assigned in Inspector
        if (claraUIPortrait != null) claraUIPortrait.SetActive(isClara);
        if (kaiUIPortrait != null) kaiUIPortrait.SetActive(!isClara);

        // Update the sprite
        if (isClara && claraUIPortrait != null)
            claraUIPortrait.GetComponent<Image>().sprite = line.expressionSprite;
        else if (!isClara && kaiUIPortrait != null)
            kaiUIPortrait.GetComponent<Image>().sprite = line.expressionSprite;

        // Voice Over
        if (voiceSource != null && line.voiceClip != null)
        {
            voiceSource.Stop();
            voiceSource.clip = line.voiceClip;
            voiceSource.Play();
        }

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
        if (dialogueBox) dialogueBox.SetActive(false);
        if (claraUIPortrait) claraUIPortrait.SetActive(false);
        if (kaiUIPortrait) kaiUIPortrait.SetActive(false);

        if (claraNPC) claraNPC.SetActive(true);
        if (kaiPlayer) kaiPlayer.SetActive(true);

        if (kaiMoveScript) kaiMoveScript.canMove = true;
        if (tutorialScript != null) tutorialScript.ShowTutorial();
    }
}