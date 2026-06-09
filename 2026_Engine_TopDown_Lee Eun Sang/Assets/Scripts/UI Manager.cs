using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void GameSave()
    {
        GameDateManager.instance.SaveData(GameDateManager.instance.playerData);
    }

    public void GameStart()
    {
        SceneManager.LoadScene("Level_1");
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
