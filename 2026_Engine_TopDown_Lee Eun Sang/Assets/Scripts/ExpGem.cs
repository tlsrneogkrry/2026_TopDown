using UnityEngine;

public class ExpGem : MonoBehaviour
{
    [Header("경험치 및 흡입 설정")]
    public int expValue = 1;      // 이 보석이 줄 경험치 양
    public float moveSpeed = 5f;  // 플레이어에게 끌려오는 시작 속도

    private Transform playerTransform;
    private bool isFlying = false; // 플레이어에게 끌려가는 중인지 여부

    void Update()
    {
        // 플레이어 자석에 걸려서 쫓아가는 상태라면
        if (isFlying && playerTransform != null)
        {
            // 플레이어의 중심 위치로 부드럽게 이동
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);

            // 속도가 매 프레임 조금씩 빨라지게 하여 찰진 흡입감 연출
            moveSpeed += Time.deltaTime * 7f;
        }
    }

    // 플레이어의 자석 범위(OverlapCircle)에 감지되었을 때 레벨 매니저가 호출해줄 함수
    public void StartFly(Transform target)
    {
        playerTransform = target;
        isFlying = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어 본체(Collider)와 충돌했을 때만 경험치를 주고 파괴됨
        if (collision.CompareTag("Player"))
        {
            PlayerLevelManager levelManager = collision.GetComponent<PlayerLevelManager>();
            if (levelManager != null)
            {
                levelManager.GetExp(expValue);
            }

            // 획득 성공 후 보석 삭제
            Destroy(gameObject);
        }
    }
}