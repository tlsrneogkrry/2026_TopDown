using UnityEngine;

public class PlayerLevelManager : MonoBehaviour
{
    public static PlayerLevelManager instance;

    [Header("레벨 및 경험치 스탯")]
    public int level = 1;
    public int currentExp = 0;
    public int maxExp = 100;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    // 보석을 먹었을 때 실행되는 핵심 함수
    public void AddExp(int amount)
    {
        // ★ [10개씩 오르는 버그 방어선] 
        // 외부에서 얼마의 수치를 넘겨주든 강제로 경험치를 정확히 '1'씩만 올리도록 수식을 고정합니다!
        currentExp += 1;

        // 하단 UI 게이지 실시간 연동
        if (InGameHUDManager.instance != null)
        {
            InGameHUDManager.instance.UpdateExpBar((float)currentExp, (float)maxExp);
        }

        // 경험치 충족 시 레벨업
        if (currentExp >= maxExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentExp = 0; // 경험치 리셋
        level++;
        maxExp = Mathf.RoundToInt(maxExp * 1.3f);

        if (InGameHUDManager.instance != null)
        {
            InGameHUDManager.instance.UpdateExpBar((float)currentExp, (float)maxExp);
        }

        // 레벨업 매니저 호출
        if (LevelUpUIManager.instance != null)
        {
            LevelUpUIManager.instance.OpenLevelUpUI();
        }
    }
}