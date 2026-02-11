using UnityEngine;

public class MovementTutorial : MonoBehaviour
{
    public GameObject tutorialUI; // Drag your "Press A/D" UI object here
    private bool playerMoved = false;

    void Update()
    {
        // Check if the tutorial is active and if the player has pressed a key
        if (tutorialUI.activeSelf && !playerMoved)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) ||
                Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                playerMoved = true;
                Invoke("HideTutorial", 0.5f); // Hides it after a short delay
            }
        }
    }

    void HideTutorial()
    {
        tutorialUI.SetActive(false);
    }

    public void ShowTutorial()
    {
        tutorialUI.SetActive(true);
    }
}