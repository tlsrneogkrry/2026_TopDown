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

        Time.timeScale = 0f; // 일시정지
        levelUpPanel.SetActive(true);

        for (int i = 0; i < cardComponents.Count; i++)
        {
            if (cardComponents[i] == null) continue;

            UpgradeCard.UpgradeType randomType = (UpgradeCard.UpgradeType)Random.Range(0, 3);
            float value = 0f;

            switch (randomType)
            {
                case UpgradeCard.UpgradeType.AttackDamage:
                    value = 5f;
                    break;
                case UpgradeCard.UpgradeType.AttackCooldown:
                    value = 0.05f;
                    break;
                case UpgradeCard.UpgradeType.MaxHealth:
                    value = 20f;
                    break;
            }

            cardComponents[i].SetupCard(randomType, value, this);
        }
    }

    public void HideLevelUpUI()
    {
        if (levelUpPanel == null) return;

        levelUpPanel.SetActive(false);
        Time.timeScale = 1f; // 시간 재생
    }
}