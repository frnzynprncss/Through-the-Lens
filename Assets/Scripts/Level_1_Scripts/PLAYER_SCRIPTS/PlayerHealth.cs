using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement; // To reload scene on death

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI References")]
    public Image healthFillImage; // Assign a health bar UI image
    public GameObject deathScreen; // Assign your Game Over UI panel

    [Header("Damage Visuals")]
    public float flashDuration = 0.1f;
    private SpriteRenderer sr;
    private Color originalColor;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;

        UpdateHealthUI();
        if (deathScreen != null) deathScreen.SetActive(false);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();
        StartCoroutine(DamageFlash());

        // Play damage sound via your AudioManager
        if (AudioManager.instance != null) AudioManager.instance.Play("PlayerHurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = currentHealth / maxHealth;
        }
    }

    IEnumerator DamageFlash()
    {
        if (sr != null)
        {
            sr.color = Color.red; // Flash red when hit
            yield return new WaitForSeconds(flashDuration);
            sr.color = originalColor;
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Kai has fallen...");

        // Disable movement (Assuming your script is named PlayerController)
        var controller = GetComponent<PlayerController>();
        if (controller != null) controller.enabled = false;

        // Show Death Screen
        if (deathScreen != null) deathScreen.SetActive(true);

        // Optional: Trigger a "Game Over" sound
        if (AudioManager.instance != null) AudioManager.instance.Play("GameOver");
    }

    // Call this from a "Restart" button on your Death Screen
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}