using UnityEngine;

public class ChaseTeleporter : MonoBehaviour
{
    [Header("Player (Kai)")]
    public Transform playerDestination;

    [Header("The Enemy")]
    public GameObject enemyObject;
    public Transform enemyDestination;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Teleport Kai
            TeleportEntity(other.gameObject, playerDestination);

            // 2. Teleport the Enemy
            if (enemyObject != null)
            {
                TeleportEntity(enemyObject, enemyDestination);
            }
        }
    }

    private void TeleportEntity(GameObject entity, Transform target)
    {
        if (target == null) return;

        Rigidbody2D rb = entity.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Stop speed so they don't fly off after teleporting
            rb.velocity = Vector2.zero;
        }

        // FIX: We only move the POSITION. 
        // We do NOT change the rotation or scale, so Kai stays facing his original direction.
        entity.transform.position = target.position;

        Debug.Log($"{entity.name} teleported and kept their facing direction.");
    }
}