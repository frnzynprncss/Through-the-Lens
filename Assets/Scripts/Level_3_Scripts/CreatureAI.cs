using UnityEngine;

public class CreatureAI : MonoBehaviour
{
    public Sprite[] runSprites;
    public float runSpeed = 4.5f;
    public float animRate = 0.1f; // Faster for running

    private SpriteRenderer sr;
    private Transform player;
    private int currentFrame;
    private float animTimer;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Movement: Direct Pursuit
        Vector2 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * runSpeed * Time.deltaTime);

        // Code-based Animation: Running Cycle
        animTimer += Time.deltaTime;
        if (animTimer >= animRate)
        {
            currentFrame = (currentFrame + 1) % runSprites.Length;
            sr.sprite = runSprites[currentFrame];
            animTimer = 0;
        }

        sr.flipX = direction.x < 0;
    }
}