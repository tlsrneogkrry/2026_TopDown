using System.Collections.Generic;
using UnityEngine;

public class LevelUpUIManager : MonoBehaviour
{
    public static LevelUpUIManager instance;

    [Header("UI 패널")]
    public GameObject levelUpPanel;

    [Header("카드 컴포넌트 리스트 (3개)")]
    public List<UpgradeCard> cardComponents;

    private void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (levelUpPanel != null) levelUpPanel.SetActive(false);
    }

    public void ShowLevelUpUI()
    {
        if (levelUpPanel == null) return;

        Time.timeScale = 0f; // 게임 일시정지
        levelUpPanel.SetActive(true);

        // 1. 중복 방지용 후보군 리스트 생성
        List<UpgradeCard.UpgradeType> availableUpgrades = new List<UpgradeCard.UpgradeType>()
        {
            UpgradeCard.UpgradeType.AttackDamage,
            UpgradeCard.UpgradeType.AttackCooldown,
            UpgradeCard.UpgradeType.HealthRestore
        };

        // 2. 카드 3개를 순회하며 데이터를 세팅
        for (int i = 0; i < cardComponents.Count; i++)
        {
            if (cardComponents[i] == null) continue;
            if (availableUpgrades.Count == 0) break;

            // 무작위로 하나 뽑고 리스트에서 제거하여 중복 차단
            int randomIndex = Random.Range(0, availableUpgrades.Count);
            UpgradeCard.UpgradeType selectedType = availableUpgrades[randomIndex];
            availableUpgrades.RemoveAt(randomIndex);

            float value = 0f;
            switch (selectedType)
            {
                case UpgradeCard.UpgradeType.AttackDamage:
                    value = 5f;
                    break;
                case UpgradeCard.UpgradeType.AttackCooldown:
                    value = 0.05f;
                    break;
                case UpgradeCard.UpgradeType.HealthRestore:
                    value = 20f;
                    break;
            }

            // ★ 중요: 매개변수 개수를 맞춰서 무조건 3개의 인자를 던집니다.
            cardComponents[i].SetupCard(selectedType, value, this);
        }
    }

    // 카드가 클릭되었을 때 호출되어 창을 닫고 게임을 재개하는 함수
    public void HideLevelUpUI()
    {
        if (levelUpPanel == null) return;

        levelUpPanel.SetActive(false);
        Time.timeScale = 1f; // 일시정지 해제 (게임 재개)
    }
}