using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Animation Settings")]
    [Tooltip("How fast the animation plays (Frames Per Second)")]
    [SerializeField] private float frameRate = 10f;

    [Header("Sprite Arrays (Drag Images Here)")]
    [Tooltip("Insert your 4 Idle Frames here")]
    [SerializeField] private Sprite[] idleSprites;

    [Tooltip("Insert your 8 Walk Frames here")]
    [SerializeField] private Sprite[] walkSprites;

    [Tooltip("Insert your 4 Hurt Frames here")]
    [SerializeField] private Sprite[] hurtSprites;

    [Header("Flashlight")]
    [SerializeField] private Transform flashlightTransform;

    public bool isHiding = false;

    // State Management
    private enum PlayerState { Idle, Walking, Hurt }
    private PlayerState currentState;
    private PlayerState previousState; // Track previous state to handle frame resets

    // Internal Variables
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private float animationTimer;
    private int currentFrameIndex;
    private bool isFacingRight = true;
    private float hurtTimer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // Setup Rigidbody for 2D movement
        rb.gravityScale = 1;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // 1. Handle Input & Movement
        float moveInput = 0f;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveInput = 1f;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveInput = -1f;
        }

        // 2. Determine State
        if (currentState != PlayerState.Hurt)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

            if (moveInput != 0)
            {
                ChangeState(PlayerState.Walking);
                FlipSprite(moveInput);
            }
            else
            {
                ChangeState(PlayerState.Idle);
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        else
        {
            // Logic for when player is hurt
            rb.velocity = new Vector2(0, rb.velocity.y); // Stop movement during hurt
            hurtTimer -= Time.deltaTime;

            if (hurtTimer <= 0)
            {
                ChangeState(PlayerState.Idle);
            }
        }

        // 3. Play Animation
        HandleAnimation();

        if (isHiding)
        {
            rb.velocity = Vector2.zero;
            ChangeState(PlayerState.Idle);
            return;
        }

    }

    private void ChangeState(PlayerState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        // Reset animation index when switching states to avoid "Out of Bounds" errors
        currentFrameIndex = 0;
        animationTimer = 0f;
    }

    private void HandleAnimation()
    {
        Sprite[] currentAnimationArray = null;

        switch (currentState)
        {
            case PlayerState.Idle:
                currentAnimationArray = idleSprites;
                break;
            case PlayerState.Walking:
                currentAnimationArray = walkSprites;
                break;
            case PlayerState.Hurt:
                currentAnimationArray = hurtSprites;
                break;
        }

        if (currentAnimationArray == null || currentAnimationArray.Length == 0) return;

        // Timer Logic
        animationTimer += Time.deltaTime;
        float secondsPerFrame = 1f / frameRate;

        if (animationTimer >= secondsPerFrame)
        {
            animationTimer -= secondsPerFrame;
            currentFrameIndex++;

            if (currentFrameIndex >= currentAnimationArray.Length)
            {
                // If hurt, we usually don't loop the animation indefinitely 
                // but for this simple logic, it will loop until the timer ends.
                currentFrameIndex = 0;
            }

            spriteRenderer.sprite = currentAnimationArray[currentFrameIndex];
        }
    }

    private void FlipSprite(float direction)
    {
        if (direction > 0 && !isFacingRight)
        {
            isFacingRight = true;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direction < 0 && isFacingRight)
        {
            isFacingRight = false;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    /// <summary>
    /// Call this from an Enemy or Trap script: 
    /// other.GetComponent<PlayerController>().TriggerHurt(0.4f);
    /// </summary>
    public void TriggerHurt(float duration = 0.5f)
    {
        // Don't restart the hurt state if already hurt (provides a tiny bit of I-frame logic)
        if (currentState == PlayerState.Hurt) return;

        currentState = PlayerState.Hurt;
        hurtTimer = duration;
        currentFrameIndex = 0;
        animationTimer = 0f;
    }
}