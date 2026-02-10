using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject pauseMenuPanel;

    [Header("Audio Settings")]
    [Tooltip("The AudioSource that will play the sound.")]
    public AudioSource audioSource;
    [Tooltip("The sound clip to play when the escape key is pressed.")]
    public AudioClip pauseSound;

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu";

    [Header("Cursor Settings")]
    public bool lockCursorDuringGameplay = false;

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // Initialize cursor state based on your settings
        if (lockCursorDuringGameplay)
        {
            SetCursorState(false); // Lock and hide cursor at start
        }

        // This ensures the sound plays even if the game audio is globally paused
        if (audioSource != null) audioSource.ignoreListenerPause = true;
    }

    void Update()
    {
        // Toggle pause when Escape is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Play sound effect
            PlayPauseSFX();

            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PlayPauseSFX()
    {
        if (audioSource != null && pauseSound != null)
        {
            // PlayOneShot allows the sound to play even if the source is busy
            audioSource.PlayOneShot(pauseSound);
        }
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        // Always show the cursor when paused so the player can click buttons
        SetCursorState(true);
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        // If the game uses a locked cursor, re-lock it now
        if (lockCursorDuringGameplay)
        {
            SetCursorState(false);
        }
    }

    public void RetryLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void SetCursorState(bool isVisible)
    {
        if (isVisible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}