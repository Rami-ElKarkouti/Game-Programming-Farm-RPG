using UnityEngine;
using UnityEngine.UI;

public class PlayerHandVisual : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Image handImage;

    [Header("Inventory Reference")]
    [SerializeField] private InventoryManagern inventory;

    private ItemData lastItem;

    void Update()
    {
        UpdateHand();
    }

    void UpdateHand()
    {
        ItemData currentItem = inventory.GetSelectedItem(false);

        if (currentItem == lastItem) return;

        lastItem = currentItem;

        if (currentItem == null)
        {
            handImage.enabled = false;
            return;
        }

        handImage.enabled = true;
        handImage.sprite = currentItem.icon;
    }
}