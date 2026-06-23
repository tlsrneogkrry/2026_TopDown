using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 제어용

public class UpgradeCard : MonoBehaviour
{
    public enum UpgradeType
    {
        AttackDamage,
        AttackCooldown,
        HealthRestore // ★ MaxHealth에서 HealthRestore(체력 회복)로 변경
    }

    [Header("카드 설정")]
    public UpgradeType upgradeType;
    public float upgradeValue;

    [Header("UI 컴포넌트 (인스펙터에서 직접 자식을 드래그해서 넣으세요!)")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    private LevelUpUIManager uiManager;
    private Button cardButton;

    private void Start()
    {
        cardButton = GetComponent<Button>();
        if (cardButton != null)
        {
            cardButton.onClick.RemoveAllListeners();
            cardButton.onClick.AddListener(OnCardClicked);
        }
    }

    public void SetupCard(UpgradeType type, float value, LevelUpUIManager manager)
    {
        upgradeType = type;
        upgradeValue = value;
        uiManager = manager;

        if (titleText == null || descriptionText == null) return;

        // ★ 기획 텍스트 및 수치 매칭 변경
        switch (upgradeType)
        {
            case UpgradeType.AttackDamage:
                titleText.text = "날카로운 칼날";
                descriptionText.text = $"공격력이 10만큼 증가합니다.";
                break;
            case UpgradeType.AttackCooldown:
                titleText.text = "신속한 손놀림";
                descriptionText.text = $"공격 쿨타임이 0.1초 빨라집니다.";
                break;
            case UpgradeType.HealthRestore: // ★ 최대 체력에서 체력 회복으로 변경
                titleText.text = "응급 처치";
                descriptionText.text = $"채력 10 회복.";
                break;
        }
    }

    public void OnCardClicked()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        switch (upgradeType)
        {
            case UpgradeType.AttackDamage:
                player.SendMessage("UpgradeDamage", Mathf.RoundToInt(upgradeValue), SendMessageOptions.DontRequireReceiver);
                break;
            case UpgradeType.AttackCooldown:
                player.SendMessage("UpgradeCooldown", upgradeValue, SendMessageOptions.DontRequireReceiver);
                break;
            case UpgradeType.HealthRestore: // ★ 클릭 시 PlayerHealth의 RestoreHealth 함수를 호출합니다.
                player.SendMessage("RestoreHealth", Mathf.RoundToInt(upgradeValue), SendMessageOptions.DontRequireReceiver);
                break;
        }

        if (uiManager != null)
        {
            uiManager.HideLevelUpUI();
        }
        else if (LevelUpUIManager.instance != null)
        {
            LevelUpUIManager.instance.HideLevelUpUI();
        }
    }
}