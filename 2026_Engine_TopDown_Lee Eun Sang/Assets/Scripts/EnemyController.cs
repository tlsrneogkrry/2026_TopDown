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

    public float moveSpeed = 0.5f;
    public float raycastDistance = 0.2f;
    public float traceDistance = 2f;

    private Transform player;
    private Vector2 currentDirection = Vector2.down;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        currentSprites = spriteDown;
        sr.sprite = currentSprites[0];
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        Vector2 direction = player.position - transform.position;

        if (direction.magnitude > traceDistance)
        {
            return;
        }

        Vector2 directionNormalized = direction.normalized;
        currentDirection = directionNormalized;

        // 방향에 따라 스프라이트 변경
        UpdateDirection(directionNormalized);

        // 애니메이션 업데이트
        UpdateAnimation();

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionNormalized, raycastDistance);
        Debug.DrawRay(transform.position, directionNormalized * raycastDistance, Color.red);

        foreach (RaycastHit2D rHit in hits)
        {
            if (rHit.collider != null && rHit.collider.CompareTag("Obstacle"))
            {
                Vector3 alternativeDirection = Quaternion.Euler(0f, 0f, -90f) * direction;
                transform.Translate(alternativeDirection.normalized * moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(directionNormalized * moveSpeed * Time.deltaTime);
            }
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
                sr.flipX = true;
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
        if (currentSprites == newSprites)
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

    public void TakeDamage(int damage)
    {
        // TODO: 적의 체력 감소 및 사망 처리 로직 구현
        // 예시: health -= damage;
        // if (health <= 0) { Die(); }
    }
}
