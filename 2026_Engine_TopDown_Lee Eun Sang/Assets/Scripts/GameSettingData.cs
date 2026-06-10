using UnityEngine;

[CreateAssetMenu(fileName = "GameSettingData", menuName = "Scriptable Objects/GameSettingData")]
public class GameSettingData : ScriptableObject
{
    public int startHp = 100;
    public int startAttack = 10;
    public float playerMoveSpeed = 5f;

    public int hpbounsperDeath = 5;
    public int atkBounsperDeath = 1;
}
