using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object we hit is the Player
        if (collision.CompareTag("Player"))
        {
            // Try to find the FlashlightSystem on the player
            FlashlightSystem flashlight = collision.GetComponent<FlashlightSystem>();

            if (flashlight != null)
            {
                flashlight.Recharge();
                Destroy(gameObject); // Remove battery from world
            }
        }
    }
}