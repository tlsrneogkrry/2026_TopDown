using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("스크립터블 오브젝트 데이터")]
    [SerializeField] private StageWaveData currentStageData;

    [Header("Target")]
    [SerializeField] private Transform playerTransform;

    [Header("Spawn Zone (Radius)")]
    [SerializeField] private float minRadius = 7f;
    [SerializeField] private float maxRadius = 11f;

    [Header("보스 스폰 설정")]
    [SerializeField] private GameObject bossPrefab;

    // ★ [유저님 요청 기믹] 최대 몬스터 마리 수 제한 설정 칸입니다.
    [Header("최대 몬스터 수 제한 설정")]
    [SerializeField] private int maxEnemyCount = 100; // 인스펙터에서 이 숫자를 바꾸면 최대 마리 수가 제한됩니다!

    private int currentWaveIndex = 0;
    private float spawnTimer;

    private float totalPlayTimer = 0f;
    private float waveDurationTimer = 0f;
    private float lastBossSpawnTime = 0f;

    private int waveLoopCount = 0;
    private int hpBonusAmount = 0;         // 누적된 체력 증가량 (라운드당 80씩 증가)

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
        if (currentStageData.waveList.Count == 0) return;

        totalPlayTimer += Time.deltaTime;
        waveDurationTimer += Time.deltaTime;

        // 1. 2분(120초)마다 고정 보스 기습 스폰
        if (totalPlayTimer - lastBossSpawnTime >= 120f)
        {
            lastBossSpawnTime = totalPlayTimer;
            SpawnAbsoluteBoss();
        }

        // 2. 1분(60초)마다 웨이브 전환 및 1~2번 무한 반복 연산
        if (waveDurationTimer >= 60f)
        {
            waveDurationTimer = 0f;
            currentWaveIndex++;

            if (currentWaveIndex >= currentStageData.waveList.Count)
            {
                currentWaveIndex = 0;
                waveLoopCount++;
                hpBonusAmount = waveLoopCount * 80; // 세트 리셋 시 피 80 증가
                Debug.LogWarning($"♻️ [웨이브 대반복] 다시 1번 웨이브 시작! (현재 몬스터 추가 HP: +{hpBonusAmount})");
            }
            else
            {
                int totalSteps = (waveLoopCount * currentStageData.waveList.Count) + currentWaveIndex;
                hpBonusAmount = totalSteps * 80; // 단계 넘어갈 때마다 피 80 증가
                Debug.LogWarning($"[웨이브 단계 전환] 현재 웨이브 인덱스: {currentWaveIndex} (현재 몬스터 추가 HP: +{hpBonusAmount})");
            }
        }

        // 3. 현재 뺑뺑이 돌고 있는 웨이브 주기에 맞춰 일반 적 스폰
        StageWaveData.EnemySpawnInfo currentWave = currentStageData.waveList[currentWaveIndex];
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= currentWave.spawnInterval)
        {
            spawnTimer = 0f;

            // ★ [최대 몬스터 수 체크 최적화 방어선]
            // 현재 맵에 살아있는 "Enemy" 태그를 가진 몬스터 개수를 카운트합니다.
            int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

            // 만약 현재 마리 수가 인스펙터에 지정한 최대 마리 수보다 크거나 같으면 소환을 잠시 Skip합니다.
            if (currentEnemyCount < maxEnemyCount)
            {
                SpawnNormalEnemy(currentWave);
            }
        }
    }

    // 일반 몬스터 스폰 (체력 루프 누적 반영)
    void SpawnNormalEnemy(StageWaveData.EnemySpawnInfo wave)
    {
        if (wave.enemyPrefabs.Count == 0) return;

        Vector2 spawnPosition = GetRandomSpawnPosition();
        int randomIndex = Random.Range(0, wave.enemyPrefabs.Count);
        GameObject enemyPrefab = wave.enemyPrefabs[randomIndex];

        if (enemyPrefab != null)
        {
            GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            EnemyHealth health = spawnedEnemy.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.maxHp += hpBonusAmount;
            }
        }
    }

    // ★ [보스 스폰 기믹 수정] 일반 몹처럼 보스도 웨이브가 넘어갈 때마다 체력이 누적 증가합니다!
    void SpawnAbsoluteBoss()
    {
        if (bossPrefab == null) return;

        Vector2 spawnPosition = GetRandomSpawnPosition();
        GameObject spawnedBoss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);

        EnemyHealth bossHealth = spawnedBoss.GetComponent<EnemyHealth>();
        if (bossHealth != null)
        {
            // 기본 보스 체력(예: 500)에 일반 몹과 똑같이 쌓인 hpBonusAmount를 더해줍니다.
            // 보스답게 더 단단하게 만들고 싶다면 (hpBonusAmount * 2) 등으로 증폭하셔도 좋습니다!
            bossHealth.maxHp = 500 + hpBonusAmount;

            Debug.LogError($"🚨 [보스 출현] 현재 웨이브 버프 반영 보스 체력: {bossHealth.maxHp} (기본 500 + 추가 {hpBonusAmount})");
        }

        spawnedBoss.transform.localScale *= 2f;
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