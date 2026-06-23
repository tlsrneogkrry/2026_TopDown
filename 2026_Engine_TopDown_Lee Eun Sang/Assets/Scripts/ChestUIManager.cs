using System.Collections.Generic;
using UnityEngine;

public class ChestUIManager : MonoBehaviour
{
    public static ChestUIManager instance;

    // ★ 캔버스 아래에 배치된 패널(Panel) 오브젝트를 그대로 넣을 수 있도록 GameObject로 통일했습니다.
    [Header("실제 켜고 끌 UI 연출 패널 오브젝트")]
    public GameObject chestPanel;

    [Header("보물상자 카드 컴포넌트 리스트 (3개)")]
    public List<ChestCard> cardComponents;

    [Header("보물상자 전용 아이템 목록 (SO 3개)")]
    public List<ItemData> rewardItemList;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // 게임 시작 시 보물상자 패널을 안전하게 숨깁니다.
        if (chestPanel != null)
        {
            chestPanel.SetActive(false);
        }
    }

    public void OpenChestUI()
    {
        // 예외 방어 코드
        if (chestPanel == null || cardComponents.Count == 0 || rewardItemList.Count == 0) return;

        Time.timeScale = 0f; // 뱀서 특유의 일시정지 기동
        chestPanel.SetActive(true); // 숨겨둔 보물상자 패널 활성화

        // 중복 방지 무작위 추첨 리스트 복사
        List<ItemData> availableItems = new List<ItemData>(rewardItemList);

        // 카드 3개에 무작위로 데이터 배정
        for (int i = 0; i < cardComponents.Count; i++)
        {
            if (cardComponents[i] == null) continue;
            if (availableItems.Count == 0) break;

            int randomIndex = Random.Range(0, availableItems.Count);
            ItemData selectedItem = availableItems[randomIndex];
            availableItems.RemoveAt(randomIndex);

            // 각 카드 컴포넌트에 데이터 전달
            cardComponents[i].SetupCard(selectedItem, this);
        }
    }

    public void CloseChestUI()
    {
        if (chestPanel != null)
        {
            chestPanel.SetActive(false);
        }
        Time.timeScale = 1f; // 게임 다시 재생
    }
}