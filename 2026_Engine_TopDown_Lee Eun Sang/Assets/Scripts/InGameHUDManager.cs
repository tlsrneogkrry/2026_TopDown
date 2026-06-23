using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InGameHUDManager : MonoBehaviour
{
    public static InGameHUDManager instance;

    [Header("HUD UI 컴포넌트")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI killCountText;

    [Header("플레이어 체력바 설정")]
    public Slider healthSlider;
    public Transform playerTransform;
    public Vector3 healthBarOffset = new Vector3(0f, 1.2f, 0f);

    // ★ 체력바가 World Space Canvas인지 Screen Space Canvas인지 선택
    [Tooltip("체력바 Canvas가 World Space면 true, Screen Space면 false")]
    public bool isWorldSpaceCanvas = false;

    // Screen Space일 때 사용할 Camera 참조
    private Camera mainCamera;

    [Header("경험치 바 (Slider)")]
    public Slider expSlider;

    private float gameTimer = 0f;
    private int totalKillCount = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;

        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                playerTransform = playerObj.transform;
        }

        // 시작 시 체력바 초기값 세팅 (꽉 찬 상태)
        if (healthSlider != null)
            healthSlider.value = 1f;

        // 경험치 바 초기값
        if (expSlider != null)
            expSlider.value = 0f;
    }

    private void LateUpdate()
    {
        FollowPlayerWithHealthBar();
    }

    private void FollowPlayerWithHealthBar()
    {
        if (healthSlider == null || playerTransform == null) return;

        if (isWorldSpaceCanvas)
        {
            // World Space Canvas: 그냥 월드 좌표로 따라가면 됨
            healthSlider.transform.position = playerTransform.position + healthBarOffset;
        }
        else
        {
            // ★ Screen Space Canvas: 월드 좌표를 스크린 좌표로 변환해야 함
            if (mainCamera == null) return;

            Vector3 worldPos = playerTransform.position + healthBarOffset;
            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

            // 플레이어가 카메라 뒤에 있으면 숨기기
            if (screenPos.z < 0)
            {
                healthSlider.gameObject.SetActive(false);
                return;
            }

            healthSlider.gameObject.SetActive(true);
            healthSlider.transform.position = screenPos;
        }
    }

    private void Update()
    {
        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        gameTimer += Time.deltaTime;
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(gameTimer / 60f);
        int seconds = Mathf.FloorToInt(gameTimer % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void AddKillCount(int amount = 1)
    {
        totalKillCount += amount;
        if (killCountText != null)
            killCountText.text = totalKillCount.ToString();
    }

    public void UpdateHealthBar(float currentHp, float maxHp)
    {
        if (healthSlider == null || maxHp <= 0) return;
        healthSlider.value = currentHp / maxHp;
    }

    public void UpdateExpBar(float currentExp, float maxExp)
    {
        if (expSlider == null || maxExp <= 0) return;
        expSlider.value = currentExp / maxExp;
    }

    // ★ 인게임 저장 버튼 OnClick()에 연결하세요
    public void OnSaveButtonClicked()
    {
        // GameDataManager가 없으면 (타이틀을 거치지 않고 인게임 씬 직접 실행 시) 자동 생성
        if (GameDataManager.Instance == null)
        {
            GameObject obj = new GameObject("GameDataManager");
            obj.AddComponent<GameDataManager>();
            Debug.LogWarning("GameDataManager가 없어서 자동 생성했습니다. 타이틀 씬부터 실행하는 것을 권장합니다.");
        }

        GameDataManager.Instance.SaveGameData();
        Debug.Log("저장 완료!");
    }

    // ★ 인게임 불러오기 버튼 OnClick()에 연결하세요
    public void OnLoadButtonClicked()
    {
        // GameDataManager가 없으면 자동 생성
        if (GameDataManager.Instance == null)
        {
            GameObject obj = new GameObject("GameDataManager");
            obj.AddComponent<GameDataManager>();
            Debug.LogWarning("GameDataManager가 없어서 자동 생성했습니다. 타이틀 씬부터 실행하는 것을 권장합니다.");
        }

        // JSON 파일에서 데이터 읽기
        GameDataManager.Instance.LoadGameData();

        Debug.Log("불러오기 완료! 씬을 다시 로드합니다.");

        // ★ 저장된 스테이지 씬을 새로 로드
        // 씬 로드 완료 후 OnSceneLoaded → ApplyLoadedDataToGame() 자동 호출됨
        int stage = GameDataManager.Instance.playerData.stage;
        SceneManager.LoadScene("Level_" + stage);
    }
}