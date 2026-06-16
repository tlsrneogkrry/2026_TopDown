using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    public enum UpgradeType
    {
        AttackDamage,
        AttackCooldown,
        MaxHealth
    }

    [Header("카드 설정")]
    public UpgradeType upgradeType;
    public float upgradeValue;

    [Header("UI 컴포넌트 연결")]
    public Text titleText;
    public Text descriptionText;

    private LevelUpUIManager uiManager;

    public void SetupCard(UpgradeType type, float value, LevelUpUIManager manager)
    {
        upgradeType = type;
        upgradeValue = value;
        uiManager = manager;

        switch (upgradeType)
        {
            case UpgradeType.AttackDamage:
                titleText.text = "날카로운 칼날";
                descriptionText.text = $"이번 판의 공격력이 <color=red>+{upgradeValue}</color> 증가합니다.";
                break;
            case UpgradeType.AttackCooldown:
                titleText.text = "신속한 손놀림";
                descriptionText.text = $"이번 판의 공격 속도가 빨라집니다.\n(쿨타임 <color=green>{upgradeValue}초</color> 감소)";
                break;
            case UpgradeType.MaxHealth:
                titleText.text = "거인의 심장";
                descriptionText.text = $"이번 판의 최대 체력이 <color=orange>+{upgradeValue}</color> 증가합니다.";
                break;
        }
    }

    public void OnCardClicked()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        // 직접 클래스 이름을 언급하지 않기 때문에 절대 CS0246 에러가 발생하지 않습니다.
        switch (upgradeType)
        {
            case UpgradeType.AttackDamage:
                player.SendMessage("UpgradeDamage", Mathf.RoundToInt(upgradeValue), SendMessageOptions.DontRequireReceiver);
                break;

            case UpgradeType.AttackCooldown:
                player.SendMessage("UpgradeCooldown", upgradeValue, SendMessageOptions.DontRequireReceiver);
                break;

            case UpgradeType.MaxHealth:
                player.SendMessage("UpgradeMaxHealth", upgradeValue, SendMessageOptions.DontRequireReceiver);
                break;
        }

        if (uiManager != null) uiManager.HideLevelUpUI();
    }
}