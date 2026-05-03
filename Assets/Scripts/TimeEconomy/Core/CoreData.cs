using System.Collections.Generic;
using UnityEngine.Events;

public enum Season
{
    Spring,
    Summer,
    Fall,
    Winter
}

[System.Serializable]
public class TimeChangedEvent : UnityEvent<int, int, int> { }

public static class GameEventSystem
{
    public static TimeChangedEvent timeChanged = new TimeChangedEvent();
    public static UnityEvent newDayStarted = new UnityEvent();
    public static UnityEvent moneyChanged = new UnityEvent();
    public static UnityEvent inventoryChanged = new UnityEvent();
    public static UnityEvent shopStockChanged = new UnityEvent();
}

[System.Serializable]
public class InventoryItemData
{
    public string ItemName;
    public int Quantity;

    public InventoryItemData()
    {
        ItemName = "";
        Quantity = 0;
    }

    public InventoryItemData(string itemName, int quantity)
    {
        ItemName = itemName;
        Quantity = quantity;
    }
}

[System.Serializable]
public class ShopItem
{
    public string itemName;
    public int price;
    public int sellPrice;
    public int stock;
}

[System.Serializable]
public class FarmTileSaveData
{
    public int x;
    public int y;
    public int z;

    public bool isTilled;
    public bool isWatered;

    public bool hasCrop;
    public string cropItemName;
    public int cropGrowthStage;
    public bool cropWatered;
}

[System.Serializable]
public class ResourceNodeSaveData
{
    public string resourceId;
    public int currentHealth;
    public bool depleted;
}

[System.Serializable]
public class SaveData
{
    public int Money;
    public int Hour;
    public int Minute;
    public int DayNumber;
    public Season Season;

    public List<InventorySaveData> inventory = new List<InventorySaveData>();
    public int SelectedInventorySlot;

    public List<int> ShopStock = new List<int>();

    public float PlayerX;
    public float PlayerY;
    public float PlayerZ;

    public int CurrentStamina;
    public int MaxStamina;

    public List<FarmTileSaveData> FarmTiles = new List<FarmTileSaveData>();
    public List<ResourceNodeSaveData> ResourceNodes = new List<ResourceNodeSaveData>();
}