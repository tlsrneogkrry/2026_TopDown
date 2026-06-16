using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // 인스펙터 편집을 위한 클래스 (기존 방식 유지)
    [System.Serializable]
    public class WaveData
    {
        public string waveName;
        public List<GameObject> enemyPrefabs; // 인스펙터에서 프리팹 등록용
        public float spawnInterval = 1.0f;    // 생성 주기 (초)
        public int maxEnemiesInWave = 100;    // 이 웨이브의 최대 동시 적 수

        // [추가] 이 인스펙터 데이터를 GameDateManager의 저장용 구조(WaveSaveData)로 변환해주는 메서드
        public WaveSaveData ToSaveData()
        {
            WaveSaveData saveData = new WaveSaveData
            {
                waveName = this.waveName,
                spawnInterval = this.spawnInterval,
                maxEnemiesInWave = this.maxEnemiesInWave
            };

            foreach (var prefab in enemyPrefabs)
            {
                if (prefab != null)
                {
                    saveData.enemyIDs.Add(prefab.name); // 프리팹의 이름을 ID로 저장
                }
            }
            return saveData;
        }
    }

    [Header("Target")]
    [SerializeField] private Transform playerTransform; // 플레이어 위치

    [Header("Spawn Zone (Radius)")]
    [SerializeField] private float minRadius = 12f; // 플레이어로부터의 최소 거리 (화면 밖)
    [SerializeField] private float maxRadius = 18f; // 플레이어로부터의 최대 거리

    [Header("Wave Settings")]
    [SerializeField] private List<WaveData> waves;

    private int currentWaveIndex = 0;
    private float spawnTimer;

    void Start()
    {
        // 인스펙터에서 플레이어를 지정하지 않았다면, 태그로 자동 검색
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                Debug.LogError("씬에 'Player' 태그를 가진 오브젝트가 없습니다! 플레이어 오브젝트의 태그를 확인해주세요.");
            }
        }
    }

    void Update()
    {
        if (playerTransform == null || currentWaveIndex >= waves.Count) return;

        spawnTimer += Time.deltaTime;

        // 생성 주기가 되었을 때 적 생성
        if (spawnTimer >= waves[currentWaveIndex].spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (waves.Count == 0 || waves[currentWaveIndex].enemyPrefabs.Count == 0) return;

        WaveData currentWave = waves[currentWaveIndex];

        // 1. 플레이어 주변의 무작위 생성 위치 계산 (도넛 형태)
        Vector2 spawnPosition = GetRandomSpawnPosition();

        // 2. 이번 웨이브의 적 목록 중 랜덤으로 하나 선택
        int randomIndex = Random.Range(0, currentWave.enemyPrefabs.Count);
        GameObject enemyPrefab = currentWave.enemyPrefabs[randomIndex];

        // 3. 적 생성
        if (enemyPrefab != null)
        {
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    Vector2 GetRandomSpawnPosition()
    {
        // 랜덤한 각도(0~360도)를 라디안으로 변환
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        // 최소 반지름과 최대 반지름 사이의 랜덤한 거리 선택
        float randomRadius = Random.Range(minRadius, maxRadius);

        // 삼각함수를 이용해 플레이어 주변 좌표 계산
        float spawnX = playerTransform.position.x + Mathf.Cos(randomAngle) * randomRadius;
        float spawnY = playerTransform.position.y + Mathf.Sin(randomAngle) * randomRadius;

        return new Vector2(spawnX, spawnY);
    }

    // 다음 웨이브로 수동 전환하고 싶을 때 외부에서 호출하는 메서드
    public void NextWave()
    {
        if (currentWaveIndex < waves.Count - 1)
        {
            currentWaveIndex++;
            Debug.Log($"Wave 변경됨: {waves[currentWaveIndex].waveName}");
        }
    }

    // [활용 팁] 현재 진행 중인 스테이지의 웨이브 정보를 세이브 파일에 강제로 쓰고 싶을 때 호출하는 예시 함수
    public void SaveCurrentWaveProgress()
    {
        if (GameDateManager.instance == null) return;

        List<WaveSaveData> saveList = new List<WaveSaveData>();

        // 현재 스포너가 가지고 있는 모든 웨이브 정보를 텍스트 데이터(WaveSaveData)로 변환
        foreach (var wave in waves)
        {
            saveList.Add(wave.ToSaveData());
        }

        // 데이터 매니저의 메모리에 찌르고 저장하기
        GameDateManager.instance.playerData.customWaveProgress = saveList;
        GameDateManager.instance.SaveData(GameDateManager.instance.playerData);
    }
}