using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerFarmInteract : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private InventoryManagern inventory;
    [SerializeField] private TileHighlightManager highlight;
    [SerializeField] private FarmManagerA farmManager;


    [Header("Valid Soil Ground")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private TileBase[] validSoilTiles;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Interact();
        }
    }

    void Interact()
    {
        Debug.Log("SPACE pressed → Interact started");

        if (inventory == null || highlight == null || farmManager == null)
        {
            Debug.LogError("Missing references!");
            return;
        }

        Vector3Int cell = highlight.GetTargetCell();
        Debug.Log("Target cell: " + cell);

        ItemData item = inventory.GetSelectedItem(false);
        if (item == null)
        {
            Debug.Log("❌ No item selected");
            return;
        }

        switch (item.itemType)
        {
            case ItemType.Tool:
                UseTool(item, cell);
                break;

            case ItemType.Seed:
                PlantSeed(item, cell);
                break;
        }
    }

    // ✅ TILE-BASED TOOL USE
    void UseTool(ItemData item, Vector3Int cell){
        switch (item.itemName)
        {
            case "Hoe":
                if (!IsValidSoilGround(cell))
                {
                    Debug.Log("❌ Cannot till here. Not valid soil ground.");
                    return;
                }

                farmManager.TillTile(cell);
                break;

            case "Watering Can":
                farmManager.WaterTile(cell);
                break;

            case "Sickle":
                Debug.Log("Sickle not implemented yet");
                break;
    }
}


    bool IsValidSoilGround(Vector3Int cell)
        {
            if (groundTilemap == null)
            {
                Debug.LogError("Ground Tilemap is missing!");
                return false;
            }

            TileBase tileAtCell = groundTilemap.GetTile(cell);

            if (tileAtCell == null)
                return false;

            foreach (TileBase validTile in validSoilTiles)
            {
                if (tileAtCell == validTile)
                    return true;
            }

            return false;
        }

    // ✅ TILE-BASED PLANTING
    void PlantSeed(ItemData seed, Vector3Int cell)
    {
        if (!farmManager.IsTilled(cell))
        {
            Debug.Log("❌ Must till first");
            return;
        }

        if (farmManager.HasCrop(cell))
        {
            Debug.Log("❌ Already has crop");
            return;
        }

        Vector3 pos = farmManager.GetCellCenterWorld(cell);
        pos.z = 0f;
        GameObject cropObj = Instantiate(seed.cropPrefab, pos, Quaternion.identity);

        // ✅ THIS IS THE FIX
        Crop crop = cropObj.GetComponent<Crop>();
        if (crop != null)
        {
            crop.cropData = seed;
        }

        bool planted = farmManager.PlantCrop(cell, cropObj);

        if (planted)
        {
            inventory.RemoveFromSelectedSlot(1);
            Debug.Log("🌱 Seed planted");
        }
        else
        {
            Destroy(cropObj);
            Debug.Log("❌ Plant failed");
        }
    }
}