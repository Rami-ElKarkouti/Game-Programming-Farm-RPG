using UnityEngine;

public class FarmManager : MonoBehaviour
{
    [Header("Inventory & Seed Selection")]
    public InventoryManagern inventory;

    [Header("Player Stamina")]
    public PlayerStamina playerStamina;

    void Update()
    {
        HandleTileClick();
        HandleMockSleep();
    }

    void HandleTileClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider == null) return;

        // ANY RESOURCE 
        IDamageable damageable = hit.collider.GetComponent<IDamageable>();

        if (damageable != null)
        {
            ItemData heldItem = inventory.GetSelectedItem(false);

            if (heldItem != null && heldItem.itemType == ItemType.Tool)
            {
                if (playerStamina.UseStamina(heldItem.staminaCost))
                {
                    damageable.Hit(1);
                }

                return;
            }

            return;
        }

        // 🌱 FARM TILE
        FarmTile tile = hit.collider.GetComponent<FarmTile>();
        if (tile == null) return;

        ItemData heldItemTile = inventory.GetSelectedItem(false);

        if (heldItemTile == null)
        {
            Debug.Log("No item selected");
            return;
        }

        if (heldItemTile.itemType == ItemType.Tool)
        {
            HandleToolOnTile(heldItemTile, tile);
        }
        else if (heldItemTile.itemType == ItemType.Seed)
        {
            HandleSeedOnTile(heldItemTile, tile);
        }
    }

    void HandleToolOnTile(ItemData item, FarmTile tile)
    {
        switch (item.itemName)
        {
            case "Hoe":
                if (!tile.isHoed && playerStamina.UseStamina(item.staminaCost))
                    tile.HoeTile();
                break;

            case "Watering Can":
                if (tile.isHoed && playerStamina.UseStamina(item.staminaCost))
                    tile.WaterTile();
                break;

            case "Sickle":
                if (tile.currentCrop != null &&
                    tile.currentCrop.IsFullyGrown() &&
                    playerStamina.UseStamina(item.staminaCost))
                {
                    tile.HarvestCrop(inventory);
                }
                break;
        }
    }

    void HandleSeedOnTile(ItemData item, FarmTile tile)
    {
        if (tile.CanPlantSeed && inventory.HasItem(item))
        {
            bool planted = tile.PlantSeed(item, item.cropPrefab);

            if (planted)
            {
                inventory.RemoveFromSelectedSlot(1);
            }
        }
    }

    void HandleMockSleep()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            playerStamina.RestoreFull();

            FarmTile[] tiles = FindObjectsOfType<FarmTile>();

            foreach (FarmTile tile in tiles)
            {
                if (tile.currentCrop == null)
                {
                    tile.ResetWater();
                }
            }
        }
    }
}