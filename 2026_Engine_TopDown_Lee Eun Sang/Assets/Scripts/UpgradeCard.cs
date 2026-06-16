using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    public enum UpgradeType
    {
        AttackDamage,   // 공격력 증가
        AttackCooldown, // 공격 속도 증가 (쿨타임 감소)
        MaxHealth       // 최대 체력 증가
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

        // ★ 오직 '현재 판'의 플레이어 컴포넌트 수치만 실시간으로 조절합니다.
        // 세이브 파일(GameDateManager)에는 기록하지 않으므로 죽으면 자동 초기화됩니다!
        switch (upgradeType)
        {
            case UpgradeType.AttackDamage:
                PlayerAttack attack = player.GetComponent<PlayerAttack>();
                if (attack != null) attack.attackDamage += Mathf.RoundToInt(upgradeValue);
                break;

            case UpgradeType.AttackCooldown:
                PlayerAttack attackCooldown = player.GetComponent<PlayerAttack>();
                if (attackCooldown != null)
                {
                    attackCooldown.attackCooldown = Mathf.Max(0.1f, attackCooldown.attackCooldown - upgradeValue);
                }
                break;

            case UpgradeType.MaxHealth:
                // 만약 플레이어 체력 스크립트가 따로 있다면 연동
                // player.GetComponent<PlayerHealth>().maxHealth += Mathf.RoundToInt(upgradeValue);
                Debug.Log($"이번 판의 최대 체력 {upgradeValue} 증가!");
                break;
        }

        // 카드 선택창 UI 닫고 게임 재개
        uiManager.HideLevelUpUI();
    }
}