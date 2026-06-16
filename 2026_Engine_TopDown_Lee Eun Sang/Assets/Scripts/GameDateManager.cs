using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// [개선] JSON 저장용 웨이브 데이터 구조 (프리팹 대신 ID/이름 문자열 사용)
[Serializable]
public class WaveSaveData
{
    public string waveName;
    public List<string> enemyIDs = new List<string>(); // 예: "Zombie", "Skeleton" 등 프리팹 이름
    public float spawnInterval = 1.0f;
    public int maxEnemiesInWave = 100;
}

[Serializable]
public class PlayerData
{
    public List<string> collectedItems = new List<string>();
    public int stage = 1;

    // [추가] 이제 플레이어 데이터에 현재 진행 중이거나 해금된 웨이브 정보도 함께 저장할 수 있습니다.
    public List<WaveSaveData> customWaveProgress = new List<WaveSaveData>();
}

public class GameDateManager : MonoBehaviour
{
    public static GameDateManager instance;
    public PlayerData playerData;

    private string FilePath => Application.persistentDataPath + "/player_data.json";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // 게임 시작 시 자동으로 데이터를 불러와 메모리에 적재해둡니다.
            playerData = LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveData(PlayerData dataToSave)
    {
        try
        {
            string json = JsonUtility.ToJson(dataToSave, true);
            File.WriteAllText(FilePath, json);
            playerData = dataToSave; // 메모리 데이터 갱신
            Debug.Log("게임 데이터 저장됨: " + json);
        }
        catch (Exception e)
        {
            Debug.LogError($"데이터 저장 실패: {e.Message}");
        }
    }

    public PlayerData LoadData()
    {
        if (File.Exists(FilePath))
        {
            try
            {
                string json = File.ReadAllText(FilePath);
                PlayerData loadedData = JsonUtility.FromJson<PlayerData>(json);
                Debug.Log("게임 데이터 로드됨: " + json);
                return loadedData;
            }
            catch (Exception e)
            {
                Debug.LogError($"데이터 로드 실패 (파일 손상 가능성): {e.Message}");
                return new PlayerData();
            }
        }
        else
        {
            Debug.LogWarning("저장된 게임 데이터가 없습니다. 새로운 데이터를 생성합니다.");
            return new PlayerData();
        }
    }

    public void GameStart()
    {
        // 최신 데이터 로드
        playerData = LoadData();

        if (playerData == null)
        {
            playerData = new PlayerData();
        }

        SceneManager.LoadScene("Level_" + playerData.stage);
    }

    public void PlayerDead()
    {
        // 사망 시 데이터 초기화 및 저장
        playerData.stage = 1;
        playerData.collectedItems.Clear();
        playerData.customWaveProgress.Clear(); // 웨이브 데이터도 필요시 초기화

        SaveData(playerData);
        SceneManager.LoadScene("GameOver");
    }
}