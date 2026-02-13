using UnityEngine;

public class CreatureAI : MonoBehaviour
{
    [Header("Movement & Animation")]
    public Sprite[] runSprites;
    public float runSpeed = 4.5f;
    public float animRate = 0.1f;

    [Header("Attack Settings")]
    public float damageAmount = 10f;
    public float attackCooldown = 1.0f;
    private float nextAttackTime;

    private SpriteRenderer sr;
    private Transform player;
    private int currentFrame;
    private float animTimer;

    // Awake runs even if the script starts disabled
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        // Debug to prove it's working
        Debug.Log("Creature AI has officially started moving!");
    }

    void Update()
    {
        if (player == null) return;

        // Movement: Direct Pursuit (Space.World ensures it goes TOWARD the player)
        Vector2 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * runSpeed * Time.deltaTime, Space.World);

        // Animation
        if (runSprites.Length > 0)
        {
            animTimer += Time.deltaTime;
            if (animTimer >= animRate)
            {
                currentFrame = (currentFrame + 1) % runSprites.Length;
                sr.sprite = runSprites[currentFrame];
                animTimer = 0;
            }
        }

        sr.flipX = direction.x < 0;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= nextAttackTime)
            {
                AttackPlayer(collision.gameObject);
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    private void AttackPlayer(GameObject target)
    {
        PlayerHealth health = target.GetComponent<PlayerHealth>();
        if (health != null) health.TakeDamage(damageAmount);
    }
}