using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI Panels")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject pauseMenu;

    private bool isPaused = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Safety: Ensure time is moving when the scene starts
        Time.timeScale = 1f;
        if (deathScreen != null) deathScreen.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);
    }

    void Update()
    {
        // Toggle pause with ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pauseMenu != null) pauseMenu.SetActive(true);

        // This stops all movement and physics
        Time.timeScale = 0f;
        Debug.Log("Game Paused: TimeScale is 0");
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenu != null) pauseMenu.SetActive(false);

        // This resumes movement and physics
        Time.timeScale = 1f;
        Debug.Log("Game Resumed: TimeScale is 1");
    }

    public void ShowDeathScreen()
    {
        if (deathScreen != null) deathScreen.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // ALWAYS reset time before loading a scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // NEW: Function for your Main Menu button
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // ALWAYS reset time so the menu isn't frozen
        SceneManager.LoadScene("MainMenu"); // Make sure this matches your scene name exactly!
    }
}