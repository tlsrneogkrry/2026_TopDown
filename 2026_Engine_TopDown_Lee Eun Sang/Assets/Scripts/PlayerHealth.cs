using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 100;
    public int currentHealth;

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
        Debug.Log("플레이어 현재 체력: " + currentHealth);

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
        Debug.Log("플레이어 사망 - 정산 진행");

        // 데이터 매니저 정산 세이브 실행
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SaveGameResult();
        }

        // GameManager를 통해 정석적으로 게임 오버 화면 요청
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            // 만약 하이어라키에 GameManager 오브젝트가 배치되지 않았다면 즉시 다이렉트 전환
            SceneManager.LoadScene("GameOver");
        }
    }
}