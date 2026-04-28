using UnityEngine;

public class PlayerToolActions : MonoBehaviour
{
    [SerializeField] private PlayerToolManager playerToolManager;
    [SerializeField] private TileHighlightManager tileHighlightManager;
    [SerializeField] private FarmManager farmManager;
    [SerializeField] private GameObject cropPrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UseEquippedTool();
        }
    }

    private void UseEquippedTool()
    {
        Vector3Int targetCell = tileHighlightManager.GetTargetCell();

        switch (playerToolManager.EquippedTool)
        {
            case ToolType.Hoe:
                farmManager.TillTile(targetCell);
                break;

            case ToolType.WateringCan:
                farmManager.WaterTile(targetCell);
                break;

            case ToolType.Seeds:
                PlantSeed(targetCell);
                break;
        }
    }

    private void PlantSeed(Vector3Int targetCell)
    {
        if (farmManager.HasCrop(targetCell))
            return;

        Vector3 spawnPos = farmManager.GetCellCenterWorld(targetCell);
        GameObject cropObj = Instantiate(cropPrefab, spawnPos, Quaternion.identity);

        Crop crop = cropObj.GetComponent<Crop>();
        if (crop != null)
            crop.plantedCell = targetCell;

        bool planted = farmManager.PlantCrop(targetCell, cropObj);

        if (!planted)
            Destroy(cropObj);
    }
}