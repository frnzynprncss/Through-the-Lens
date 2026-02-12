using UnityEngine;

public class HallucinationAI : MonoBehaviour
{
    [Header("Damage Settings")]
    [Tooltip("How much damage to deal per hit")]
    public float damageAmount = 10f;
    [Tooltip("Seconds between each hit")]
    public float damageCooldown = 2f;
    private float lastDamageTime;

    [Header("Sanity Thresholds")]
    [Tooltip("Must match the Hallucination Threshold in LockerMechanic")]
    public float appearanceThreshold = 30f;
    [Tooltip("Sanity level where it stops roaming and starts chasing")]
    public float chaseThreshold = 15f;

    [Header("Movement")]
    public float roamSpeed = 2f;
    public float chaseSpeedMultiplier = 1.5f;
    public Transform[] waypoints;
    private int currentWaypointIndex;

    [Header("Visuals")]
    public Sprite[] faces;
    public float animationSpeed = 0.15f;
    public float fadeSpeed = 0.5f;

    private SpriteRenderer sr;
    private Transform player;
    private LockerMechanic sanityManager;
    private PlayerHealth playerHealth;
    private PlayerController playerController;

    private float animTimer;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
            playerController = playerObj.GetComponent<PlayerController>();
        }

        // Find the script that holds sanity and hiding logic
        sanityManager = FindFirstObjectByType<LockerMechanic>();
    }

    void Update()
    {
        if (sanityManager == null || player == null) return;

        // 1. Logic: Check Sanity & Hiding State
        float sanity = sanityManager.currentSanity;
        bool isHiding = sanityManager.isHiding;

        // Calculate intensity: 0 at threshold, 1 at 0 sanity
        float intensity = 1f - (sanity / appearanceThreshold);
        intensity = Mathf.Clamp01(intensity);

        // 2. Visuals: Get worse as sanity decreases
        UpdateVisuals(intensity);

        // 3. Behavior Tree
        if (isHiding)
        {
            // Player is safe in locker, ghost just roams
            RoamHallways();
        }
        else if (sanity <= chaseThreshold)
        {
            // Sanity is critical, start the chase
            ChasePlayer(intensity);
            TryDamagePlayer();
        }
        else
        {
            // Sanity is low enough to appear, but not yet chasing
            RoamHallways();
        }
    }

    void UpdateVisuals(float intensity)
    {
        // Face swap animation
        if (faces.Length > 0)
        {
            animTimer += Time.deltaTime;
            if (animTimer >= animationSpeed)
            {
                sr.sprite = faces[Random.Range(0, faces.Length)];
                animTimer = 0;
            }
        }

        // Transparency: Higher intensity = more solid/visible
        Color c = sr.color;
        float flicker = Mathf.PingPong(Time.time * fadeSpeed, 0.7f);
        c.a = flicker * intensity;
        sr.color = c;
    }

    void RoamHallways()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, roamSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    void ChasePlayer(float intensity)
    {
        // Ghost gets faster as sanity drops lower
        float currentChaseSpeed = roamSpeed * chaseSpeedMultiplier * (1f + intensity);
        transform.position = Vector3.MoveTowards(transform.position, player.position, currentChaseSpeed * Time.deltaTime);
    }

    void TryDamagePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // If touching player and cooldown is ready
        if (distance < 1.2f && Time.time >= lastDamageTime + damageCooldown)
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);

                // Also trigger the visual hurt animation from the controller
                if (playerController != null) playerController.TriggerHurt(0.4f);

                lastDamageTime = Time.time;
            }
        }
    }
}