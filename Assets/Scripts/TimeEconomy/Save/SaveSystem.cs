using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    private string savePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        savePath = Path.Combine(Application.persistentDataPath, "saveData.json");
        Debug.Log("Save Path: " + savePath);
    }

    public void SaveGame()
    {
        if (!CanUseSystems()) return;

        SaveData data = new SaveData
        {
            Money = MoneySystem.Instance.CurrentMoney,
            Hour = TimeSystem.Instance.Hour,
            Minute = TimeSystem.Instance.Minute,
            DayNumber = TimeSystem.Instance.DayNumber,
            Season = TimeSystem.Instance.CurrentSeason,
            inventory = InventorySystem.Instance.GetInventoryData(),
            ShopStock = ShopSystem.Instance.GetStockData()
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log("Game Saved Successfully");
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No save file found.");
            return;
        }

        if (!CanUseSystems()) return;

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        MoneySystem.Instance.LoadMoney(data.Money);
        TimeSystem.Instance.LoadTime(data.Hour, data.Minute, data.DayNumber, data.Season);
        InventorySystem.Instance.LoadInventoryData(data.inventory);
        ShopSystem.Instance.LoadStockData(data.ShopStock);

        Debug.Log("Game Loaded Successfully");
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted.");
        }
    }

    private bool CanUseSystems()
    {
        if (MoneySystem.Instance == null || TimeSystem.Instance == null || InventorySystem.Instance == null || ShopSystem.Instance == null)
        {
            Debug.LogWarning("SaveSystem: one or more required systems are missing from the scene.");
            return false;
        }

        return true;
    }
}
