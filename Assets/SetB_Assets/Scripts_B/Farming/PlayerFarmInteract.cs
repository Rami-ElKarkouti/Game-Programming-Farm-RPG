using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerFarmInteract : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private InventoryManagern inventory;
    [SerializeField] private TileHighlightManager highlight;
    [SerializeField] private FarmManagerA farmManager;

    [Header("Valid Soil Ground")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private TileBase[] validSoilTiles;

    [Header("Resource Nodes")]
    [SerializeField] private LayerMask resourceLayer;
    [SerializeField] private float resourceHitRadius = 0.35f;
    [SerializeField] private PlayerStamina playerStamina;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Interact();
        }
    }

    void Interact()
    {
        Debug.Log("SPACE pressed → Interact started");

        if (inventory == null || highlight == null || farmManager == null)
        {
            Debug.LogError("Missing references!");
            return;
        }

        Vector3Int cell = highlight.GetTargetCell();
        Debug.Log("Target cell: " + cell);

        ItemData item = inventory.GetSelectedItem(false);
        if (item == null)
        {
            Debug.Log("❌ No item selected");
            return;
        }

        switch (item.itemType)
        {
            case ItemType.Tool:
                UseTool(item, cell);
                break;

            case ItemType.Seed:
                PlantSeed(item, cell);
                break;
        }
    }

    void UseTool(ItemData item, Vector3Int cell)
    {
        bool actionValid = false;

        switch (item.itemName)
        {
            case "Hoe":
                actionValid = IsValidSoilGround(cell);
                break;

            case "Watering Can":
                actionValid = true;
                break;

            case "Sickle":
                actionValid = farmManager.HasCrop(cell);
                break;

            case "Axe":
            case "Pickaxe":
                actionValid = true;
                break;
        }

        if (!actionValid)
        {
            Debug.Log("❌ Invalid action");
            return;
        }

        // consume stamina
        if (!playerStamina.UseStamina(item.staminaCost))
        {
            Debug.Log("❌ Not enough stamina");
            return;
        }

        //  perform action
        switch (item.itemName)
        {
            case "Hoe":
                farmManager.TillTile(cell);
                break;

            case "Watering Can":
                farmManager.WaterTile(cell);
                break;

            case "Sickle":
                Harvest(cell);
                break;

            case "Axe":
            case "Pickaxe":
                UseResourceTool(item, cell);
                break;
        }
    }

    bool IsValidSoilGround(Vector3Int cell)
    {
        if (groundTilemap == null)
        {
            Debug.LogError("Ground Tilemap is missing!");
            return false;
        }

        TileBase tileAtCell = groundTilemap.GetTile(cell);

        if (tileAtCell == null)
            return false;

        foreach (TileBase validTile in validSoilTiles)
        {
            if (tileAtCell == validTile)
                return true;
        }

        return false;
    }

    void PlantSeed(ItemData seed, Vector3Int cell)
    {
        if (!farmManager.IsTilled(cell))
        {
            Debug.Log("❌ Must till first");
            return;
        }

        if (farmManager.HasCrop(cell))
        {
            Debug.Log("❌ Already has crop");
            return;
        }

        Vector3 pos = farmManager.GetCellCenterWorld(cell);
        pos.z = 0f;
        GameObject cropObj = Instantiate(seed.cropPrefab, pos, Quaternion.identity);

        Crop crop = cropObj.GetComponent<Crop>();
        if (crop != null)
        {
            crop.cropData = seed;
        }

        bool planted = farmManager.PlantCrop(cell, cropObj);

        if (planted)
        {
            inventory.RemoveFromSelectedSlot(1);
            Debug.Log("🌱 Seed planted");
        }
        else
        {
            Destroy(cropObj);
            Debug.Log("❌ Plant failed");
        }
    }

    void Harvest(Vector3Int cell)
    {
        if (!farmManager.HasCrop(cell))
        {
            Debug.Log("❌ No crop here");
            return;
        }

        GameObject cropObj = farmManager.GetCrop(cell);
        Crop crop = cropObj.GetComponent<Crop>();

        if (crop == null || !crop.IsFullyGrown())
        {
            Debug.Log("❌ Not ready");
            return;
        }

        ItemData harvestItem = crop.GetHarvestItem();

        if (harvestItem != null)
        {
            int amount = Random.Range(
                harvestItem.minYield,
                harvestItem.maxYield + 1
            );

            for (int i = 0; i < amount; i++)
            {
                inventory.AddItem(harvestItem);
            }

            Debug.Log($"🌾 Harvested {amount}x {harvestItem.itemName}");
        }

        farmManager.RemoveCrop(cell);
        Destroy(cropObj);
    }

    void UseResourceTool(ItemData tool, Vector3Int cell)
    {
        if (groundTilemap == null)
        {
            Debug.LogError("Ground Tilemap is missing. Cannot convert cell to world position.");
            return;
        }

        Vector3 hitWorldPosition = groundTilemap.GetCellCenterWorld(cell);
        hitWorldPosition.z = 0f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            hitWorldPosition,
            resourceHitRadius,
            resourceLayer
        );

        if (hits.Length == 0)
        {
            Debug.Log("❌ No tree or rock here.");
            return;
        }

        ResourceNode closestNode = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            ResourceNode node = hit.GetComponentInParent<ResourceNode>();

            if (node == null)
                continue;

            float distance = Vector2.Distance(
                hitWorldPosition,
                hit.ClosestPoint(hitWorldPosition)
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }

        if (closestNode == null)
        {
            Debug.Log("❌ Hit object has no ResourceNode script.");
            return;
        }

        closestNode.TryHit(tool.itemName, inventory);
    }

}