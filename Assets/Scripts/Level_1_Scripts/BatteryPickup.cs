using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    [SerializeField] private float rechargeAmount = 50f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that touched the battery is the Player
        if (other.CompareTag("Player"))
        {
            // Look for the FlashlightSystem in the player's children
            FlashlightSystem flashlight = other.GetComponentInChildren<FlashlightSystem>();

            if (flashlight != null)
            {
                // Use the variable here! 
                // We will create a new method in FlashlightSystem called AddBattery
                flashlight.AddBattery(rechargeAmount);

                Destroy(gameObject);
            }
        }
    }
}