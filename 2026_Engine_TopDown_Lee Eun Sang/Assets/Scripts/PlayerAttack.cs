using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("공격 기본 설정 (새 판 시작 시 이 값으로 초기화됨)")]
    public float attackRange = 1.5f;
    public float attackCooldown = 0.5f; // 죽고 다시 시작하면 0.5초로 초기화[cite: 3]
    public int attackDamage = 10;        // 죽고 다시 시작하면 10으로 초기화[cite: 3]
    public float autoAttackDetectionRange = 2f;

    [Header("공격 이펙트")]
    public float attackDuration = 0.4f;
    public Color slashColor = new Color(1f, 1f, 1f, 0.8f);

    private float lastAttackTime = 0f;
    private PlayerController playerController; //[cite: 3]
    private GameObject slashEffectObject;   //[cite: 3]
    private LineRenderer lineRenderer; //[cite: 3]

    private void Start()
    {
        playerController = GetComponent<PlayerController>(); //[cite: 3]
        CreateSlashEffect(); //[cite: 3]

        // ★ [기존 영구 강화 자동 주입 로직 제거]
        // 이제 씬이 시작될 때 세이브 파일의 영구 수치를 강제로 더하지 않습니다.
        // 즉, 플레이어가 죽고 재시작(씬 재로드)되면 위의 기본값(10 데미지, 0.5초 쿨타임)으로 완벽하게 초기화됩니다.
    }

    private void CreateSlashEffect() //[cite: 3]
    {
        slashEffectObject = new GameObject("SlashEffect"); //[cite: 3]
        slashEffectObject.transform.SetParent(null); //[cite: 3]
        slashEffectObject.transform.position = Vector3.zero; //[cite: 3]

        lineRenderer = slashEffectObject.AddComponent<LineRenderer>(); //[cite: 3]
        lineRenderer.positionCount = 64; //[cite: 3]
        lineRenderer.startWidth = 0.1f; //[cite: 3]
        lineRenderer.endWidth = 0.1f; //[cite: 3]
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); //[cite: 3]
        lineRenderer.startColor = slashColor; //[cite: 3]
        lineRenderer.endColor = slashColor; //[cite: 3]
        lineRenderer.sortingLayerName = "Default"; //[cite: 3]
        lineRenderer.loop = true; //[cite: 3]

        slashEffectObject.SetActive(false); //[cite: 3]
    }

    private void Update() //[cite: 3]
    {
        CheckAndAutoAttack(); //[cite: 3]
    }

    private void CheckAndAutoAttack() //[cite: 3]
    {
        if (Time.time - lastAttackTime < attackCooldown) //[cite: 3]
        {
            return; //[cite: 3]
        }

        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, autoAttackDetectionRange); //[cite: 3]

        bool hasEnemyNearby = false; //[cite: 3]
        foreach (Collider2D collider in nearbyColliders) //[cite: 3]
        {
            if (collider.CompareTag("Enemy")) //[cite: 3]
            {
                hasEnemyNearby = true; //[cite: 3]
                break; //[cite: 3]
            }
        }

        if (hasEnemyNearby) //[cite: 3]
        {
            PerformAttack(); //[cite: 3]
        }
    }

    private void PerformAttack() //[cite: 3]
    {
        lastAttackTime = Time.time; //[cite: 3]

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange); //[cite: 3]

        foreach (Collider2D collider in hitColliders) //[cite: 3]
        {
            if (collider.gameObject == gameObject) //[cite: 3]
            {
                continue; //[cite: 3]
            }

            if (collider.CompareTag("Enemy")) //[cite: 3]
            {
                Debug.Log("적 히트: " + collider.gameObject.name); //[cite: 3]

                EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>(); //[cite: 3]
                if (enemyHealth != null) //[cite: 3]
                {
                    // 레벨업 카드로 강화된 현재 판의 attackDamage가 실시간으로 들어갑니다.
                    enemyHealth.TakeDamage(attackDamage); //[cite: 3]
                }
            }
        }

        StartCoroutine(ShowOmnidirectionalSlashEffect()); //[cite: 3]
    }

    private IEnumerator ShowOmnidirectionalSlashEffect() //[cite: 3]
    {
        if (slashEffectObject == null || lineRenderer == null) //[cite: 3]
            yield break; //[cite: 3]

        slashEffectObject.SetActive(true); //[cite: 3]
        Vector3 playerPos = transform.position; //[cite: 3]
        slashEffectObject.transform.position = playerPos; //[cite: 3]

        float elapsedTime = 0f; //[cite: 3]

        while (elapsedTime < attackDuration) //[cite: 3]
        {
            elapsedTime += Time.deltaTime; //[cite: 3]
            float progress = elapsedTime / attackDuration; //[cite: 3]

            float currentRadius = Mathf.Lerp(0.2f, attackRange, progress); //[cite: 3]

            int lineCount = 64; //[cite: 3]
            for (int i = 0; i < lineCount; i++) //[cite: 3]
            {
                float angle = (i / (float)lineCount) * 360f * Mathf.Deg2Rad; //[cite: 3]
                float x = Mathf.Cos(angle) * currentRadius; //[cite: 3]
                float y = Mathf.Sin(angle) * currentRadius; //[cite: 3]
                lineRenderer.SetPosition(i, playerPos + new Vector3(x, y, 0)); //[cite: 3]
            }

            float alpha = 1f - (progress * progress); //[cite: 3]
            Color color = new Color(slashColor.r, slashColor.g, slashColor.b, alpha * slashColor.a); //[cite: 3]

            lineRenderer.startColor = color; //[cite: 3]
            lineRenderer.endColor = color; //[cite: 3]

            yield return null; //[cite: 3]
        }

        slashEffectObject.SetActive(false); //[cite: 3]
    }

    private void OnDrawGizmosSelected() //[cite: 3]
    {
        Gizmos.color = Color.red; //[cite: 3]
        Gizmos.DrawWireSphere(transform.position, attackRange); //[cite: 3]
        Gizmos.color = Color.yellow; //[cite: 3]
        Gizmos.DrawWireSphere(transform.position, autoAttackDetectionRange); //[cite: 3]
    }

    private void OnDestroy() //[cite: 3]
    {
        if (slashEffectObject != null) //[cite: 3]
        {
            Destroy(slashEffectObject); //[cite: 3]
        }
    }
}