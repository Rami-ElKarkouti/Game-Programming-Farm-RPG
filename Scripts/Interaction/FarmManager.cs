using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FarmManager : MonoBehaviour
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
        if (!IsFarmable(cell)) return;

        if (!farmTiles.ContainsKey(cell))
            farmTiles[cell] = new FarmTileData();

        farmTiles[cell].isTilled = true;
        soilTilemap.SetTile(cell, tillledSoilTile);
    }

    public void WaterTile(Vector3Int cell)
    {
        if (!IsTilled(cell)) return;

        farmTiles[cell].isWatered = true;
        soilTilemap.SetTile(cell, wateredSoilTile);
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
}

[System.Serializable]
public class FarmTileData
{
    public bool isTilled;
    public bool isWatered;
    public GameObject crop;
}
