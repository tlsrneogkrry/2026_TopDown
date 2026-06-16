using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("공격 기본 설정 (새 게임 시 초기화용)")]
    public float attackRange = 1.5f;
    public float attackCooldown = 0.5f;
    public int attackDamage = 10;
    public float autoAttackDetectionRange = 2f;

    [Header("공격 이펙트")]
    public float attackDuration = 0.4f;
    public Color slashColor = new Color(1f, 1f, 1f, 0.8f);

    private float lastAttackTime = 0f;
    private GameObject slashEffectObject;
    private LineRenderer lineRenderer;

    // ★ UpgradeCard가 보낸 강화 메시지를 수신하는 통로 함수들
    public void UpgradeDamage(int amount)
    {
        attackDamage += amount;
        Debug.Log($"[능력치 상승] 현재 인게임 공격력: {attackDamage}");
    }

    public void UpgradeCooldown(float amount)
    {
        attackCooldown = Mathf.Max(0.1f, attackCooldown - amount);
        Debug.Log($"[능력치 상승] 현재 인게임 공격 쿨타임: {attackCooldown}");
    }

    public void UpgradeMaxHealth(float amount)
    {
        Debug.Log($"[능력치 상승] 체력 증가 메시지 수신: {amount}");
    }

    private void Start()
    {
        CreateSlashEffect();
    }

    private void CreateSlashEffect()
    {
        slashEffectObject = new GameObject("SlashEffect");
        slashEffectObject.transform.SetParent(null);
        slashEffectObject.transform.position = Vector3.zero;

        lineRenderer = slashEffectObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 64;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = slashColor;
        lineRenderer.endColor = slashColor;
        lineRenderer.sortingLayerName = "Default";
        lineRenderer.loop = true;

        slashEffectObject.SetActive(false);
    }

    private void Update()
    {
        CheckAndAutoAttack();
    }

    private void CheckAndAutoAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, autoAttackDetectionRange);
        bool hasEnemyNearby = false;
        foreach (Collider2D collider in nearbyColliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                hasEnemyNearby = true;
                break;
            }
        }

        if (hasEnemyNearby) PerformAttack();
    }

    private void PerformAttack()
    {
        lastAttackTime = Time.time;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.gameObject == gameObject) continue;

            if (collider.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(attackDamage);
                }
            }
        }
        StartCoroutine(ShowOmnidirectionalSlashEffect());
    }

    private IEnumerator ShowOmnidirectionalSlashEffect()
    {
        if (slashEffectObject == null || lineRenderer == null) yield break;

        slashEffectObject.SetActive(true);
        Vector3 playerPos = transform.position;
        slashEffectObject.transform.position = playerPos;

        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / attackDuration;
            float currentRadius = Mathf.Lerp(0.2f, attackRange, progress);

            int lineCount = 64;
            for (int i = 0; i < lineCount; i++)
            {
                float angle = (i / (float)lineCount) * 360f * Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * currentRadius;
                float y = Mathf.Sin(angle) * currentRadius;
                lineRenderer.SetPosition(i, playerPos + new Vector3(x, y, 0));
            }

            float alpha = 1f - (progress * progress);
            Color color = new Color(slashColor.r, slashColor.g, slashColor.b, alpha * slashColor.a);
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            yield return null;
        }
        slashEffectObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, autoAttackDetectionRange);
    }

    private void OnDestroy()
    {
        if (slashEffectObject != null) Destroy(slashEffectObject);
    }
}