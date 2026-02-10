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

    // State Management
    private enum PlayerState { Idle, Walking, Hurt }
    private PlayerState currentState;

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
        rb.gravityScale = 1; // Set to 0 if this is a top-down game, 1 if side-scroller
        rb.freezeRotation = true; // Prevent player from rolling around
    }

    private void Update()
    {
        // 1. Handle Input & Movement
        float moveInput = 0f;

        // Check for Right Movement (D or Right Arrow)
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveInput = 1f;
        }
        // Check for Left Movement (A or Left Arrow - assuming A is standard)
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveInput = -1f;
        }
        // Note: If you strictly needed 'W' to move Left/Right, change KeyCode.A to KeyCode.W above.

        // 2. Determine State
        if (currentState != PlayerState.Hurt)
        {
            // Move the player physically
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

            if (moveInput != 0)
            {
                currentState = PlayerState.Walking;
                FlipSprite(moveInput);
            }
            else
            {
                currentState = PlayerState.Idle;
                rb.velocity = new Vector2(0, rb.velocity.y); // Stop sliding
            }
        }
        else
        {
            // Logic for when player is hurt (stop moving briefly)
            hurtTimer -= Time.deltaTime;
            if (hurtTimer <= 0)
            {
                currentState = PlayerState.Idle; // Return to normal
            }
        }

        // 3. Play Animation
        HandleAnimation();
    }

    private void HandleAnimation()
    {
        Sprite[] currentAnimationArray = null;

        // Pick the correct array based on state
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

        // Safety check: ensure we have sprites
        if (currentAnimationArray == null || currentAnimationArray.Length == 0) return;

        // Timer Logic
        animationTimer += Time.deltaTime;
        float secondsPerFrame = 1f / frameRate;

        if (animationTimer >= secondsPerFrame)
        {
            animationTimer -= secondsPerFrame;
            currentFrameIndex++;

            // Loop back to start if we reach the end
            if (currentFrameIndex >= currentAnimationArray.Length)
            {
                currentFrameIndex = 0;
            }

            // Apply the sprite
            spriteRenderer.sprite = currentAnimationArray[currentFrameIndex];
        }
    }

    private void FlipSprite(float direction)
    {
        // If moving RIGHT
        if (direction > 0 && !isFacingRight)
        {
            isFacingRight = true;
            // Face normal (0 degrees)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        // If moving LEFT
        else if (direction < 0 && isFacingRight)
        {
            isFacingRight = false;
            // Face opposite (Flip 180 degrees on Y-Axis)
            // This mirrors the Sprite AND the Flashlight perfectly!
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    // Call this method from other scripts (like a trap) to trigger the hurt anim
    public void TriggerHurt(float duration = 0.5f)
    {
        currentState = PlayerState.Hurt;
        hurtTimer = duration;
        currentFrameIndex = 0; // Reset animation to start
    }
}