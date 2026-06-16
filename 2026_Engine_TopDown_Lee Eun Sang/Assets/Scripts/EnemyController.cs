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

    public float moveSpeed = 1.5f;
    public float raycastDistance = 0.4f;
    public float traceDistance = 35f;

    private Transform player;
    private Rigidbody2D rb;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        currentSprites = spriteDown;
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    private void Update()
    {
        if (player == null) return;

        Vector2 direction = player.position - transform.position;
        if (direction.magnitude > traceDistance) return;

        Vector2 directionNormalized = direction.normalized;
        UpdateDirection(directionNormalized);
        UpdateAnimation();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionNormalized, raycastDistance, LayerMask.GetMask("Obstacle"));

        Vector2 finalDirection = directionNormalized;
        if (hit.collider != null)
        {
            finalDirection = Quaternion.Euler(0f, 0f, -90f) * directionNormalized;
        }

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
                sr.flipX = (spriteLeft == spriteRight);
                ChangeSprites(spriteLeft);
            }
        }
        else
        {
            if (direction.y > 0) ChangeSprites(spriteUp);
            else ChangeSprites(spriteDown);
        }
    }

    private void ChangeSprites(Sprite[] newSprites)
    {
        if (currentSprites == newSprites) return;
        currentSprites = newSprites;
        frameIndex = 0;
        timer = 0f;
    }

    private void UpdateAnimation()
    {
        if (currentSprites == null || currentSprites.Length <= 1) return;
        timer += Time.deltaTime;
        if (timer >= frameTime)
        {
            timer = 0f;
            frameIndex = (frameIndex + 1) % currentSprites.Length;
            sr.sprite = currentSprites[frameIndex];
        }
    }
}