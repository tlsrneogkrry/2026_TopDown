using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class WaveData
    {
        public string waveName;
        public List<GameObject> enemyPrefabs;
        public float spawnInterval = 1.0f;
        public int maxEnemiesInWave = 100;
    }

    [Header("Target")]
    [SerializeField] private Transform playerTransform;

    [Header("Spawn Zone (Radius)")]
    [SerializeField] private float minRadius = 7f;
    [SerializeField] private float maxRadius = 11f;

    [Header("Wave Settings")]
    [SerializeField] private List<WaveData> waves;

    private int currentWaveIndex = 0;
    private float spawnTimer;

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
        if (playerTransform == null || currentWaveIndex >= waves.Count) return;

        spawnTimer += Time.deltaTime;

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
        Vector2 spawnPosition = GetRandomSpawnPosition();

        int randomIndex = UnityEngine.Random.Range(0, currentWave.enemyPrefabs.Count);
        GameObject enemyPrefab = currentWave.enemyPrefabs[randomIndex];

        if (enemyPrefab != null)
        {
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    Vector2 GetRandomSpawnPosition()
    {
        float randomAngle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float randomRadius = UnityEngine.Random.Range(minRadius, maxRadius);

        float spawnX = playerTransform.position.x + Mathf.Cos(randomAngle) * randomRadius;
        float spawnY = playerTransform.position.y + Mathf.Sin(randomAngle) * randomRadius;

        return new Vector2(spawnX, spawnY);
    }
}