using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHp = 30;
    private int currentHp;

    [Header("피격 연출 (빨개짐)")]
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;

    [Header("드롭 아이템 세팅")]
    public GameObject expGemPrefab;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine hitCoroutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    void Start()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (hitCoroutine != null) StopCoroutine(hitCoroutine);
        if (gameObject.activeInHierarchy && spriteRenderer != null)
        {
            hitCoroutine = StartCoroutine(FlashHitColor());
        }
        if (currentHp <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashHitColor()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        if (expGemPrefab != null) Instantiate(expGemPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);

        if (InGameHUDManager.instance != null)
        {
            InGameHUDManager.instance.AddKillCount(1);
        }
    }
}