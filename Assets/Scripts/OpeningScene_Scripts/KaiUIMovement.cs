using UnityEngine;
using UnityEngine.UI;

public class KaiUIMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 300f;
    public RectTransform rectTransform;
    public bool canMove = false;

    [Header("Animation Assets")]
    public Image displayImage;
    public Sprite[] idleFrames;      // Drag your 4 Idle sprites here
    public Sprite[] walkFrames;      // Drag your 4 Walking sprites here
    public float frameRate = 0.15f;  // Speed of animation

    private float moveX;
    private int currentFrame;
    private float timer;
    private bool isWalking;

    void Update()
    {
        if (!canMove) return;

        moveX = Input.GetAxisRaw("Horizontal");

        if (moveX != 0)
        {
            // FIX: Multiply 0.3f (your scale) by the direction (1 or -1)
            float flipScale = moveX > 0 ? 1f : -1f;
            rectTransform.localScale = new Vector3(flipScale, 1f, 1f);

            HandleAnimation(walkFrames);
        }
        else
        {
            HandleAnimation(idleFrames);
        }
    }

    void FixedUpdate()
    {
        if (!canMove) return;
        Vector2 newPos = rectTransform.anchoredPosition;
        newPos.x += moveX * moveSpeed * Time.fixedDeltaTime;
        rectTransform.anchoredPosition = newPos;
    }

    // Generic function to handle looping through any sprite array
    void HandleAnimation(Sprite[] frames)
    {
        if (frames.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= frameRate)
        {
            timer -= frameRate;
            currentFrame = (currentFrame + 1) % frames.Length;
            displayImage.sprite = frames[currentFrame];
        }
    }
}