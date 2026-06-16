using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHp = 30; // 3대 맞아야 하므로 (공격력 10 x 3 = 30)
    private int currentHp;

    [Header("피격 연출 (빨개짐)")]
    public Color hitColor = Color.red;     // 맞았을 때 변할 색상
    public float flashDuration = 0.1f;    // 빨갛게 유지될 시간 (초)

    [Header("드롭 아이템 세팅")]
    public GameObject expGemPrefab;       // 인스펙터에서 경험치 보석 프리팹을 꼭 넣어주세요!

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine hitCoroutine;

    void Awake()
    {
        // 색상을 변경하기 위해 SpriteRenderer 컴포넌트를 가져옵니다.
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // 적의 원래 기본 색상 저장
        }
    }

    void Start()
    {
        currentHp = maxHp;
    }

    // PlayerAttack에서 호출하는 데미지 함수
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        Debug.Log($"{gameObject.name}이(가) {damage}의 데미지를 받음. 남은 HP: {currentHp}");

        // ★ [피격 연출 실행] 이미 돌고 있는 피격 코루틴이 있다면 겹치지 않게 끄고 새로 시작
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }

        if (gameObject.activeInHierarchy && spriteRenderer != null)
        {
            hitCoroutine = StartCoroutine(FlashHitColor());
        }

        // 체력이 0 이하면 사망
        if (currentHp <= 0)
        {
            Die();
        }
    }

    // ★ 잠깐 빨개졌다가 원래 색으로 돌아오는 코루틴 함수
    private IEnumerator FlashHitColor()
    {
        spriteRenderer.color = hitColor; // 빨간색으로 변경

        yield return new WaitForSeconds(flashDuration); // 0.1초 대기

        spriteRenderer.color = originalColor; // 원래 색상으로 복구
    }

    private void Die()
    {
        // 적이 진짜 체력이 다 닳아 죽는 순간에 딱 한 번 보석을 떨굽니다.
        if (expGemPrefab != null)
        {
            Instantiate(expGemPrefab, transform.position, Quaternion.identity);
        }

        // 적 오브젝트 파괴
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        // 오브젝트가 없어지거나 비활성화될 때 코루틴 안전하게 정리
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }
    }
}