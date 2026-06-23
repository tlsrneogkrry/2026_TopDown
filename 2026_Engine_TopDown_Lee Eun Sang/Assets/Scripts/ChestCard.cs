using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChestCard : MonoBehaviour
{
    private ItemData itemData;
    private ChestUIManager uiManager;
    private Button cardButton;

    [Header("UI 컴포넌트")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescText;
    public Image itemIconImage;

    private void Start()
    {
        cardButton = GetComponent<Button>();
        if (cardButton != null)
        {
            cardButton.onClick.RemoveAllListeners();
            cardButton.onClick.AddListener(OnCardClicked);
        }
    }

    public void SetupCard(ItemData data, ChestUIManager manager)
    {
        itemData = data;
        uiManager = manager;

        if (itemNameText == null || itemDescText == null || itemIconImage == null) return;

        itemNameText.text = itemData.itemName;
        itemDescText.text = itemData.itemDescription;
        if (itemData.itemIcon != null)
        {
            itemIconImage.sprite = itemData.itemIcon;
        }
    }

    private void OnCardClicked()
    {
        if (itemData == null) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        // 플레이어의 컴포넌트 추출
        PlayerAttack attackScript = player.GetComponent<PlayerAttack>();
        PlayerHealth healthScript = player.GetComponent<PlayerHealth>();

        string msg = itemData.playerMessageName;
        int intVal = Mathf.RoundToInt(itemData.value);

        if (msg == "UpgradeDamage" && attackScript != null)
        {
            attackScript.UpgradeDamage(intVal);
        }
        else if (msg == "UpgradeAttackCount" && attackScript != null)
        {
            // ★ 여기서 공격 스크립트를 추가 장착하라는 함수를 실행합니다!
            attackScript.UpgradeAttackCount(intVal);
        }
        else if (msg == "RestoreHealth" && healthScript != null)
        {
            if (intVal == -1)
            {
                healthScript.RestoreHealth(healthScript.maxHealth);
            }
            else
            {
                healthScript.RestoreHealth(intVal);
            }
        }

        if (uiManager != null)
        {
            uiManager.CloseChestUI();
        }
    }
}