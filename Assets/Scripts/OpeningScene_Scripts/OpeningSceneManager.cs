using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningSceneManager : MonoBehaviour
{
    [SerializeField] private string nextLevelName = "Level_1";

    // Call this function when the dialogue/video ends
    public void LoadFirstLevel()
    {
        SceneManager.LoadScene(nextLevelName);
    }

    // Optional: Allow players to skip the opening with Space or Enter
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            LoadFirstLevel();
        }
    }
}