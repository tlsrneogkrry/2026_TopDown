using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelManager : MonoBehaviour
{
    [Header("플레이어 레벨 경험치")]
    public int currentLevel = 1;
    public int currentExp = 0;
    public int maxExp = 5; // 레벨 1당 필요 경험치

    [Header("자동 수집 범위 (매그넷)")]
    public float magnetRadius = 3f;

    void Update()
    {
        // 자동 수집: 주변의 경험치 젬 자동 수집
        // 플레이어 위치의 중심에서 magnetRadius 크기의 원 그려 콜라이더 검색
        Collider2D[] hitGems = Physics2D.OverlapCircleAll(transform.position, magnetRadius);
        foreach (Collider2D hit in hitGems)
        {
            ExpGem gem = hit.GetComponent<ExpGem>();
            if (gem != null)
            {
                gem.StartFly(transform); // 경험치 플레이어 쪽으로 날아가게 만듦
            }
        }
    }

    // 경험치 획득 함수 (경험치 플레이어에게 넘겨줄 때 호출)
    public void GetExp(int amount)
    {
        currentExp += amount;
        Debug.Log($"경험치 획득! 현재 경험치: {currentExp} / {maxExp}");

        // 경험치가 최대 경험치 이상일 때
        while (currentExp >= maxExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentExp -= maxExp; // 사용된 경험치 제외
        currentLevel++;

        // 뱀서류 특성: 레벨이 올라갈수록 필요 경험치도 점점 늘어남 (예: 각 레벨마다 1.2배 + 5)
        maxExp = Mathf.RoundToInt(maxExp * 1.2f) + 5;

        Debug.LogWarning($"레벨 업! 새로운 레벨: {currentLevel} 입니다");

        // 레벨업 UI 표시
        TriggerLevelUpUI();
    }

    private void TriggerLevelUpUI()
    {
        // LevelUpUIManager에서 UI 표시
        if (LevelUpUIManager.instance != null)
        {
            LevelUpUIManager.instance.ShowLevelUpUI();
        }
        else
        {
            Debug.LogError("LevelUpUIManager 싱글톤을 찾을 수 없습니다!");
        }
    }

    // 디버그: 플레이어 주변의 플레이어 자동 수집 범위를 에디터에서 확인하기 위한 함수
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
}