using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("피격 설정")]
    public int enemyDamage = 10;
    public float invincibilityTime = 0.25f;

    [Header("피격 효과")]
    public float blinkDuration = 0.25f;
    public float blinkInterval = 0.05f;

    private bool isInvincible = false;
    private SpriteRenderer sr;
    private Color originalColor;

    private void Start()
    {
        currentHealth = maxHealth; // ★ 먼저 초기화

        if (InGameHUDManager.instance != null)
        {
            InGameHUDManager.instance.UpdateHealthBar(currentHealth, maxHealth);
        }

        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        Debug.Log("플레이어 현재 체력: " + currentHealth);

        // ★ 피격 시 체력바 즉시 갱신
        if (InGameHUDManager.instance != null)
        {
            InGameHUDManager.instance.UpdateHealthBar(currentHealth, maxHealth);
        }

        StartCoroutine(InvincibilityEffect());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void AddMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth;

        if (InGameHUDManager.instance != null)
        {
            InGameHUDManager.instance.UpdateHealthBar(currentHealth, maxHealth);
        }
    }

    private IEnumerator InvincibilityEffect()
    {
        isInvincible = true;
        float elapsedTime = 0f;
        while (elapsedTime < blinkDuration)
        {
            elapsedTime += Time.deltaTime;
            if ((elapsedTime / blinkInterval) % 2 < 1)
            {
                if (sr != null) sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
            }
            else
            {
                if (sr != null) sr.color = originalColor;
            }
            yield return null;
        }
        if (sr != null) sr.color = originalColor;
        yield return new WaitForSeconds(invincibilityTime - blinkDuration);
        isInvincible = false;
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }

    private void Die()
    {
        Debug.Log("플레이어 사망 - 게임 오버");

        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SaveGameResult();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    public void RestoreHealth(int amount)
    {
        currentHealth = currentHealth + amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log($"[체력 회복] 현재 체력: {currentHealth}/{maxHealth}");

        if (InGameHUDManager.instance != null)
        {
            InGameHUDManager.instance.UpdateHealthBar((float)currentHealth, (float)maxHealth);
        }
    }
}