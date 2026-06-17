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

    [Header("UI 컴포넌트 자동 연결용 (비어있어도 자동 검색됩니다)")]
    public Text titleText;
    public Text descriptionText;

    private LevelUpUIManager uiManager;

    private void Awake()
    {
        // ★ [UI 자동 복구 연동] 인스펙터에 수동 드래그를 안 했거나 깨졌을 경우 
        // 자식 오브젝트 중 "Title", "Description" 이름을 가진 텍스트 컴포넌트를 탐색해 자동 탑재합니다.
        if (titleText == null)
        {
            Transform foundTitle = transform.Find("Title") ?? transform.Find("TitleText") ?? transform.Find("Text");
            if (foundTitle != null) titleText = foundTitle.GetComponent<Text>();
        }

        if (descriptionText == null)
        {
            Transform foundDesc = transform.Find("Description") ?? transform.Find("DescriptionText") ?? transform.Find("SubText");
            if (foundDesc != null) descriptionText = foundDesc.GetComponent<Text>();
        }
    }

    public void SetupCard(UpgradeType type, float value, LevelUpUIManager manager)
    {
        upgradeType = type;
        upgradeValue = value;
        uiManager = manager;

        // 자동 검색 후에도 텍스트가 유실되었다면 하위 컴포넌트를 통째로 긁어와 비상 할당합니다.
        if (titleText == null || descriptionText == null)
        {
            Text[] allChildrenTexts = GetComponentsInChildren<Text>();
            if (allChildrenTexts.Length >= 2)
            {
                if (titleText == null) titleText = allChildrenTexts[0];
                if (descriptionText == null) descriptionText = allChildrenTexts[1];
            }
        }

        if (titleText == null || descriptionText == null) return;

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

        // SendMessage 방식을 통해 컴파일 의존성을 완벽 차단하여 
        // 업그레이드 선택 로직 실행 시 절대 에러가 나지 않도록 구현되었습니다.
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