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

    private bool isAttacking = false;

    // 성기사의 검 업그레이드 함수
    public void UpgradeDamage(int amount)
    {
        attackDamage += amount;
        Debug.Log($"[{gameObject.name}] 현재 인게임 공격력: {attackDamage}");

        // 지팡이 효과로 늘어난 복사본 스크립트들의 데미지도 일괄 동기화합니다.
        PlayerAttack[] allAttacks = GetComponents<PlayerAttack>();
        foreach (PlayerAttack attack in allAttacks)
        {
            attack.attackDamage = this.attackDamage;
        }
    }

    // 마법사의 지팡이 업그레이드 함수 (독립 컴포넌트 추가 기믹)
    public void UpgradeAttackCount(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            PlayerAttack additionalAttack = gameObject.AddComponent<PlayerAttack>();

            additionalAttack.attackDamage = this.attackDamage;
            additionalAttack.attackCooldown = this.attackCooldown;
            additionalAttack.attackRange = this.attackRange;
            additionalAttack.autoAttackDetectionRange = this.autoAttackDetectionRange;

            Debug.LogWarning($"[아이템 효과] 플레이어에게 독립 공격 스크립트가 추가 장착되었습니다!");
        }
    }

    public void UpgradeCooldown(float amount)
    {
        attackCooldown = Mathf.Max(0.1f, attackCooldown - amount);
        Debug.Log($"[능력치 상승] 현재 인게임 공격 쿨타임: {attackCooldown}");

        PlayerAttack[] allAttacks = GetComponents<PlayerAttack>();
        foreach (PlayerAttack attack in allAttacks)
        {
            attack.attackCooldown = this.attackCooldown;
        }
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
        if (isAttacking) return;
        if (Time.time - lastAttackTime < attackCooldown) return;

        CheckAndAutoAttack();
    }

    private void CheckAndAutoAttack()
    {
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

        if (hasEnemyNearby)
        {
            PerformAttack();
        }

        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlaySFX(SoundManager.instance.attackSound);
        }
    }

    private void PerformAttack()
    {
        isAttacking = true;
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
        if (slashEffectObject == null || lineRenderer == null)
        {
            isAttacking = false;
            yield break;
        }

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
        isAttacking = false;
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