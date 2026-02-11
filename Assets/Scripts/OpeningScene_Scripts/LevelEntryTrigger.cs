using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEntryTrigger : MonoBehaviour
{
    public string levelToLoad = "Level1";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object touching the trigger is Kai
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(levelToLoad);
        }
    }
}