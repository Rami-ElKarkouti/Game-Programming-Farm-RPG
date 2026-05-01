using UnityEngine;

public class PlayerHandVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventoryManagern inventory;
    [SerializeField] private SpriteRenderer handSprite;

    void Update()
    {
        if (inventory == null || handSprite == null) return;

        ItemData currentItem = inventory.CurrentSelectedItem;

        if (currentItem == null)
        {
            if (handSprite.enabled)
            {
                handSprite.sprite = null;
                handSprite.enabled = false;
            }
            return;
        }

        if (!handSprite.enabled || handSprite.sprite != currentItem.icon)
        {
            handSprite.enabled = true;
            handSprite.sprite = currentItem.icon;
        }
    }
}