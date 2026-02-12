using UnityEngine;
using System.Collections;

public class ChanakEnemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float crawlSpeed = 6f;
    [SerializeField] private float jumpSpeed = 12f;

    [Header("Behavior")]
    [SerializeField] private float spawnIntervalMin = 10f;
    [SerializeField] private float spawnIntervalMax = 25f;
    [SerializeField] private float lungeDistance = 0.5f;
    [SerializeField] private float damageValue = 20f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip spawnScreech;
    [SerializeField] private AudioClip lungeScreech;

    [Header("Animation")]
    [SerializeField] private Sprite[] crawlFrames;
    [SerializeField] private float animSpeed = 0.08f;

    [Header("Dialogue Integration")]
    [SerializeField] private float dialogueTriggerDistance = 3f;
    private bool hasPlayedProximityDialogue = false;

    private Transform player;
    private PlayerHealth playerHealth;
    private SpriteRenderer spriteRenderer;
    private Collider2D enemyCollider;
    private bool isHunting = false;
    private bool isLunging = false;
    private int currentFrame;
    private float animTimer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();

        GameObject kai = GameObject.Find("Kai");
        if (kai != null)
        {
            player = kai.transform;
            playerHealth = kai.GetComponent<PlayerHealth>();
        }
    }

    void Start()
    {
        SetEnemyActive(false);
        StartCoroutine(SpawnTimer());
    }

    void Update()
    {
        // If game is paused, we don't process horizontal movement
        if (Time.timeScale == 0 && !isLunging) return;

        if (Input.GetKeyDown(KeyCode.P) && !isHunting)
        {
            DoSpawn();
        }

        if (isHunting && player != null)
        {
            MoveOnCeiling();
            HandleAnimation();
            CheckForDialogue();
            CheckForLunge();
        }
    }

    void SetEnemyActive(bool state)
    {
        isHunting = state;
        if (spriteRenderer != null) spriteRenderer.enabled = state;
        if (enemyCollider != null) enemyCollider.enabled = state;
    }

    IEnumerator SpawnTimer()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(Random.Range(spawnIntervalMin, spawnIntervalMax));
            if (!isHunting) DoSpawn();
        }
    }

    void DoSpawn()
    {
        SetEnemyActive(true);
        isLunging = false;
        if (player == null) return;

        float spawnSide = Random.value > 0.5f ? 8f : -8f;
        transform.position = new Vector3(player.position.x + spawnSide, player.position.y + 4.5f, 0);

        // Ceiling flip
        spriteRenderer.flipY = true;

        if (audioSource != null && spawnScreech != null)
            audioSource.PlayOneShot(spawnScreech);
    }

    void MoveOnCeiling()
    {
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, 0);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, crawlSpeed * Time.deltaTime);
        spriteRenderer.flipX = (player.position.x < transform.position.x);
    }

    void CheckForDialogue()
    {
        if (!hasPlayedProximityDialogue && player != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist <= dialogueTriggerDistance)
            {
                hasPlayedProximityDialogue = true;
                DialogueTrigger proximityTrigger = GetComponent<DialogueTrigger>();
                if (proximityTrigger != null && NarrativeEngine.Instance != null)
                {
                    NarrativeEngine.Instance.PlayLine(proximityTrigger.lineToSay, null, null);
                }
            }
        }
    }

    void CheckForLunge()
    {
        if (player != null && Mathf.Abs(transform.position.x - player.position.x) < lungeDistance && !isLunging)
        {
            StartCoroutine(PerformLunge());
        }
    }

    IEnumerator PerformLunge()
    {
        isHunting = false;
        isLunging = true;

        if (audioSource != null && lungeScreech != null)
            audioSource.PlayOneShot(lungeScreech);

        yield return new WaitForSecondsRealtime(0.1f);

        // Flip back to normal feet-down orientation for the attack
        spriteRenderer.flipY = false;

        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(player.position.x, player.position.y, 0);
        float t = 0;

        while (t < 1)
        {
            // Use unscaledDeltaTime so the attack doesn't "freeze" mid-air during dialogue
            t += Time.unscaledDeltaTime * jumpSpeed;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageValue);
        }

        yield return new WaitForSecondsRealtime(0.6f);

        SetEnemyActive(false);
        isLunging = false;
    }

    void HandleAnimation()
    {
        if (crawlFrames.Length == 0) return;
        animTimer += Time.deltaTime;
        if (animTimer >= animSpeed)
        {
            animTimer = 0;
            currentFrame = (currentFrame + 1) % crawlFrames.Length;
            spriteRenderer.sprite = crawlFrames[currentFrame];
        }
    }
}