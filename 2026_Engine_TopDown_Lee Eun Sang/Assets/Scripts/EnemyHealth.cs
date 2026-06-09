using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 30;
    private int currentHealth;

    [Header("피격 효과")]
    public Color hitColor = new Color(1f, 0.5f, 0.5f, 1f); // 연빨강
    public float hitEffectDuration = 0.15f;

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
        currentHealth -= damage;
        Debug.Log("적 체력: " + currentHealth);

        // 피격 효과
        StartCoroutine(HitEffect());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator HitEffect()
    {
        // 피격 색상으로 변경
        sr.color = hitColor;
        yield return new WaitForSeconds(hitEffectDuration);
        // 원래 색상으로 복원
        sr.color = originalColor;
    }

    private void Die()
    {
        Debug.Log("적 처치: " + gameObject.name);
        Destroy(gameObject);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}