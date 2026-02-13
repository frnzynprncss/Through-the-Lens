using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    [Tooltip("Drag the Creature GAME OBJECT here")]
    public GameObject creatureObject; // Changed from CreatureAI to GameObject

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (creatureObject != null)
            {
                // Wake up the entire monster
                creatureObject.SetActive(true);

                // Double check it has the script and enable that too
                CreatureAI script = creatureObject.GetComponent<CreatureAI>();
                if (script != null) script.enabled = true;

                Debug.Log("GO SIGNAL SENT: Creature is now Active!");
            }

            Destroy(gameObject);
        }
    }
}