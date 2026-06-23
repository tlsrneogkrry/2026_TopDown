using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("스크립터블 오브젝트 데이터 (★과제 필수 조건)")]
    [SerializeField] private StageWaveData currentStageData; // 여기에 만든 SO 데이터를 꽂습니다.

    [Header("Target")]
    [SerializeField] private Transform playerTransform;

    [Header("Spawn Zone (Radius)")]
    [SerializeField] private float minRadius = 7f;
    [SerializeField] private float maxRadius = 11f;

    private int currentWaveIndex = 0;
    private float spawnTimer;
    private float waveTimer; // 1분 세는 타이머

    void Start()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null || currentStageData == null) return;
        if (currentWaveIndex >= currentStageData.waveList.Count) return; // 모든 웨이브 종료 시 안전장치

        // 1. 시간 흐름에 따른 웨이브 자동 전환 로직 (뱀서라이크 핵심)
        waveTimer += Time.deltaTime;
        if (waveTimer >= 60f) // 60초(1분)마다 다음 웨이브로 자동 변경
        {
            waveTimer = 0f;
            currentWaveIndex++;
            Debug.LogWarning($"[웨이브 전환] 다음 웨이브로 변경되었습니다! 현재 인덱스: {currentWaveIndex}");

            if (currentWaveIndex >= currentStageData.waveList.Count) return;
        }

        // 2. 현재 웨이브의 소환 주기에 맞춰 적 젠하기
        StageWaveData.EnemySpawnInfo currentWave = currentStageData.waveList[currentWaveIndex];
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= currentWave.spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemy(currentWave);
        }
    }

    void SpawnEnemy(StageWaveData.EnemySpawnInfo wave)
    {
        if (wave.enemyPrefabs.Count == 0) return;

        Vector2 spawnPosition = GetRandomSpawnPosition();
        int randomIndex = Random.Range(0, wave.enemyPrefabs.Count);
        GameObject enemyPrefab = wave.enemyPrefabs[randomIndex];

        if (enemyPrefab != null)
        {
            GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            // ★ 만약 데이터 상에 'isBossWave'가 체크되어 있다면 보스 전용 처리 진행
            if (wave.isBossWave)
            {
                // 보스는 체력을 엄청나게 늘려주거나 스케일을 키워줍니다.
                EnemyHealth health = spawnedEnemy.GetComponent<EnemyHealth>();
                if (health != null)
                {
                    health.maxHp *= 5; // 보스는 일반 몹 체력의 5배!
                    // 나중에 보스가 죽을 때 상자 주게 하려면 여기서 보스 태그를 달아주거나 컴포넌트로 제어합니다.
                }
                spawnedEnemy.transform.localScale *= 2f; // 덩치도 2배!
            }
        }
    }

    Vector2 GetRandomSpawnPosition()
    {
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float randomRadius = Random.Range(minRadius, maxRadius);

        float spawnX = playerTransform.position.x + Mathf.Cos(randomAngle) * randomRadius;
        float spawnY = playerTransform.position.y + Mathf.Sin(randomAngle) * randomRadius;

        return new Vector2(spawnX, spawnY);
    }
}