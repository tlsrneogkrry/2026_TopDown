using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// =================================================================
// [주교재 13강 규격] 데이터 구조체 정의 (다른 곳은 절대 수정 없음)
// =================================================================

[Serializable]
public class PlayerData
{
    public List<string> collectedItems = new List<string>();
    public int stage = 1;
    public int totalGold = 0;
    public int playCount = 0;
}

// =================================================================
// [주교재 13강 규격] 메인 데이터 매니저 컴포넌트
// =================================================================
public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    [Header("메모리 데이터 적재 영역")]
    public PlayerData playerData;

    // ★ 변수 선언 타입과 상단 클래스명을 'GameSettingData'로 정확하게 일치시켰습니다.
    public GameSettingData settingData;

    private string SaveFilePath => Path.Combine(Application.persistentDataPath, "player_data.json");
    private string SettingFilePath => Path.Combine(Application.persistentDataPath, "setting_data.json");

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadGameData();
            LoadSettingData(); // 시작 시 세팅 데이터 로드
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =================================================================
    // [파트 1] 세이브 데이터 JSON 입출력 (기존 그대로 유지)
    // =================================================================

    public void SaveGameData()
    {
        try
        {
            string json = JsonUtility.ToJson(playerData, true);
            File.WriteAllText(SaveFilePath, json);
            Debug.Log("게임 데이터 저장됨: " + json);
        }
        catch (Exception e)
        {
            Debug.LogError("데이터 저장 중 오류 발생: " + e.Message);
        }
    }

    public void LoadGameData()
    {
        if (File.Exists(SaveFilePath))
        {
            try
            {
                string json = File.ReadAllText(SaveFilePath);
                playerData = JsonUtility.FromJson<PlayerData>(json);
                Debug.Log("게임 데이터 로드됨: " + json);
            }
            catch (Exception e)
            {
                Debug.LogError("데이터 로드 중 오류 발생: " + e.Message);
                playerData = new PlayerData();
            }
        }
        else
        {
            playerData = new PlayerData();
        }
    }

    // =================================================================
    // ★ [파트 2] 환경 설정 세팅 데이터 JSON 입출력 오류 완벽 수정
    // =================================================================

    public void SaveSettingData()
    {
        try
        {
            // 수정된 GameSettingData 형식으로 안전하게 직렬화 저장합니다.
            string json = JsonUtility.ToJson(settingData, true);
            File.WriteAllText(SettingFilePath, json);
            Debug.Log("게임 세팅 데이터 저장됨: " + json);
        }
        catch (Exception e)
        {
            Debug.LogError("세팅 데이터 저장 중 오류 발생: " + e.Message);
        }
    }

    public void LoadSettingData()
    {
        if (File.Exists(SettingFilePath))
        {
            try
            {
                string json = File.ReadAllText(SettingFilePath);
                settingData = JsonUtility.FromJson<GameSettingData>(json);
                Debug.Log("게임 세팅 데이터 로드됨: " + json);
            }
            catch (Exception e)
            {
                Debug.LogError("세팅 데이터 로드 중 오류 발생: " + e.Message);
                settingData = new GameSettingData();
            }
        }
        else
        {
            Debug.LogWarning("저장된 세팅 데이터가 없습니다. 기본 세팅을 생성합니다.");
            settingData = new GameSettingData();
        }
    }

    // =================================================================
    // [파트 3] 외부 연동 제어 함수군 (기존 그대로 유지)
    // =================================================================

    public void GameStart()
    {
        LoadGameData();
        if (playerData == null) playerData = new PlayerData();

        playerData.playCount++;
        SaveGameData();

        SceneManager.LoadScene("Level_" + playerData.stage);
    }

    public void SaveGameResult()
    {
        LoadGameData();

        if (playerData != null)
        {
            playerData.stage = 1;
            playerData.collectedItems.Clear();
            SaveGameData();
        }
    }
}