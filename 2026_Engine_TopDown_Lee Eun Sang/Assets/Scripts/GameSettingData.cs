using UnityEngine;

// [주교재 13강 규격] 인스펙터에서 에셋을 만들 수 있는 ScriptableObject 형식을 무조건 유지합니다.
[CreateAssetMenu(fileName = "GameSettingData", menuName = "Scriptable Objects/GameSettingData")]
public class GameSettingData : ScriptableObject
{
    // [유저님의 기존 인게임 밸런스 설정 데이터 원본 100% 보존]
    public int startHp = 100;
    public int startAttack = 10;
    public float playerMoveSpeed = 5f;

    public int hpbounsperDeath = 5;
    public int atkBounsperDeath = 1;

    // -------------------------------------------------------------
    // ★ [데이터 매니저 JSON 세이브/로드 호환용 안전장치]
    // GameDataManager 내부에서 JsonUtility를 통해 이 데이터를 파일로 읽고 쓸 때
    // 볼륨 데이터(bgmVolume, sfxVolume) 칸이 비어있어 터지던 컴파일 에러(빨간 줄)를 완벽하게 방어합니다.
    // [HideInInspector]를 붙여두었기 때문에 유니티 인스펙터 창에는 깔끔하게 보이지 않습니다.
    // -------------------------------------------------------------
    [HideInInspector] public float bgmVolume = 1.0f;
    [HideInInspector] public float sfxVolume = 1.0f;
    [HideInInspector] public bool isFullScreen = true;
}