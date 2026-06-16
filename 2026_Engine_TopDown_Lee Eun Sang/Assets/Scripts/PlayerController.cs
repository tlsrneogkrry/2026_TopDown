using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Sprite[] spriteUp;
    public Sprite[] spriteDown;
    public Sprite[] spriteLeft;
    public Sprite[] spriteRight;
    
    public float frameTime = 0.15f;
    
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 input;
    private Vector2 velocity;
    private Sprite[] currentSprites;
    private Sprite[] currentMovementSprites;
    private int frameIndex = 0;
    private float timer = 0f;
    private PlayerHealth playerHealth;
    
    private Vector2 lastDirection = Vector2.down;
    private Camera mainCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        playerHealth = GetComponent<PlayerHealth>();
        mainCamera = Camera.main;

        currentSprites = spriteDown;
        currentMovementSprites = spriteDown;
        sr.sprite = currentSprites[0];
    }

    private void Update()
    {
        // ?¥Îèô Ï≤òÎ¶¨
        if (input.sqrMagnitude <= 0.01f)
        {
            frameIndex = 0;
            sr.sprite = currentSprites[frameIndex];
            UpdateSortingOrder();
            return;
        }

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

        UpdateSortingOrder();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    private void ChangeSprites(Sprite[] newSprites)
    {
        if (currentSprites == newSprites)
        {
            return;
        }
        currentSprites = newSprites;
        currentMovementSprites = newSprites;
        frameIndex = 0;
        timer = 0f;
        sr.sprite = currentSprites[frameIndex];
    }

    public void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
        velocity = input.normalized * moveSpeed;

        if (input.sqrMagnitude > 0.01f)
        {
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                if (input.x > 0)
                {
                    lastDirection = Vector2.right;
                    sr.flipX = false;
                    ChangeSprites(spriteRight);
                }
                else
                {
                    lastDirection = Vector2.left;
                    sr.flipX = true;
                    ChangeSprites(spriteLeft);
                }
            }
            else
            {
                if (input.y > 0)
                {
                    lastDirection = Vector2.up;
                    ChangeSprites(spriteUp);
                }
                else
                {
                    lastDirection = Vector2.down;
                    ChangeSprites(spriteDown);
                }
            }
        }
    }

    private void UpdateSortingOrder()
    {
        // Y ¡¬«•¿« ¿Ωºˆ∞™¿ª Sorting Order∑Œ º≥¡§
        // Y∞° ≥Ù¿ªºˆ∑œ æ’ø°, Y∞° ≥∑¿ªºˆ∑œ µ⁄ø° ∑ª¥ı∏µµ 
        sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100f);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // ?ÅÍ≥º??Ï∂©Îèå Í∞êÏ? (Îß??ÑÎ†à??Ï≤¥ÌÅ¨)
        if (collision.CompareTag("Enemy"))
        {
            // Î¨¥Ï†Å ?ÅÌÉúÍ∞Ä ?ÑÎãàÎ©??∞Î?ÏßÄ Î∞õÏùå
            if (playerHealth != null && !playerHealth.IsInvincible())
            {
                playerHealth.TakeDamage(10);
            }
        }
    }
}
