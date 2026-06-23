using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 부딪힌 대상이 플레이어 태그를 가졌는지 검사
        if (collision.CompareTag("Player"))
        {
            // ⭐ [핵심 연결고리] 보물상자 UI 매니저의 인스턴스를 찾아 오픈 함수를 실행합니다.
            if (ChestUIManager.instance != null)
            {
                ChestUIManager.instance.OpenChestUI();
            }
            else
            {
                Debug.LogError("씬에서 ChestUIManager 싱글톤 인스턴스를 찾을 수 없습니다! 오브젝트에 스크립트가 붙어있는지 확인하세요.");
            }

            // 기존 골드 정산 데이터 보존[cite: 12]
            if (GameDataManager.Instance != null)
            {
                GameDataManager.Instance.playerData.totalGold += 100;
            }

            // 먹었으므로 월드의 상자 오브젝트 제거
            Destroy(gameObject);
        }
    }
}