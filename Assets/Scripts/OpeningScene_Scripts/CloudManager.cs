using UnityEngine;
using UnityEngine.UI;

public class CloudManager : MonoBehaviour
{
    [Header("References")]
    public GameObject[] cloudPrefabs;
    public RectTransform canvasTransform; // Drag your Canvas here

    [Header("Movement Settings")]
    public float spawnInterval = 2.5f;
    public float minSpeed = 80f;
    public float maxSpeed = 200f;

    [Header("Positioning (For 1920x1080)")]
    public float spawnX = 1100f;   // Starts off-screen to the right
    public float destroyX = -1100f; // Disappears off-screen to the left
    public float minY = 150f;      // Bottom of the "sky" area
    public float maxY = 450f;      // Top of the "sky" area

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnUICloud();
            timer = 0;
        }
    }

    void SpawnUICloud()
    {
        int randomIndex = Random.Range(0, cloudPrefabs.Length);
        GameObject cloud = Instantiate(cloudPrefabs[randomIndex], canvasTransform);

        RectTransform rt = cloud.GetComponent<RectTransform>();

        // Reset scale in case the prefab scale is weird
        rt.localScale = Vector3.one;

        // Set starting position
        float randomY = Random.Range(minY, maxY);
        rt.anchoredPosition = new Vector2(spawnX, randomY);

        float speed = Random.Range(minSpeed, maxSpeed);
        StartCoroutine(MoveUICloud(rt, speed));
    }

    System.Collections.IEnumerator MoveUICloud(RectTransform cloudRT, float speed)
    {
        while (cloudRT != null)
        {
            // Move left
            cloudRT.anchoredPosition += Vector2.left * speed * Time.deltaTime;

            // Check if it's passed the destroy point
            if (cloudRT.anchoredPosition.x < destroyX)
            {
                Destroy(cloudRT.gameObject);
                yield break;
            }
            yield return null;
        }
    }
}