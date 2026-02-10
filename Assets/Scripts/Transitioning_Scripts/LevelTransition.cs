using UnityEngine;
using UnityEngine.SceneManagement; // Required for changing levels

public class LevelTransition : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private string nextLevelName = "Level2"; // Make sure this matches your Scene name exactly

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object colliding with the trigger is the Player
        if (other.CompareTag("Player"))
        {
            LoadNextLevel();
        }
    }

    private void LoadNextLevel()
    {
        // You can add a Debug log to verify it's working in the Console
        Debug.Log("Transitioning to: " + nextLevelName);

        SceneManager.LoadScene(nextLevelName);
    }
}