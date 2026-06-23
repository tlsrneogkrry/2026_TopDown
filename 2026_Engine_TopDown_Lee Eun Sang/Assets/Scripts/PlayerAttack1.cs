using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttack1", menuName = "Scriptable Objects/PlayerAttack1")]
public class PlayerAttack1 : ScriptableObject
{
    // ★ PlayerAttack.cs 윗부분 변수 선언하는 곳에 추가
    [Header("공격 횟수 설정")]
    public int attackCount = 1; // 기본 공격 횟수는 1회

    // ★ PlayerAttack.cs 아래쪽 UpgradeCooldown 함수 근처에 추가
    public void UpgradeAttackCount(int amount)
    {
        attackCount += amount;
        Debug.Log($"[아이템 획득] 공격 횟수 상승! 현재 공격 횟수: {attackCount}회");

        // 팁: 나중에 공격 코루틴이나 발사 루프 쪽에서 
        // for(int i = 0; i < attackCount; i++) 구조로 이 변수를 쓰시면 공격이 연속으로 나갑니다!
    }
}
