using UnityEngine;

public class ExpGem : MonoBehaviour
{
    [Header("자석 및 경험치 설정")]
    private int expAmount = 1; // 정확히 1만 배달하도록 코드 고정
    [SerializeField] private float magnetRadius = 1f;
    [SerializeField] private float moveSpeed = 8f;

    private Transform playerTransform;
    private bool isAttracted = false;

    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    private void Update()
    {
        if (playerTransform == null) return;

        if (!isAttracted)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);
            if (distance <= magnetRadius) isAttracted = true;
        }

        if (isAttracted)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerLevelManager levelManager = collision.GetComponent<PlayerLevelManager>();
            if (levelManager != null)
            {
                levelManager.AddExp(expAmount); // 1 전달
            }
            Destroy(gameObject);

            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlaySFX(SoundManager.instance.expGetSound);
            }
        }
    }
}