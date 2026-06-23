using System;
using UnityEngine;

// ★ ScriptableObject → 일반 [Serializable] 클래스로 변경
// JsonUtility로 JSON 저장/불러오기가 가능하고, new GameSettingData()로 생성 가능
[Serializable]
public class GameSettingData
{
    public int startHp = 100;
    public int startAttack = 10;
    public float playerMoveSpeed = 5f;

    public int hpBonusPerDeath = 5;
    public int atkBonusPerDeath = 1;

    public float bgmVolume = 1.0f;
    public float sfxVolume = 1.0f;
    public bool isFullScreen = true;
}