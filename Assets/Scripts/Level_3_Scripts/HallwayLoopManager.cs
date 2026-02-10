using UnityEngine;

public class HallwayLoopManager : MonoBehaviour
{
    [Header("Loop Settings")]
    [SerializeField] private Transform loopStartPoint; // Drag the "Start" position here
    [SerializeField] private GameObject realExitDoor;  // The door that opens after 4 loops
    [SerializeField] private int totalRequiredLoops = 4; // As per your PDF 

    private int currentLoopCount = 0;

    void Start()
    {
        // Ensure the exit door is hidden or locked at the start 
        if (realExitDoor != null) realExitDoor.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            currentLoopCount++;

            if (currentLoopCount < totalRequiredLoops)
            {
                // Teleport Kai back to the start to loop again 
                Vector3 newPos = other.transform.position;
                newPos.x = loopStartPoint.position.x;
                other.transform.position = newPos;

                Debug.Log("Loop Number: " + currentLoopCount);
            }
            else
            {
                // After 4 loops, show the exit 
                if (realExitDoor != null) realExitDoor.SetActive(true);
                Debug.Log("Hallway Loop Broken. Exit is now reachable!");
            }
        }
    }
}