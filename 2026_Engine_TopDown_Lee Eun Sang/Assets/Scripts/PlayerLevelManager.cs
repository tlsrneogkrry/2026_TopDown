using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelManager : MonoBehaviour
{
    [Header("레벨 및 경험치")]
    public int currentLevel = 1;
    public int currentExp = 0;
    public int maxExp = 5; // 레벨 1일 때 요구 경험치

    [Header("자석 흡입 범위 (반지름)")]
    public float magnetRadius = 3f;

    void Update()
    {
        // 자석 기능: 주변의 경험치 보석 감지
        // 플레이어 위치를 중심으로 magnetRadius 크기의 원을 그려 충돌체 검사
        Collider2D[] hitGems = Physics2D.OverlapCircleAll(transform.position, magnetRadius);
        foreach (Collider2D hit in hitGems)
        {
            ExpGem gem = hit.GetComponent<ExpGem>();
            if (gem != null)
            {
                gem.StartFly(transform); // 보석을 플레이어 쪽으로 날아가게 만듦
            }
        }
    }

    // 경험치 획득 함수 (보석이 플레이어와 부딪힐 때 실행됨)
    public void GetExp(int amount)
    {
        currentExp += amount;
        Debug.Log($"경험치 획득! 현재 경험치: {currentExp} / {maxExp}");

        // 경험치가 가득 차면 레벨업
        while (currentExp >= maxExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentExp -= maxExp; // 남은 경험치 이월
        currentLevel++;

        // 뱀서류 특성: 레벨이 오를수록 요구 경험치량이 체증됨 (예: 전 레벨의 1.2배 + 5)
        maxExp = Mathf.RoundToInt(maxExp * 1.2f) + 5;

        Debug.LogWarning($"★ 레벨 업! 현재 레벨: {currentLevel} ★");

        // TODO: 여기서 게임을 일시정지하고 레벨업 선택지 UI 창을 띄워야 합니다.
        TriggerLevelUpUI();
    }

    private void TriggerLevelUpUI()
    {
        // 1. 게임을 일시정지 시킵니다. (시간 배율을 0으로)
        Time.timeScale = 0f;

        // 2. 프리팹이나 씬에 만들어둔 레벨업 UI 창을 활성화하는 코드가 들어갈 자리입니다.
        // 예: LevelUpUIManager.instance.ShowSkillSelection();
    }

    // 에디터 뷰에서 플레이어 주변 자석 범위를 시각적으로 확인하기 위한 함수
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
}