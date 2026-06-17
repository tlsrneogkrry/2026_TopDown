using UnityEngine;

// 인스펙터에서 마우스 우클릭 -> Create -> Scriptable Objects -> EnemySettingData로 에셋 파일을 만들 수 있게 해줍니다.
[CreateAssetMenu(fileName = "EnemySettingData", menuName = "Scriptable Objects/EnemySettingData")]
public class EnemySettingData : ScriptableObject
{
    [Header("기본 적(몬스터) 스탯")]
    public int enemyStartHp = 50;         // 적 기본 체력
    public int enemyStartAttack = 12;     // 적 기본 공격력
    public float enemyMoveSpeed = 3f;     // 적 이동 속도

    [Header("스테이지 클리어 시 강화 수치 (밸런스 조정용)")]
    public int hpIncreasePerStage = 10;   // 다음 스테이지 갈 때 증가할 체력
    public int atkIncreasePerStage = 2;    // 다음 스테이지 갈 때 증가할 공격력

    // -------------------------------------------------------------
    // ★ [데이터 매니저 JSON 세이브/로드 호환용 안전장치]
    // GameDataManager가 손대지 않고 JSON 시스템을 그대로 통과할 수 있도록
    // 플레이어 세팅 데이터와 규격을 맞춰주는 숨겨진 변수들입니다.
    // -------------------------------------------------------------
    [HideInInspector] public float bgmVolume = 1.0f;
    [HideInInspector] public float sfxVolume = 1.0f;
    [HideInInspector] public bool isFullScreen = true;
}