using UnityEngine;

public enum ItemType
{
    Tool,
    Seed,
    Crop,
    Resource
}

[CreateAssetMenu(fileName = "New Item", menuName = "FarmRPG/Item")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    public Sprite icon;

    public ItemType itemType;

    [Header("Stack Settings")]
    public bool stackable = true;
    public int maxStack = 99;

    [Header("Economy")]
    public int sellPrice;

    // SEED / CROP SETTINGS

    [Header("Seed Settings")]
    [Tooltip("Prefab that will spawn when this seed is planted")]
    public GameObject cropPrefab;

    [Tooltip("Sprites for crop growth stages")]
    public Sprite[] growthSprites;

    [Tooltip("Number of stages before fully grown")]
    public int maxGrowthStage = 3;

    [Tooltip("Time (seconds) between growth stages AFTER watering")]
    public float timeBetweenGrowth = 5f;

    public ItemData harvestItem;

    [Header("Harvest Settings")]
    public int minYield = 1;
    public int maxYield = 1;

    // TOOL SETTINGS

    [Header("Tool Settings")]
    [Tooltip("Only used if itemType == Tool")]
    public int staminaCost = 10;
}