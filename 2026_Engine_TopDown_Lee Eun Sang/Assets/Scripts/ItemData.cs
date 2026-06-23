using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Scriptable Objects/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;          // 아이템 이름
    [TextArea(2, 4)]
    public string itemDescription;   // 아이템 설명
    public Sprite itemIcon;          // 유저님이 가지고 계신 아이템 이미지

    [Header("플레이어 연동 설정")]
    public string playerMessageName; // 플레이어에게 보낼 함수 이름
    public float value;              // 반영할 수치 값
}