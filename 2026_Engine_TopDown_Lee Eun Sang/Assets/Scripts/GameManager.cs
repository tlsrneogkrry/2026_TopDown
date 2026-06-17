using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 플레이어가 죽었을 때 PlayerHealth에서 이 함수를 호출합니다.
    public void GameOver()
    {
        Debug.Log("GameManager: 게임 오버 화면으로 이동합니다.");

        // ★ 타이틀이 아닌, 정확히 게임 오버 씬으로 화면을 전환합니다.
        // 만약 유니티 상의 씬 파일 이름이 "GameOver"가 아니라면 그 이름으로 바꿔주세요!
        SceneManager.LoadScene("GameOver");
    }
}