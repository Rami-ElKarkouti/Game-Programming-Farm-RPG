using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// this script manages the state of each farm tile (tilled, watered, crop) and handles all logic related to farming actions like tilling, watering, planting, and crop growth. It uses a tilemap-based approach for
// better performance and easier integration with Unity's tilemap system. The FarmManagerA script replaces the older FarmManager script which had more limited functionality and was based on individual tile objects rather than a tilemap.
public class FarmManagerA : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap soilTilemap;

    [Header("Tiles")]
    [SerializeField] private TileBase tillledSoilTile;
    [SerializeField] private TileBase wateredSoilTile;

    private Dictionary<Vector3Int, FarmTileData> farmTiles = new();
    private Dictionary<Vector3Int, TileBase> originalSoilTiles = new();

    private void Awake()
    {
        CacheOriginalSoilTiles();
    }

    private void CacheOriginalSoilTiles()
    {
        originalSoilTiles.Clear();

        if (soilTilemap == null)
            return;

        BoundsInt bounds = soilTilemap.cellBounds;

        foreach (Vector3Int position in bounds.allPositionsWithin)
        {
            TileBase tile = soilTilemap.GetTile(position);

            if (tile != null)
                originalSoilTiles[position] = tile;
        }

        Debug.Log("Original soil tiles cached: " + originalSoilTiles.Count);
    }

    private void RestoreOriginalSoilTiles()
    {
        if (soilTilemap == null)
            return;

        soilTilemap.ClearAllTiles();

        foreach (KeyValuePair<Vector3Int, TileBase> pair in originalSoilTiles)
        {
            soilTilemap.SetTile(pair.Key, pair.Value);
        }
    }

    public bool IsFarmable(Vector3Int cell)
    {
        return groundTilemap.HasTile(cell);
    }

    public bool IsTilled(Vector3Int cell)
    {
        return farmTiles.ContainsKey(cell) && farmTiles[cell].isTilled;
    }

    public bool IsWatered(Vector3Int cell)
    {
        return farmTiles.ContainsKey(cell) && farmTiles[cell].isWatered;
    }

    public bool HasCrop(Vector3Int cell)
    {
        return farmTiles.ContainsKey(cell) && farmTiles[cell].crop != null;
    }

    public void TillTile(Vector3Int cell)
    {
        if (!groundTilemap.HasTile(cell))
            return;

        if (IsTilled(cell))
            return;

        if (!farmTiles.ContainsKey(cell))
            farmTiles[cell] = new FarmTileData();

        farmTiles[cell].isTilled = true;
        soilTilemap.SetTile(cell, tillledSoilTile);
    }

    public void WaterTile(Vector3Int cell)
    {
        if (!IsTilled(cell))
            return;

        farmTiles[cell].isWatered = true;
        soilTilemap.SetTile(cell, wateredSoilTile);

        if (farmTiles[cell].crop != null)
        {
            Crop crop = farmTiles[cell].crop.GetComponent<Crop>();

            if (crop != null)
                crop.Water();
        }
    }

    public bool PlantCrop(Vector3Int cell, GameObject cropObject)
    {
        if (!IsTilled(cell) || HasCrop(cell))
            return false;

        if (!farmTiles.ContainsKey(cell))
            farmTiles[cell] = new FarmTileData();

        farmTiles[cell].crop = cropObject;
        return true;
    }

    public Vector3 GetCellCenterWorld(Vector3Int cell)
    {
        return groundTilemap.GetCellCenterWorld(cell);
    }

    public GameObject GetCrop(Vector3Int cell)
    {
        if (!farmTiles.ContainsKey(cell))
            return null;

        return farmTiles[cell].crop;
    }

    public void RemoveCrop(Vector3Int cell)
    {
        if (!farmTiles.ContainsKey(cell))
            return;

        farmTiles[cell].crop = null;
    }

    public List<FarmTileSaveData> GetFarmSaveData()
    {
        List<FarmTileSaveData> saveData = new List<FarmTileSaveData>();

        foreach (KeyValuePair<Vector3Int, FarmTileData> pair in farmTiles)
        {
            Vector3Int cell = pair.Key;
            FarmTileData tile = pair.Value;

            FarmTileSaveData data = new FarmTileSaveData
            {
                x = cell.x,
                y = cell.y,
                z = cell.z,
                isTilled = tile.isTilled,
                isWatered = tile.isWatered,
                hasCrop = tile.crop != null,
                cropItemName = "",
                cropGrowthStage = 0,
                cropWatered = false
            };

            if (tile.crop != null)
            {
                Crop crop = tile.crop.GetComponent<Crop>();

                if (crop != null && crop.cropData != null)
                {
                    data.cropItemName = crop.cropData.itemName;
                    data.cropGrowthStage = crop.GetGrowthStage();
                    data.cropWatered = crop.IsWatered();
                }
            }

            saveData.Add(data);
        }

        return saveData;
    }

    public void LoadFarmSaveData(List<FarmTileSaveData> loadedTiles)
    {
        ClearFarmStateForLoad();

        if (loadedTiles == null)
            return;

        foreach (FarmTileSaveData data in loadedTiles)
        {
            Vector3Int cell = new Vector3Int(data.x, data.y, data.z);

            if (!farmTiles.ContainsKey(cell))
                farmTiles[cell] = new FarmTileData();

            farmTiles[cell].isTilled = data.isTilled;
            farmTiles[cell].isWatered = data.isWatered;

            if (data.isTilled)
                soilTilemap.SetTile(cell, data.isWatered ? wateredSoilTile : tillledSoilTile);

            if (data.hasCrop && !string.IsNullOrWhiteSpace(data.cropItemName))
            {
                ItemData seedData = null;

                if (InventoryManagern.Instance != null)
                    seedData = InventoryManagern.Instance.GetItemDataByName(data.cropItemName);

                if (seedData != null && seedData.cropPrefab != null)
                {
                    Vector3 pos = GetCellCenterWorld(cell);
                    pos.z = 0f;

                    GameObject cropObject = Instantiate(seedData.cropPrefab, pos, Quaternion.identity);

                    Crop crop = cropObject.GetComponent<Crop>();

                    if (crop != null)
                    {
                        crop.cropData = seedData;
                        crop.plantedCell = cell;
                        crop.LoadCropState(data.cropGrowthStage, data.cropWatered);
                    }

                    farmTiles[cell].crop = cropObject;
                }
                else
                {
                    Debug.LogWarning("FarmManagerA: could not restore crop: " + data.cropItemName);
                }
            }
        }

        Debug.Log("Farm state loaded from JSON.");
    }

    private void ClearFarmStateForLoad()
    {
        foreach (KeyValuePair<Vector3Int, FarmTileData> pair in farmTiles)
        {
            if (pair.Value.crop != null)
                Destroy(pair.Value.crop);
        }

        farmTiles.Clear();

        RestoreOriginalSoilTiles();
    }
}

[System.Serializable]
public class FarmTileData
{
    public bool isTilled;
    public bool isWatered;
    public GameObject crop;
}