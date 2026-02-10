using UnityEngine;

public class HallucinationAI : MonoBehaviour
{
    public Sprite[] faces;
    public float animationSpeed = 0.15f;
    public float followSpeed = 2f;
    public float fadeSpeed = 0.5f;

    private SpriteRenderer sr;
    private Transform player;
    private float timer;
    private int frame;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        // 1. Code-based Animation: Random Face Swapping
        if (faces.Length > 0)
        {
            timer += Time.deltaTime;
            if (timer >= animationSpeed)
            {
                frame = Random.Range(0, faces.Length);
                sr.sprite = faces[frame];
                timer = 0;
            }
        }

        // 2. Movement: Ethereal Floating follow
        if (player != null)
        {
            // Position it slightly offset so it haunts Kai rather than sitting on top of him
            Vector3 targetPos = player.position + new Vector3(2f, 1f, 0);
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
        }

        // 3. Visual Effect: "Swish" / Alpha Flicker
        Color c = sr.color;
        c.a = Mathf.PingPong(Time.time * fadeSpeed, 0.7f);
        sr.color = c;
    }
}