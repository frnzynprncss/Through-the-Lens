using UnityEngine;

public class CreatureAI : MonoBehaviour
{
    [Header("Movement & Animation")]
    public Sprite[] runSprites;
    public float runSpeed = 4.5f;
    public float patrolSpeed = 2.0f; // Slower when searching
    public float animRate = 0.1f;

    [Header("Attack Settings")]
    public float damageAmount = 10f;
    public float attackCooldown = 1.0f;
    private float nextAttackTime;

    [Header("Patrol Logic")]
    public float changeDirectionTime = 3.0f;
    private float patrolTimer;
    private Vector2 patrolDirection;

    private SpriteRenderer sr;
    private Transform player;
    private SpriteRenderer playerSpriteRenderer;
    private int currentFrame;
    private float animTimer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        // Initialize a random starting patrol direction
        PickNewPatrolDirection();
    }

    void OnEnable()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerSpriteRenderer = playerObj.GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (player == null) return;

        // Check if player is hiding (Sprite is disabled)
        bool isPlayerHiding = (playerSpriteRenderer != null && !playerSpriteRenderer.enabled);

        if (isPlayerHiding)
        {
            Patrol();
        }
        else
        {
            Chase();
        }

        HandleAnimation(isPlayerHiding ? patrolDirection : (Vector2)(player.position - transform.position));
    }

    private void Chase()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * runSpeed * Time.deltaTime, Space.World);
        sr.flipX = direction.x < 0;
    }

    private void Patrol()
    {
        patrolTimer += Time.deltaTime;

        // Change direction every few seconds so it doesn't just walk into a wall forever
        if (patrolTimer >= changeDirectionTime)
        {
            PickNewPatrolDirection();
            patrolTimer = 0;
        }

        transform.Translate(patrolDirection * patrolSpeed * Time.deltaTime, Space.World);
        sr.flipX = patrolDirection.x < 0;
    }

    private void PickNewPatrolDirection()
    {
        // Picks a random direction (Left or Right) 
        // You can change this to include Up/Down if your game allows it
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-0.2f, 0.2f); // Slight Y variation
        patrolDirection = new Vector2(randomX, randomY).normalized;
    }

    private void HandleAnimation(Vector2 direction)
    {
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
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerSpriteRenderer != null && playerSpriteRenderer.enabled)
            {
                if (Time.time >= nextAttackTime)
                {
                    AttackPlayer(collision.gameObject);
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
        }
    }

    private void AttackPlayer(GameObject target)
    {
        PlayerHealth health = target.GetComponent<PlayerHealth>();
        if (health != null) health.TakeDamage(damageAmount);
    }
}