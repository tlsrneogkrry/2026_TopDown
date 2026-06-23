using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    [Header("보스 데이터 에셋")]
    public BossData bossData;

    private EnemyHealth enemyHealth;
    private EnemyController enemyController;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyController = GetComponent<EnemyController>();
    }

    private void Start()
    {
        if (bossData != null)
        {
            // 1. 스크립터블 오브젝트에 적힌 데이터대로 인게임 스탯을 강제 세팅합니다.
            if (enemyHealth != null)
            {
                enemyHealth.maxHp = bossData.maxHp;
            }

            if (enemyController != null)
            {
                enemyController.moveSpeed = bossData.moveSpeed;
            }
        }
    }

    // ★ 유저님의 EnemyHealth.cs 등에서 죽을 때(Die) 이 함수를 호출하게 만들거나,
    // OnDestroy() 또는 OnDisable() 시점에 보물상자를 소환합니다.
    private void OnDestroy()
    {
        // 게임이 꺼지거나 씬이 바뀔 때 소환되는 것을 방지하는 안전장치
        if (!gameObject.scene.isLoaded) return;

        if (bossData != null && bossData.treasureChestPrefab != null)
        {
            // 보스가 죽은 바로 그 자리에 보물상자를 생성합니다.
            Instantiate(bossData.treasureChestPrefab, transform.position, Quaternion.identity);
            Debug.LogWarning($"[보스 처치] {bossData.bossName} 처치 완료! 보물상자가 드롭되었습니다.");
        }
    }
}