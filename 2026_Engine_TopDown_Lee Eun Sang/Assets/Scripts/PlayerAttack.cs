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

    // ★ [버그 해결 핵심] 파동 코루틴이 중복 실행되는 것을 원천 차단하는 안전장치
    private bool isAttacking = false;

    // 성기사의 검을 먹었을 때 데미지를 올리는 함수
    public void UpgradeDamage(int amount)
    {
        // 1. 먼저 내 스크립트의 데미지를 올립니다.
        attackDamage += amount;
        Debug.Log($"[{gameObject.name}] 현재 인게임 공격력: {attackDamage}");

        // 2. 만약 지팡이 효과로 스크립트가 여러 개 복사되어 있다면, 다른 모든 PlayerAttack들의 데미지도 똑같이 올려줍니다!
        PlayerAttack[] allAttacks = GetComponents<PlayerAttack>();
        foreach (PlayerAttack attack in allAttacks)
        {
            attack.attackDamage = this.attackDamage;
        }
    }

    // ★ [마법사의 지팡이용 함수 추가 완료!] 새 게임 초기화용 변수는 절대 건들지 않습니다.
    public void UpgradeAttackCount(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            // 플레이어 자신(gameObject)에게 새로운 PlayerAttack 스크립트 컴포넌트를 하나 더 추가 장착합니다!
            PlayerAttack additionalAttack = gameObject.AddComponent<PlayerAttack>();

            // 새로 태어난 복사본 스크립트에게 현재까지 업그레이드된 능력치(데미지, 쿨타임 등)를 그대로 동기화해 줍니다.
            additionalAttack.attackDamage = this.attackDamage;
            additionalAttack.attackCooldown = this.attackCooldown;
            additionalAttack.attackRange = this.attackRange;
            additionalAttack.autoAttackDetectionRange = this.autoAttackDetectionRange;

            Debug.LogWarning($"[아이템 효과] 플레이어에게 독립적인 공격 스크립트가 추가 장착되었습니다! (총 공격 횟수 추가)");
        }
    }

    public void UpgradeCooldown(float amount)
    {
        attackCooldown = Mathf.Max(0.1f, attackCooldown - amount);
        Debug.Log($"[능력치 상승] 현재 인게임 공격 쿨타임: {attackCooldown}");

        // 쿨타임 감소도 마찬가지로 늘어난 모든 지팡이에 동기화해 줍니다.
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
        // ★ 1. 이미 파동을 발사 중이거나 쿨타임이 지나지 않았다면 절대 다음 연산을 하지 못하게 막습니다.
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
    }

    private void PerformAttack()
    {
        // ★ 2. PerformAttack에 들어오자마자 가장 먼저 문을 잠궈서 
        // 다음 프레임의 Update가 이 함수를 또 실행시키는 것을 완벽히 방지합니다.
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

        // ★ 3. 문을 잠근 상태에서 안전하게 코루틴을 "딱 1번만" 실행합니다.
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

        // ★ 4. 파동 연출이 눈앞에서 완전히 사라진 뒤에 문을 열어줍니다.
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