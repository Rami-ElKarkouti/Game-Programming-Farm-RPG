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
        // ❌ must exist in world
        if (!groundTilemap.HasTile(cell))
            return;

        // ❌ prevent re-tilling same tile
        if (IsTilled(cell))
            return;

        // init data
        if (!farmTiles.ContainsKey(cell))
            farmTiles[cell] = new FarmTileData();

        farmTiles[cell].isTilled = true;

        // visual change
        soilTilemap.SetTile(cell, tillledSoilTile);
    }

    public void WaterTile(Vector3Int cell)
    {
        if (!IsTilled(cell)) return;

        farmTiles[cell].isWatered = true;
        soilTilemap.SetTile(cell, wateredSoilTile);

        // ✅ NEW: water crop
        if (farmTiles[cell].crop != null)
        {
            Crop crop = farmTiles[cell].crop.GetComponent<Crop>();
            if (crop != null)
            {
                crop.Water();
            }
        }
    }

    public bool PlantCrop(Vector3Int cell, GameObject cropObject)
    {
        if (!IsTilled(cell) || HasCrop(cell)) return false;

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
        if (!farmTiles.ContainsKey(cell)) return null;
        return farmTiles[cell].crop;
    }

    public void RemoveCrop(Vector3Int cell)
    {
        if (!farmTiles.ContainsKey(cell)) return;

        farmTiles[cell].crop = null;
    }
}

[System.Serializable]
public class FarmTileData
{
    public bool isTilled;
    public bool isWatered;
    public GameObject crop;
}
