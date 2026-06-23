using UnityEngine;
using UnityEngine.UI;
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
}