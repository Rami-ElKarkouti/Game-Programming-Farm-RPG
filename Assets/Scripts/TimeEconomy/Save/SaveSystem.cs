using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    [Header("Extra Save References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerStamina playerStamina;
    [SerializeField] private FarmManagerA farmManager;

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
        if (!CanUseSystems())
            return;

        SaveData data = new SaveData
        {
            Money = MoneySystem.Instance.CurrentMoney,

            Hour = TimeSystem.Instance.Hour,
            Minute = TimeSystem.Instance.Minute,
            DayNumber = TimeSystem.Instance.DayNumber,
            Season = TimeSystem.Instance.CurrentSeason,

            inventory = InventoryManagern.Instance.GetInventoryData(),
            SelectedInventorySlot = InventoryManagern.Instance.GetSelectedSlot(),

            ShopStock = ShopSystem.Instance != null ? ShopSystem.Instance.GetStockData() : new List<int>(),

            PlayerX = playerTransform.position.x,
            PlayerY = playerTransform.position.y,
            PlayerZ = playerTransform.position.z,

            CurrentStamina = playerStamina.GetCurrentStamina(),
            MaxStamina = playerStamina.GetMaxStamina(),

            FarmTiles = farmManager.GetFarmSaveData(),
            ResourceNodes = GetResourceNodeSaveData()
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

        if (!CanUseSystems())
            return;

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        MoneySystem.Instance.LoadMoney(data.Money);
        TimeSystem.Instance.LoadTime(data.Hour, data.Minute, data.DayNumber, data.Season);

        InventoryManagern.Instance.LoadInventoryData(data.inventory, data.SelectedInventorySlot);

        if (ShopSystem.Instance != null)
            ShopSystem.Instance.LoadStockData(data.ShopStock);

        playerTransform.position = new Vector3(data.PlayerX, data.PlayerY, data.PlayerZ);

        if (data.MaxStamina > 0)
            playerStamina.LoadStamina(data.CurrentStamina, data.MaxStamina);

        farmManager.LoadFarmSaveData(data.FarmTiles);
        LoadResourceNodeSaveData(data.ResourceNodes);

        Debug.Log("Game Loaded Successfully");
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted.");
        }
        else
        {
            Debug.LogWarning("No save file to delete.");
        }
    }

    private List<ResourceNodeSaveData> GetResourceNodeSaveData()
    {
        List<ResourceNodeSaveData> data = new List<ResourceNodeSaveData>();

        ResourceNode[] nodes = FindObjectsByType<ResourceNode>(FindObjectsSortMode.None);

        foreach (ResourceNode node in nodes)
        {
            if (node != null)
                data.Add(node.GetSaveData());
        }

        return data;
    }

    private void LoadResourceNodeSaveData(List<ResourceNodeSaveData> savedNodes)
    {
        if (savedNodes == null)
            return;

        ResourceNode[] nodes = FindObjectsByType<ResourceNode>(FindObjectsSortMode.None);

        foreach (ResourceNode node in nodes)
        {
            if (node == null)
                continue;

            foreach (ResourceNodeSaveData savedNode in savedNodes)
            {
                if (savedNode == null)
                    continue;

                if (savedNode.resourceId == node.ResourceId)
                {
                    node.LoadSaveData(savedNode);
                    break;
                }
            }
        }
    }

    private bool CanUseSystems()
    {
        bool ok = true;

        if (MoneySystem.Instance == null)
        {
            Debug.LogWarning("SaveSystem: MoneySystem is missing from the scene.");
            ok = false;
        }

        if (TimeSystem.Instance == null)
        {
            Debug.LogWarning("SaveSystem: TimeSystem is missing from the scene.");
            ok = false;
        }

        if (InventoryManagern.Instance == null)
        {
            Debug.LogWarning("SaveSystem: InventoryManagern is missing from the scene.");
            ok = false;
        }

        if (playerTransform == null)
        {
            Debug.LogWarning("SaveSystem: Player Transform is not assigned.");
            ok = false;
        }

        if (playerStamina == null)
        {
            Debug.LogWarning("SaveSystem: PlayerStamina is not assigned.");
            ok = false;
        }

        if (farmManager == null)
        {
            Debug.LogWarning("SaveSystem: FarmManagerA is not assigned.");
            ok = false;
        }

        if (ShopSystem.Instance == null)
        {
            Debug.LogWarning("SaveSystem: ShopSystem is missing. Shop stock will not be saved/loaded.");
        }

        return ok;
    }
}