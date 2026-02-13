using UnityEngine;

public class HallwayLoopManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform loopStartPoint;
    [SerializeField] private GameObject realExitDoor;
    [SerializeField] private int totalRequiredLoops = 4;

    [Header("Detection")]
    [SerializeField] private LayerMask playerLayer;

    private int currentLoopCount = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Ensure it's ONLY the player
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            // 2. Check if the player is currently hiding
            // Replace 'PlayerController' with the name of your actual movement/hiding script
            var playerScript = other.GetComponent<PlayerController>();

            if (playerScript != null && playerScript.isHiding)
            {
                // If the player is hiding, do nothing. 
                // They shouldn't teleport while inside a locker.
                return;
            }

            currentLoopCount++;

            if (currentLoopCount < totalRequiredLoops)
            {
                TeleportPlayer(other.transform);
            }
            else
            {
                BreakLoop();
            }
        }
    }

    private void TeleportPlayer(Transform playerTransform)
    {
        playerTransform.position = loopStartPoint.position;

        if (playerTransform.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            // Change 'linearVelocity' to 'velocity' for older Unity versions
            rb.velocity = Vector2.zero;
        }
    }

    private void BreakLoop()
    {
        if (realExitDoor != null) realExitDoor.SetActive(true);
        gameObject.SetActive(false);
    }
}