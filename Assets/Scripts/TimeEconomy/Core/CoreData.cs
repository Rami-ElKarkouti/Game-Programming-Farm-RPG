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
public class SaveData
{
    public int Money;
    public int Hour;
    public int Minute;
    public int DayNumber;
    public Season Season;
    public List<InventoryItemData> inventory = new List<InventoryItemData>();
    public List<int> ShopStock = new List<int>();
}
