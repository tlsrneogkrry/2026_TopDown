using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("피해 설정")]
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
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        Debug.Log("플레이어 체력: " + currentHealth);

        StartCoroutine(InvincibilityEffect());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // UpgradeCard의 MaxHealth 선택 메시지를 실시간 수신하여 연동합니다.
    public void AddMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth;
        Debug.Log("최대 체력 증가! 현재 최대 체력: " + maxHealth);
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
        Debug.Log("플레이어 사망");

        // ★ [데이터 매니저 에러 해결 완벽 동기화] 
        // 주교재 13강 규칙과 GameManager 구조에 맞게 영구 저장 프로세스를 올바르게 호출합니다.
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SaveGameResult();
        }

        // 게임오버 처리를 위해 타이틀 또는 게임오버 씬으로 전환하도록 GameManager를 작동시킵니다.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}