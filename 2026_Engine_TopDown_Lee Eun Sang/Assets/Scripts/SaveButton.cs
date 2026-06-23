using UnityEngine;

public class SaveButton : MonoBehaviour
{
    public void OnSaveButtonClicked()
    {
        GameDataManager.Instance.SaveGameData();
        Debug.Log("저장 완료!");
    }
}
