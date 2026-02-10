using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Configuration")]
    [Tooltip("The name of the Opening Cutscene/Dialogue scene.")]
    [SerializeField] private string openingSceneName = "OpeningScene";

    [Header("UI Panels")]
    [SerializeField] private GameObject creditsPanel;

    // 1. LOGIC FOR THE PLAY BUTTON
    public void PlayGame()
    {
        // This now loads the Opening Scene first
        SceneManager.LoadScene(openingSceneName);
    }

    public void OpenCredits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}