using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("공격 설정")]
    public float attackRange = 1.5f;
    public float attackCooldown = 0.5f;
    public int attackDamage = 10;
    
    [Header("공격 이펙트")]
    public float attackDuration = 0.4f;
    public Color slashColor = new Color(1f, 1f, 1f, 0.8f); // 흰색
    public int trailSegments = 20; // 잔상 개수
    
    private float lastAttackTime = 0f;
    private PlayerController playerController;
    private GameObject slashEffectObject;
    private TrailRenderer slashTrail;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        CreateSlashEffect();
    }

    private void CreateSlashEffect()
    {
        // 칼 휘두르는 이펙트 GameObject 생성 (캐릭터의 자식으로 설정)
        slashEffectObject = new GameObject("SlashEffect");
        slashEffectObject.transform.SetParent(transform); // 캐릭터를 부모로 설정
        slashEffectObject.transform.localPosition = Vector3.zero;
        
        // TrailRenderer 추가 (잔상 효과)
        slashTrail = slashEffectObject.AddComponent<TrailRenderer>();
        slashTrail.time = attackDuration; // 잔상 유지 시간
        slashTrail.startWidth = 0.3f;
        slashTrail.endWidth = 0.05f;
        slashTrail.material = new Material(Shader.Find("Sprites/Default"));
        slashTrail.startColor = slashColor;
        slashTrail.endColor = new Color(1f, 1f, 1f, 0f);
        slashTrail.sortingLayerName = "Default";
        
        // LineRenderer 추가 (칼날 표시)
        LineRenderer lineRenderer = slashEffectObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.15f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(1f, 1f, 1f, 0.9f);
        lineRenderer.endColor = new Color(1f, 1f, 1f, 0.5f);
        lineRenderer.sortingLayerName = "Default";
        
        // 처음에는 비활성화
        slashEffectObject.SetActive(false);
    }

    private void Update()
    {
        // Update에서는 아무것도 하지 않음
    }

    private void LateUpdate()
    {
        // LateUpdate에서 Input System 체크
        UnityEngine.InputSystem.Mouse mouse = UnityEngine.InputSystem.Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            Attack();
        }
    }

    private void Attack()
    {
        // 공격 쿨타임 체크
        if (Time.time - lastAttackTime < attackCooldown)
        {
            return;
        }

        lastAttackTime = Time.time;

        // 플레이어가 바라보는 방향 가져오기
        Vector2 attackDirection = GetFacingDirection();
        
        // 범위 내의 모든 콜라이더 감지
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (Collider2D collider in hitColliders)
        {
            // 자신은 제외
            if (collider.gameObject == gameObject)
            {
                continue;
            }

            // 적 태그 확인
            if (collider.CompareTag("Enemy"))
            {
                Debug.Log("적 히트: " + collider.gameObject.name);
                
                // 적에게 데미지 주기
                EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(attackDamage);
                }
            }
        }

        // 칼 휘두르는 이펙트 표시
        StartCoroutine(ShowSlashEffect(attackDirection));
        
        Debug.Log("공격! 방향: " + attackDirection);
    }

    private IEnumerator ShowSlashEffect(Vector2 direction)
    {
        if (slashEffectObject == null)
            yield break;

        slashEffectObject.SetActive(true);

        // 방향에 따른 기본 각도 계산
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // 애니메이션 진행도
        float elapsedTime = 0f;
        LineRenderer lineRenderer = slashEffectObject.GetComponent<LineRenderer>();
        
        while (elapsedTime < attackDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / attackDuration; // 0 ~ 1

            // 왼쪽(-45도)에서 오른쪽(+45도)으로 휘두르는 각도
            float slashStartAngle = baseAngle - 45f;
            float slashEndAngle = baseAngle + 45f;
            float currentAngle = Mathf.Lerp(slashStartAngle, slashEndAngle, progress);

            // 칼날이 회전하면서 움직임
            float angleRad = currentAngle * Mathf.Deg2Rad;
            Vector3 slashDirection = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0);
            
            // 칼날 시작점과 끝점 (캐릭터 위치 기준)
            Vector3 playerPos = transform.position;
            Vector3 startPos = playerPos + slashDirection * 0.3f;
            Vector3 endPos = playerPos + slashDirection * attackRange;

            // LineRenderer 위치 업데이트 (월드 좌표)
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);

            // 진행도에 따라 투명도 조정
            float alpha = 1f - (progress * progress); // 빠르게 투명해짐
            Color startColor = new Color(1f, 1f, 1f, alpha * 0.9f);
            Color endColor = new Color(1f, 1f, 1f, alpha * 0.3f);
            
            lineRenderer.startColor = startColor;
            lineRenderer.endColor = endColor;

            yield return null;
        }

        slashEffectObject.SetActive(false);
    }

    private Vector2 GetFacingDirection()
    {
        UnityEngine.InputSystem.Mouse mouse = UnityEngine.InputSystem.Mouse.current;
        if (mouse == null || Camera.main == null)
        {
            return Vector2.down;
        }

        Vector2 mousePos = mouse.position.ReadValue();
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 directionToMouse = (new Vector2(worldMousePos.x, worldMousePos.y) - (Vector2)transform.position).normalized;
        
        return directionToMouse.magnitude > 0 ? directionToMouse : Vector2.down;
    }

    // 공격 범위 시각화 (에디터에서만 표시)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void OnDestroy()
    {
        if (slashEffectObject != null)
        {
            Destroy(slashEffectObject);
        }
    }
}