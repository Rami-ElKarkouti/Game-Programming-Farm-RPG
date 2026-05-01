using UnityEngine;

public class PlayerToolActions : MonoBehaviour
{
    [SerializeField] private InventoryManagern inventory;
    [SerializeField] private TileHighlightManager tileHighlightManager;
    [SerializeField] private FarmManagerA farmManager;
    [SerializeField] private GameObject cropPrefab;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UseItem();
        }
    }

    void UseItem()
    {
        ItemData item = inventory.GetSelectedItem(false);
        if (item == null) return;

        Vector3Int targetCell = tileHighlightManager.GetTargetCell();

        switch (item.itemType)
        {
            case ItemType.Tool:
                UseTool(item, targetCell);
                break;

            case ItemType.Seed:
                PlantSeed(targetCell, item);
                break;
        }
    }

    void UseTool(ItemData tool, Vector3Int cell)
    {
        switch (tool.itemName)
        {
            case "Hoe":
                farmManager.TillTile(cell);
                break;

            case "Watering Can":
                farmManager.WaterTile(cell);
                break;

            case "Sickle":
                // later hook harvest system here
                break;
        }
    }

    void PlantSeed(Vector3Int cell, ItemData seed)
    {
        if (farmManager.HasCrop(cell)) return;

        Vector3 pos = farmManager.GetCellCenterWorld(cell);
        GameObject cropObj = Instantiate(cropPrefab, pos, Quaternion.identity);

        CropA crop = cropObj.GetComponent<CropA>();
        if (crop != null)
            crop.plantedCell = cell;

        bool planted = farmManager.PlantCrop(cell, cropObj);

        if (!planted)
            Destroy(cropObj);
        else
            inventory.RemoveFromSelectedSlot(1);
    }
}