using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI References")]
    public Image healthFillImage;

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
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();
        StartCoroutine(DamageFlash());

        if (SoundManager.instance != null && SoundManager.instance.playerHurt != null)
        {
            SoundManager.instance.PlaySFX(SoundManager.instance.playerHurt);
        }

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
            sr.color = Color.red;
            yield return new WaitForSeconds(flashDuration);
            sr.color = originalColor;
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Kai has fallen...");

        // Disable movement
        var controller = GetComponent<PlayerController>();
        if (controller != null) controller.enabled = false;

        // TELL GAMEMANAGER TO SHOW THE UI
        if (GameManager.instance != null)
        {
            GameManager.instance.ShowDeathScreen();
        }

        // Play Game Over Sound
        if (SoundManager.instance != null && SoundManager.instance.gameOver != null)
        {
            SoundManager.instance.PlaySFX(SoundManager.instance.gameOver);
        }
    }
}