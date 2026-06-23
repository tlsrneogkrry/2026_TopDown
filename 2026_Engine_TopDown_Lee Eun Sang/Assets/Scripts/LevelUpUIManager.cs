using UnityEngine;

public class LevelUpUIManager : MonoBehaviour
{
    public static LevelUpUIManager instance;

    [Header("레벨업 UI 패널 설정")]
    public GameObject levelUpPanel;

    [Header("레벨업 카드 컴포넌트 리스트")]
    public UpgradeCard[] cardComponents;

    // ★ 카드에 표시할 업그레이드 타입과 수치를 여기서 지정
    [Header("각 카드 업그레이드 설정 (cardComponents와 순서 맞추기)")]
    public UpgradeCard.UpgradeType[] upgradeTypes = new UpgradeCard.UpgradeType[]
    {
        UpgradeCard.UpgradeType.AttackDamage,
        UpgradeCard.UpgradeType.AttackCooldown,
        UpgradeCard.UpgradeType.HealthRestore
    };
    public float[] upgradeValues = new float[] { 10f, 0.1f, 10f };

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

    public void OpenLevelUpUI()
    {
        if (levelUpPanel == null) return;

        levelUpPanel.SetActive(true);
        Time.timeScale = 0f;

        // ★ 핵심 수정: 카드가 열릴 때 실제로 SetupCard()를 호출해 텍스트 세팅
        for (int i = 0; i < cardComponents.Length; i++)
        {
            if (cardComponents[i] == null) continue;

            UpgradeCard.UpgradeType type = (i < upgradeTypes.Length)
                ? upgradeTypes[i]
                : UpgradeCard.UpgradeType.AttackDamage;

            float value = (i < upgradeValues.Length) ? upgradeValues[i] : 10f;

            cardComponents[i].SetupCard(type, value, this);

            Debug.Log($"[레벨업 UI] 카드 {i} 세팅 완료: {type} / {value}");
        }

        Debug.Log("🎉 [레벨업 UI] 선택 창 활성화 완료.");
    }

    public void HideLevelUpUI()
    {
        if (levelUpPanel == null) return;

        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
        Debug.Log("[레벨업 UI] 카드 선택 완료! 게임을 재개합니다.");
    }

    public void CloseLevelUpUI()
    {
        HideLevelUpUI();
    }
}