using UnityEngine;

public class ChanakEnemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float crawlSpeed = 6f;

    [Header("Behavior")]
    [SerializeField] private float spawnIntervalMin = 10f;
    [SerializeField] private float spawnIntervalMax = 25f;

    [Header("Animation (Code-Based)")]
    [SerializeField] private Sprite[] crawlFrames; // Drag your 2 sprites here
    [SerializeField] private float animSpeed = 0.15f;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private bool isAttacking = false;
    private int currentFrame;
    private float animTimer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        gameObject.SetActive(false);
        Invoke("AttemptSpawn", Random.Range(spawnIntervalMin, spawnIntervalMax));
    }

    void Update()
    {
        if (isAttacking && player != null)
        {
            // 1. Move towards player
            transform.position = Vector3.MoveTowards(transform.position, player.position, crawlSpeed * Time.deltaTime);

            // 2. Handle 2-Frame Animation
            HandleCrawlAnimation();

            // 3. Attack Check
            if (Vector2.Distance(transform.position, player.position) < 0.8f)
            {
                AttackPlayer();
            }
        }
    }

    private void HandleCrawlAnimation()
    {
        if (crawlFrames.Length < 2) return;

        animTimer += Time.deltaTime;
        if (animTimer >= animSpeed)
        {
            animTimer = 0;
            currentFrame = (currentFrame + 1) % crawlFrames.Length;
            spriteRenderer.sprite = crawlFrames[currentFrame];
        }
    }

    void AttemptSpawn()
    {
        // Only spawn if player is NOT hiding (check if player sprite is visible)
        if (player.GetComponent<SpriteRenderer>().enabled)
        {
            isAttacking = true;
            gameObject.SetActive(true);

            // Spawn behind player on the ceiling
            float spawnSide = Random.value > 0.5f ? 10f : -10f;
            transform.position = new Vector3(player.position.x + spawnSide, player.position.y + 3.5f, 0);
        }
        else
        {
            // If player is hiding, wait and try again
            Invoke("AttemptSpawn", 3f);
        }
    }

    void AttackPlayer()
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null) pc.TriggerHurt(0.6f);

        // Retreat after attack
        isAttacking = false;
        gameObject.SetActive(false);
        Invoke("AttemptSpawn", Random.Range(spawnIntervalMin, spawnIntervalMax));
    }
}