using System.Collections.Generic;
using UnityEngine;

public class LevelUpUIManager : MonoBehaviour
{
    // 어디서나 쉽게 접근할 수 있도록 싱글톤(Singleton) 구조로 만듭니다.
    public static LevelUpUIManager instance;

    [Header("UI 패널")]
    public GameObject levelUpPanel; // 화면을 가릴 레벨업 전체 UI 패널

    [Header("카드 컴포넌트 리스트 (3개)")]
    public List<UpgradeCard> cardComponents; // 화면에 배치한 카드 버튼 3개

    private void Awake()
    {
        // 싱글톤 세팅
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 게임 시작할 때는 레벨업 창이 안 보이도록 숨깁니다.
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }
    }

    // PlayerLevelManager에서 레벨업하는 순간 호출할 대망의 함수
    public void ShowLevelUpUI()
    {
        if (levelUpPanel == null) return;

        // 1. 뱀서류 국룰: 레벨업 창이 뜨면 게임의 시간을 일시정지합니다.
        Time.timeScale = 0f;
        levelUpPanel.SetActive(true);

        // 2. 화면에 있는 3개의 카드에 무작위 능력치를 배정합니다.
        for (int i = 0; i < cardComponents.Count; i++)
        {
            if (cardComponents[i] == null) continue;

            // 랜덤으로 강화 종류 선택 (0: 공격력, 1: 공속, 2: 체력)
            UpgradeCard.UpgradeType randomType = (UpgradeCard.UpgradeType)Random.Range(0, 3);
            float value = 0f;

            // 종류별 능력치 가산/감산 수치 셋팅
            switch (randomType)
            {
                case UpgradeCard.UpgradeType.AttackDamage:
                    value = 5f; // 선택 시 공격력 5 증가
                    break;
                case UpgradeCard.UpgradeType.AttackCooldown:
                    value = 0.05f; // 선택 시 공격 쿨타임 0.05초 감소 (공속 증가)
                    break;
                case UpgradeCard.UpgradeType.MaxHealth:
                    value = 20f; // 선택 시 최대 체력 20 증가
                    break;
            }

            // 각 카드 스크립트에 무작위로 뽑힌 데이터 주입
            cardComponents[i].SetupCard(randomType, value, this);
        }
    }

    // 카드를 클릭 완료했을 때 UpgradeCard에서 호출해줄 닫기 함수
    public void HideLevelUpUI()
    {
        if (levelUpPanel == null) return;

        // UI를 끄고 정지되었던 게임의 시간을 다시 정상(1배속)으로 돌려놓습니다.
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}