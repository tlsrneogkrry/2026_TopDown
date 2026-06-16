using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class PlayerData
{
    public List<string> collectedItems = new List<string>();
    public int stage = 1;

    // ★ [기말 프로젝트 필수 조건] 죽어도 남는 데이터 항목들
    public int totalGold = 0;
    public int playCount = 0;
}

public class GameDateManager : MonoBehaviour
{
    public static GameDateManager instance;
    public PlayerData playerData;

    // 13강 교안 기준 Path.Combine 방식을 활용한 JSON 파일 경로 지정
    private string FilePath => Path.Combine(Application.persistentDataPath, "player_data.json");

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            playerData = LoadData(); // 시작 시 자동 로드
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 13강 교안 기반: Object를 JSON 텍스트로 직렬화하여 파일로 보존
    public void SaveData(PlayerData dataToSave)
    {
        try
        {
            string json = JsonUtility.ToJson(dataToSave, true);
            File.WriteAllText(FilePath, json);
            playerData = dataToSave; // 메모리 데이터 동기화
            Debug.Log($"[13강 JSON 저장 성공] 경로: {FilePath}\n내용: {json}");
        }
        catch (Exception e)
        {
            Debug.LogError($"데이터 저장 중 오류 발생: {e.Message}");
        }
    }

    // 13강 교안 기반: JSON 텍스트 파일을 읽어와 오브젝트 데이터로 복원(역직렬화)
    public PlayerData LoadData()
    {
        if (File.Exists(FilePath))
        {
            try
            {
                string json = File.ReadAllText(FilePath);
                PlayerData loadedData = JsonUtility.FromJson<PlayerData>(json);
                Debug.Log($"[13강 JSON 로드 성공] 내용: {json}");
                return loadedData;
            }
            catch (Exception e)
            {
                Debug.LogError($"데이터 로드 중 오류 발생: {e.Message}");
                return new PlayerData();
            }
        }
        else
        {
            Debug.LogWarning("저장된 세이브 파일이 존재하지 않아 새로운 인스턴스를 생성합니다.");
            return new PlayerData();
        }
    }

    public void GameStart()
    {
        playerData = LoadData();
        if (playerData == null) playerData = new PlayerData();

        playerData.playCount++; // 플레이 횟수 영구 누적 가산 [과제 필수 조건]
        SaveData(playerData);

        SceneManager.LoadScene("Level_" + playerData.stage);
    }

    public void PlayerDead()
    {
        PlayerData currentData = LoadData();
        if (currentData != null)
        {
            // 인게임 임시 스펙 및 휘발성 현황 리셋
            currentData.stage = 1;
            currentData.collectedItems.Clear();

            // ★ 과제 필수 요건: totalGold와 playCount 같은 영구 데이터는 유지!
            playerData = currentData;
            SaveData(playerData);
        }
        SceneManager.LoadScene("GameOver");
    }
}