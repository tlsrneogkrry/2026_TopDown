using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void GameSave()
    {
        // ★ [오류 수정 완료] GameDateManager.instance 대신 표준 GameDataManager.Instance를 사용하여 저장 처리를 지시합니다.
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SaveGameData();
        }
    }

    public void GameStart()
    {
        // 게임 시작 시 데이터 매니저를 거쳐 로드 후 시작하도록 유도하거나 즉시 이동 처리
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.GameStart();
        }
        else
        {
            SceneManager.LoadScene("Level_1");
        }
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    public void Title()
    {
        SceneManager.LoadScene("Title");
    }
}