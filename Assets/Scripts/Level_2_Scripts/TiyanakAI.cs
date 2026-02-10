using UnityEngine;

public class TiyanakAI : MonoBehaviour
{
    public Sprite[] walkSprites;
    public float moveSpeed = 1.5f;
    public float senseRadius = 5f;
    public float animRate = 0.2f;

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
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < senseRadius)
        {
            // Movement: Stiff follow
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);

            // Code-based Animation: Stiff Walk
            animTimer += Time.deltaTime;
            if (animTimer >= animRate)
            {
                currentFrame = (currentFrame + 1) % walkSprites.Length;
                sr.sprite = walkSprites[currentFrame];
                animTimer = 0;
            }

            // Flip sprite based on direction
            sr.flipX = direction.x < 0;
        }
    }
}