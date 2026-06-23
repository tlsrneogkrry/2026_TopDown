using UnityEngine;

// ★ 강화 데이터 — PlayerData에 포함되어 JSON으로 저장/불러오기됨
[System.Serializable]
public class UpgradeData
{
    public int healthLevel = 0;     // 체력 강화 단계 (최대 5)
    public int attackLevel = 0;     // 공격력 강화 단계 (최대 5)
    public int attackCountLevel = 0; // 공격횟수 강화 단계 (최대 3)
}

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    // ★ 강화 설정값
    private const int MAX_HEALTH_LEVEL = 5;
    private const int MAX_ATTACK_LEVEL = 5;
    private const int MAX_ATTACK_COUNT_LEVEL = 3;

    private const int HEALTH_BASE_COST = 100;   // 체력 기본 비용 (1강 100, 2강 200...)
    private const int ATTACK_BASE_COST = 150;   // 공격력 기본 비용
    private const int ATTACK_COUNT_BASE_COST = 250; // 공격횟수 기본 비용

    private const int HEALTH_BONUS_PER_LEVEL = 20; // 강화당 체력 +20
    private const int ATTACK_BONUS_PER_LEVEL = 10;  // 강화당 공격력 +10
    private const int ATTACK_COUNT_BONUS_PER_LEVEL = 1; // 강화당 공격횟수 +1

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // ★ 강화 비용 계산 (단계 × 기본비용)
    public int GetUpgradeCost(string stat)
    {
        UpgradeData upgrade = GameDataManager.Instance.playerData.upgradeData;
        switch (stat)
        {
            case "health": return HEALTH_BASE_COST * (upgrade.healthLevel + 1);
            case "attack": return ATTACK_BASE_COST * (upgrade.attackLevel + 1);
            case "attackCount": return ATTACK_COUNT_BASE_COST * (upgrade.attackCountLevel + 1);
            default: return 0;
        }
    }

    // ★ 체력 강화
    public bool UpgradeHealth()
    {
        UpgradeData upgrade = GameDataManager.Instance.playerData.upgradeData;

        if (upgrade.healthLevel >= MAX_HEALTH_LEVEL)
        {
            Debug.Log("체력 강화가 최대 단계입니다!");
            return false;
        }

        int cost = GetUpgradeCost("health");
        if (GameDataManager.Instance.playerData.totalGold < cost)
        {
            Debug.Log($"골드가 부족합니다! 필요: {cost}, 보유: {GameDataManager.Instance.playerData.totalGold}");
            return false;
        }

        GameDataManager.Instance.playerData.totalGold -= cost;
        upgrade.healthLevel++;
        Debug.Log($"체력 강화 완료! {upgrade.healthLevel}강 / 기본 체력 +{HEALTH_BONUS_PER_LEVEL * upgrade.healthLevel}");

        GameDataManager.Instance.SaveGameData();
        return true;
    }

    // ★ 공격력 강화
    public bool UpgradeAttack()
    {
        UpgradeData upgrade = GameDataManager.Instance.playerData.upgradeData;

        if (upgrade.attackLevel >= MAX_ATTACK_LEVEL)
        {
            Debug.Log("공격력 강화가 최대 단계입니다!");
            return false;
        }

        int cost = GetUpgradeCost("attack");
        if (GameDataManager.Instance.playerData.totalGold < cost)
        {
            Debug.Log($"골드가 부족합니다! 필요: {cost}, 보유: {GameDataManager.Instance.playerData.totalGold}");
            return false;
        }

        GameDataManager.Instance.playerData.totalGold -= cost;
        upgrade.attackLevel++;
        Debug.Log($"공격력 강화 완료! {upgrade.attackLevel}강 / 기본 공격력 +{ATTACK_BONUS_PER_LEVEL * upgrade.attackLevel}");

        GameDataManager.Instance.SaveGameData();
        return true;
    }

    // ★ 공격횟수 강화
    public bool UpgradeAttackCount()
    {
        UpgradeData upgrade = GameDataManager.Instance.playerData.upgradeData;

        if (upgrade.attackCountLevel >= MAX_ATTACK_COUNT_LEVEL)
        {
            Debug.Log("공격횟수 강화가 최대 단계입니다!");
            return false;
        }

        int cost = GetUpgradeCost("attackCount");
        if (GameDataManager.Instance.playerData.totalGold < cost)
        {
            Debug.Log($"골드가 부족합니다! 필요: {cost}, 보유: {GameDataManager.Instance.playerData.totalGold}");
            return false;
        }

        GameDataManager.Instance.playerData.totalGold -= cost;
        upgrade.attackCountLevel++;
        Debug.Log($"공격횟수 강화 완료! {upgrade.attackCountLevel}강 / 기본 공격횟수 +{ATTACK_COUNT_BONUS_PER_LEVEL * upgrade.attackCountLevel}");

        GameDataManager.Instance.SaveGameData();
        return true;
    }

    // ★ OnClick()용 void 래퍼 함수 (버튼에 연결하세요)
    public void OnUpgradeHealthClicked() { UpgradeHealth(); }
    public void OnUpgradeAttackClicked() { UpgradeAttack(); }
    public void OnUpgradeAttackCountClicked() { UpgradeAttackCount(); }

    // ★ 강화 수치를 적용한 최종 기본 스탯 반환
    public int GetBaseHealth()
    {
        UpgradeData upgrade = GameDataManager.Instance.playerData.upgradeData;
        return 100 + (HEALTH_BONUS_PER_LEVEL * upgrade.healthLevel);
    }

    public float GetBaseAttack()
    {
        UpgradeData upgrade = GameDataManager.Instance.playerData.upgradeData;
        return 10f + (ATTACK_BONUS_PER_LEVEL * upgrade.attackLevel);
    }

    public int GetBaseAttackCount()
    {
        UpgradeData upgrade = GameDataManager.Instance.playerData.upgradeData;
        return 1 + (ATTACK_COUNT_BONUS_PER_LEVEL * upgrade.attackCountLevel);
    }
}