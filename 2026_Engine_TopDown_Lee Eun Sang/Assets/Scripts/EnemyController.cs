using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("적 애니메이션")]
    public Sprite[] spriteUp;
    public Sprite[] spriteDown;
    public Sprite[] spriteLeft;
    public Sprite[] spriteRight;
    public float frameTime = 0.15f;

    private Sprite[] currentSprites;
    private SpriteRenderer sr;
    private int frameIndex = 0;
    private float timer = 0f;

    [Header("이동 및 감지 세팅")]
    public float moveSpeed = 1.5f;       // 뱀서류에 맞게 속도를 살짝 올렸습니다.
    public float raycastDistance = 0.4f; // 장애물 감지 거리
    public float traceDistance = 35f;    // 멀리서 스폰되어도 작동하도록 넉넉하게 설정

    [Header("드롭 아이템 세팅")]
    public GameObject expGemPrefab;      // ★ 인스펙터에서 경험치 보석 프리팹을 연결해주세요!

    private Transform player;
    private Rigidbody2D rb;              // 안전한 충돌 및 우회를 위해 Rigidbody2D 활용

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        currentSprites = spriteDown;
        if (currentSprites != null && currentSprites.Length > 0)
        {
            sr.sprite = currentSprites[0];
        }
    }

    private void Start()
    {
        // "Player" 태그를 가진 오브젝트를 자동으로 찾아서 추적합니다.
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        Vector2 direction = player.position - transform.position;
        float distance = direction.magnitude;

        // 플레이어가 추적 거리보다 멀리 있다면 얼어붙지 않고 return (스포너 반지름 고려)
        if (distance > traceDistance)
        {
            return;
        }

        Vector2 directionNormalized = direction.normalized;

        // 애니메이션 및 방향 업데이트
        UpdateDirection(directionNormalized);
        UpdateAnimation();

        // [최적화] 레이캐스트 올(All) 대신 단일 레이캐스트 사용 및 Obstacle 레이어 지정
        // 이 처리를 해야 자기 자신이나 다른 적들에게 부딪혀서 속도가 배로 빨라지는 버그가 사라집니다.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionNormalized, raycastDistance, LayerMask.GetMask("Obstacle"));
        Debug.DrawRay(transform.position, directionNormalized * raycastDistance, Color.red);

        Vector2 finalDirection = directionNormalized;

        if (hit.collider != null)
        {
            // 장애물 발견 시 우측 90도 방향으로 꺾어서 이동 시도
            finalDirection = Quaternion.Euler(0f, 0f, -90f) * directionNormalized;
        }

        // [물리 최적화] transform.Translate 대신 Rigidbody의 MovePosition을 써야 적들이 벽을 뚫지 않습니다.
        if (rb != null)
        {
            Vector2 nextPosition = (Vector2)transform.position + (finalDirection * moveSpeed * Time.deltaTime);
            rb.MovePosition(nextPosition);
        }
        else
        {
            transform.Translate(finalDirection * moveSpeed * Time.deltaTime);
        }
    }

    private void UpdateDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                sr.flipX = false;
                ChangeSprites(spriteRight);
            }
            else
            {
                // spriteLeft가 따로 없다면 flipX로 좌우반전 처리
                sr.flipX = (spriteLeft == spriteRight);
                ChangeSprites(spriteLeft);
            }
        }
        else
        {
            if (direction.y > 0)
            {
                ChangeSprites(spriteUp);
            }
            else
            {
                ChangeSprites(spriteDown);
            }
        }
    }

    private void ChangeSprites(Sprite[] newSprites)
    {
        if (newSprites == null || newSprites.Length == 0 || currentSprites == newSprites)
        {
            return;
        }
        currentSprites = newSprites;
        frameIndex = 0;
        timer = 0f;
        sr.sprite = currentSprites[frameIndex];
    }

    private void UpdateAnimation()
    {
        if (currentSprites == null || currentSprites.Length <= 1) return;

        timer += Time.deltaTime;

        if (timer >= frameTime)
        {
            timer = 0f;
            frameIndex++;

            if (frameIndex >= currentSprites.Length)
            {
                frameIndex = 0;
            }
            sr.sprite = currentSprites[frameIndex];
        }
    }

    // 무기나 공격에 맞았을 때 외부에서 호출해줄 데미지/사망 함수
    public void TakeDamage()
    {
        Die();
    }

    // ★ 적이 죽을 때 보석을 생성하고 자신을 파괴하는 핵심 함수
    private void Die()
    {
        if (expGemPrefab != null)
        {
            // 적이 죽은 현재 위치에 경험치 보석을 생성합니다.
            Instantiate(expGemPrefab, transform.position, Quaternion.identity);
        }

        // 적 게임 오브젝트 삭제
        Destroy(gameObject);
    }
}