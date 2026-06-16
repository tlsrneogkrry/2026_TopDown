using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("피해 설정")]
    public int enemyDamage = 10;
    public float invincibilityTime = 0.25f; // 무적 시간
    
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
        originalColor = sr.color;
    }

    public void TakeDamage(int damage)
    {
        // 무적 상태면 데미지 받지 않음
        if (isInvincible)
        {
            return;
        }

        currentHealth -= damage;
        Debug.Log("플레이어 체력: " + currentHealth);

        // 무적 시간 시작
        StartCoroutine(InvincibilityEffect());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void AddMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth; // 체력도 함께 회복
        Debug.Log("최대 체력 증가! 현재 최대 체력: " + maxHealth);
    }

    private IEnumerator InvincibilityEffect()
    {
        isInvincible = true;

        // 깜빡거리는 효과
        float elapsedTime = 0f;
        while (elapsedTime < blinkDuration)
        {
            elapsedTime += Time.deltaTime;

            // 일정 간격으로 투명/불투명 반복
            if ((elapsedTime / blinkInterval) % 2 < 1)
            {
                sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
            }
            else
            {
                sr.color = originalColor;
            }

            yield return null;
        }

        // 원래 색상으로 복원
        sr.color = originalColor;

        // 무적 시간 종료
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
        if (GameDateManager.instance != null)
        {
            GameDateManager.instance.PlayerDead();
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