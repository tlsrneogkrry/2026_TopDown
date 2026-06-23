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
    public int totalGold = 0;
    public int playCount = 0;

    public int level = 1;
    public int currentExp = 0;
    public int maxExp = 100;

    public int currentHealth = 100;
    public int maxHealth = 100;

    public float attackDamage = 10f;
    public float attackCooldown = 1f;

    public float totalPlayTime = 0f;

    // ★ 강화 데이터 — 죽어도 초기화되지 않고 영구 유지
    public UpgradeData upgradeData = new UpgradeData();
}

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    [Header("메모리 상의 게임 데이터")]
    public PlayerData playerData;

    // ★ ScriptableObject가 아닌 일반 클래스이므로 인스펙터에 노출하지 않음
    //    → 인스펙터에서 null로 덮어씌워지는 버그 방지
    [HideInInspector] public GameSettingData settingData;

    private string SaveFilePath => Path.Combine(Application.persistentDataPath, "player_data.json");
    private string SettingFilePath => Path.Combine(Application.persistentDataPath, "setting_data.json");

    private void Awake()
    {
        // ★ 이미 Instance가 존재하면 자신(새로 생성된 것)을 즉시 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // ★ 인스펙터에서 null이 될 수 있으므로 먼저 기본값으로 초기화
        if (settingData == null) settingData = new GameSettingData();

        LoadGameData();
        LoadSettingData();

        // ★ 씬이 로드될 때마다 ApplyLoadedDataToGame()을 자동 호출
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ★ 씬 전환 완료 후 자동으로 호출됨
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 메인 메뉴 씬 등 데이터 적용이 필요 없는 씬은 이름으로 걸러낼 수 있음
        // 예: if (scene.name == "MainMenu") return;

        // ★ 한 프레임 뒤에 적용 — 씬의 모든 오브젝트가 Awake/Start를 마친 후 실행되어야 함
        StartCoroutine(ApplyDataNextFrame());
    }

    private System.Collections.IEnumerator ApplyDataNextFrame()
    {
        // 한 프레임 대기 → 씬의 PlayerHealth, PlayerLevelManager 등이 초기화된 후 적용
        yield return null;
        ApplyLoadedDataToGame();
    }

    private void Update()
    {
        if (playerData != null)
            playerData.totalPlayTime += Time.deltaTime;
    }

    // ★ 씬에 있는 오브젝트에서 현재 값을 수집해 playerData에 반영 후 저장
    public void SaveGameData()
    {
        try
        {
            CollectCurrentGameState();

            string json = JsonUtility.ToJson(playerData, true);
            File.WriteAllText(SaveFilePath, json);
            Debug.Log("게임 데이터 저장 완료: " + json);
        }
        catch (Exception e)
        {
            Debug.LogError("데이터 저장 중 오류 발생: " + e.Message);
        }
    }

    private void CollectCurrentGameState()
    {
        // 레벨 / 경험치
        if (PlayerLevelManager.instance != null)
        {
            playerData.level = PlayerLevelManager.instance.level;
            playerData.currentExp = PlayerLevelManager.instance.currentExp;
            playerData.maxExp = PlayerLevelManager.instance.maxExp;
        }

        // 체력 / 공격력
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            PlayerHealth health = playerObj.GetComponent<PlayerHealth>();
            if (health != null)
            {
                playerData.currentHealth = health.currentHealth;
                playerData.maxHealth = health.maxHealth;
            }

            PlayerAttack attack = playerObj.GetComponent<PlayerAttack>();
            if (attack != null)
            {
                playerData.attackDamage = attack.attackDamage;
                playerData.attackCooldown = attack.attackCooldown;
            }
        }

        // ★ stage는 level과 분리해서 별도 관리
        // 필요 시 StageManager 등에서 가져오도록 교체하세요
        // playerData.stage = StageManager.instance.currentStage;
    }

    public void LoadGameData()
    {
        if (File.Exists(SaveFilePath))
        {
            try
            {
                string json = File.ReadAllText(SaveFilePath);
                playerData = JsonUtility.FromJson<PlayerData>(json);
                Debug.Log("게임 데이터 불러오기 완료: " + json);
            }
            catch (Exception e)
            {
                Debug.LogError("데이터 불러오기 중 오류 발생: " + e.Message);
                playerData = new PlayerData();
            }
        }
        else
        {
            playerData = new PlayerData();
            Debug.Log("저장 파일 없음 → 기본 데이터로 초기화");
        }
    }

    // ★ 불러온 playerData를 실제 씬의 오브젝트에 적용
    public void ApplyLoadedDataToGame()
    {
        if (playerData == null) return;

        // 레벨 / 경험치 적용
        if (PlayerLevelManager.instance != null)
        {
            PlayerLevelManager.instance.level = playerData.level;
            PlayerLevelManager.instance.currentExp = playerData.currentExp;
            PlayerLevelManager.instance.maxExp = playerData.maxExp;
        }

        // 체력 적용
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            PlayerHealth health = playerObj.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.maxHealth = playerData.maxHealth;
                health.currentHealth = playerData.currentHealth;

                if (InGameHUDManager.instance != null)
                    InGameHUDManager.instance.UpdateHealthBar(health.currentHealth, health.maxHealth);
            }

            PlayerAttack attack = playerObj.GetComponent<PlayerAttack>();
            if (attack != null)
            {
                attack.attackDamage = (int)playerData.attackDamage;
                attack.attackCooldown = playerData.attackCooldown;
            }
        }

        // 경험치 바 적용
        if (InGameHUDManager.instance != null)
            InGameHUDManager.instance.UpdateExpBar(playerData.currentExp, playerData.maxExp);

        Debug.Log("저장 데이터 게임에 적용 완료!");
    }

    public void SaveSettingData()
    {
        try
        {
            string json = JsonUtility.ToJson(settingData, true);
            File.WriteAllText(SettingFilePath, json);
            Debug.Log("설정 데이터 저장 완료");
        }
        catch (Exception e)
        {
            Debug.LogError("설정 저장 중 오류 발생: " + e.Message);
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
            }
            catch (Exception e)
            {
                Debug.LogError("설정 불러오기 중 오류 발생: " + e.Message);
                settingData = new GameSettingData();
            }
        }
        else
        {
            settingData = new GameSettingData();
        }
    }

    public void GameStart()
    {
        // ★ 새 게임 시작 — 강화 데이터와 골드, 플레이 횟수는 유지하고 나머지 초기화
        int prevPlayCount = 0;
        int prevGold = 0;
        UpgradeData prevUpgrade = new UpgradeData();

        if (playerData != null)
        {
            prevPlayCount = playerData.playCount;
            prevGold = playerData.totalGold;
            prevUpgrade = playerData.upgradeData;
        }

        playerData = new PlayerData();
        playerData.playCount = prevPlayCount + 1;
        playerData.totalGold = prevGold;       // 골드 유지
        playerData.upgradeData = prevUpgrade;  // 강화 단계 유지

        // ★ 강화 수치를 기본 스탯에 반영
        if (UpgradeManager.instance != null)
        {
            playerData.maxHealth = UpgradeManager.instance.GetBaseHealth();
            playerData.currentHealth = playerData.maxHealth;
            playerData.attackDamage = UpgradeManager.instance.GetBaseAttack();
        }

        // 초기화된 데이터를 JSON에 저장
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(SaveFilePath, json);

        Debug.Log("새 게임 시작! 강화 수치 적용 완료");
        SceneManager.LoadScene("Level_" + playerData.stage);
        // 씬 로드 완료 후 OnSceneLoaded → ApplyLoadedDataToGame() 자동 호출됨
    }

    public void SaveGameResult()
    {
        // 현재 씬에서 최신 상태 수집 후 저장
        CollectCurrentGameState();

        if (playerData != null)
        {
            playerData.stage = 1;
            playerData.collectedItems.Clear();
            SaveGameData();
        }
    }

    public string GetFormattedPlayTime()
    {
        if (playerData == null) return "00:00";
        int minutes = Mathf.FloorToInt(playerData.totalPlayTime / 60f);
        int seconds = Mathf.FloorToInt(playerData.totalPlayTime % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}