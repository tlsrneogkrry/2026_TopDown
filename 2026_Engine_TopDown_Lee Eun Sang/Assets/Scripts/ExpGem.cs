using UnityEngine;

public class ExpGem : MonoBehaviour
{
    [Header("경험치 및 흡입 설정")]
    public int expValue = 1;
    public float moveSpeed = 5f;

    private Transform playerTransform;
    private bool isFlying = false;

    private void Awake()
    {
        transform.localScale = new Vector3(0.025f, 0.025f, 1f);
    }

    void Update()
    {
        if (isFlying && playerTransform != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
            moveSpeed += Time.deltaTime * 7f;
        }
    }

    public void StartFly(Transform target)
    {
        playerTransform = target;
        isFlying = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerLevelManager levelManager = collision.GetComponent<PlayerLevelManager>();
            if (levelManager != null)
            {
                levelManager.GetExp(expValue);
            }
            Destroy(gameObject);
        }
    }
}