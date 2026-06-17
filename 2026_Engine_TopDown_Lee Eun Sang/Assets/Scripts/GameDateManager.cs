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

    // ★ [기말 프로젝트 필수 조건] 죽어도 남는 영구 데이터
    public int totalGold = 0;       // 누적 획득 골드
    public int playCount = 0;       // 총 플레이 횟수
}

public class GameDataManager : MonoBehaviour
{
    // GameManager와 UIManager에서 공통으로 호출하기 위한 싱글톤 인스턴스
    public static GameDataManager Instance;

    public PlayerData playerData;

    // 13강 기준 Path.Combine을 사용한 정석적인 JSON 저장 경로
    private string SaveFilePath => Path.Combine(Application.persistentDataPath, "player_data.json");

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 게임 시작과 동시에 기존 저장 파일에서 데이터 자동 로드
            LoadGameData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 13강 기반: 객체를 JSON 문자열로 직렬화하여 텍스트 파일로 저장하는 함수
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

    // 13강 기반: JSON 텍스트 파일을 읽어와 오브젝트 데이터로 복원(역직렬화)하는 함수
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
                Debug.LogError("데이터 로드 중 오류 발생 (파일 손상 가능성): " + e.Message);
                playerData = new PlayerData();
            }
        }
        else
        {
            Debug.LogWarning("저장된 게임 데이터가 없습니다. 새 데이터를 생성합니다.");
            playerData = new PlayerData();
        }
    }

    // UIManager의 GameStart 대응용 함수
    public void GameStart()
    {
        LoadGameData();
        if (playerData == null) playerData = new PlayerData();

        playerData.playCount++; // 플레이 횟수 영구 누적 가산
        SaveGameData();

        SceneManager.LoadScene("Level_" + playerData.stage);
    }

    // GameManager.cs에서 게임 오버 시 결과 기록 및 동기화를 위해 호출하는 정석 통로 함수
    public void SaveGameResult()
    {
        // 최신 저장 상태를 한 번 읽어와 영구 재화 데이터를 동기화합니다.
        LoadGameData();

        if (playerData != null)
        {
            // 로그라이크 규칙: 현재 스테이지 진행 상태와 획득 템 정보는 리셋
            playerData.stage = 1;
            playerData.collectedItems.Clear();

            // ★ 과제 필수 요건: totalGold나 playCount 등의 영구 축적 데이터는 리셋하지 않고 보존!

            // 데이터 변경 사항을 최종 JSON 파일로 영구 저장 처리
            SaveGameData();
        }
    }
}