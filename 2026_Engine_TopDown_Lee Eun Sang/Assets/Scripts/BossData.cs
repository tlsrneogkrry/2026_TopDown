using UnityEngine;

[CreateAssetMenu(fileName = "NewBossData", menuName = "Scriptable Objects/Boss Data")]
public class BossData : ScriptableObject
{
    [Header("보스 기본 정보")]
    public string bossName = "역병의사";
    public int maxHp = 200;
    public float moveSpeed = 1.3f;

    [Header("보물상자 드롭 설정")]
    public GameObject treasureChestPrefab; // 보스가 죽을 때 떨어뜨릴 보물상자 프리팹
}