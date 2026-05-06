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
public class AnimalSaveData
{
    public string AnimalId;
    public string AnimalName;
    public string AnimalType;
    public int CurrentHunger;
    public int CurrentHappiness;
}

[System.Serializable]
public class AnimalInteractionSaveData
{
    public int SelectedAnimalIndex;
}

[System.Serializable]
public class ProductionTotalSaveData
{
    public string ItemName;
    public int Quantity;
}

[System.Serializable]
public class AnimalProductionSaveData
{
    public List<ProductionTotalSaveData> ProductionTotals = new List<ProductionTotalSaveData>();
}

[System.Serializable]
public class NPCMovementSaveData
{
    public string NPCId;
    public float PositionX;
    public float PositionY;
    public float PositionZ;
    public float LocalScaleX;
    public int CurrentScheduleHour;
}

[System.Serializable]
public class NPCScheduleManagerSaveData
{
    public int LastProcessedHour;
}

[System.Serializable]
public class NPCDialogueSaveData
{
    public string NPCId;
    public int CurrentRelationship;
}

[System.Serializable]
public class MockTimeSaveData
{
    public int CurrentHour;
    public int CurrentDay;
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


    public List<AnimalSaveData> Animals = new List<AnimalSaveData>();
    public AnimalInteractionSaveData AnimalInteraction = new AnimalInteractionSaveData();
    public AnimalProductionSaveData AnimalProduction = new AnimalProductionSaveData();

    public List<NPCMovementSaveData> NPCMovements = new List<NPCMovementSaveData>();
    public NPCScheduleManagerSaveData NPCScheduleManager = new NPCScheduleManagerSaveData();
    public List<NPCDialogueSaveData> NPCDialogues = new List<NPCDialogueSaveData>();

    public MockTimeSaveData MockTime = new MockTimeSaveData();
}