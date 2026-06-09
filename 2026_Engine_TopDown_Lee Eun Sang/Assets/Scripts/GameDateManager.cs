using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class PlayerData
{
    public List<string> collectedItems = new List<string>();
    public int stage = 1;
}

public class GameDateManager : MonoBehaviour
{
    public static GameDateManager instance;
    public PlayerData playerData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); //중복 방지
        }
    }

    public void SaveData(PlayerData playerData)
    {
        string filepath = Application.persistentDataPath + "/player_data.json";
        string json = JsonUtility.ToJson(playerData, true);
        System.IO.File.WriteAllText(filepath, json);
        Debug.Log("게임 데이터 저장됨: " + json);
    }

    public PlayerData LoadData()
    {
        string filepath = Application.persistentDataPath + "/player_data.json";
        if (System.IO.File.Exists(filepath))
        {
            string json = System.IO.File.ReadAllText(filepath);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("게임 데이터 로드됨: " + json);
            return playerData;
        }
        else
        {
            Debug.LogWarning("저장된 게임 데이터가 없습니다.");
            return new PlayerData();
        }
    }

    public void GameStart()
    {
        PlayerData playerData = LoadData();
        if (playerData == null)
        {
            playerData = new PlayerData();
            SceneManager.LoadScene("Level_1");
        }
        else
        {
            SceneManager.LoadScene("Level_" + playerData.stage);
        }
    }

    public void PlayerDead()
    {
        PlayerData playerData = LoadData();
        if (playerData != null)
        {
            playerData.stage = 1; //스테이지 초기화

            playerData.collectedItems.Clear();

            SaveData(playerData);
        }
        SceneManager.LoadScene("GameOver");
    }
}
