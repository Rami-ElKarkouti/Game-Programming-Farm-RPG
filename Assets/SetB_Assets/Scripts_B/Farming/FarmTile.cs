using UnityEngine;

public class FarmTile : MonoBehaviour
{
    public bool isHoed = false;
    public bool isWatered = false;

    public Crop currentCrop;

    [Header("Soil Sprites")]
    public Sprite normalSoil;   
    public Sprite hoedSoil;     
    public Sprite wateredSoil;  

    public bool CanPlantSeed => isHoed && currentCrop == null;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = normalSoil;
    }

    public void HoeTile()
    {
        if (!isHoed)
        {
            isHoed = true;
            spriteRenderer.sprite = hoedSoil;

            Debug.Log("Tile hoed: " + gameObject.name);
        }
    }

    public void WaterTile()
    {
        if (!isHoed) return;

        isWatered = true;
        spriteRenderer.sprite = wateredSoil;

        Debug.Log("Tile watered: " + gameObject.name);

        if (currentCrop != null && !currentCrop.IsFullyGrown())
        {
            currentCrop.Water();
        }
    }

    public bool PlantSeed(ItemData seedData, GameObject cropPrefab)
    {
        if (!isHoed)
        {
            Debug.Log("Tile must be hoed first!");
            return false;
        }

        if (currentCrop != null)
        {
            Debug.Log("Tile already has a crop!");
            return false;
        }

        if (seedData.itemType != ItemType.Seed)
        {
            Debug.Log("Not a seed!");
            return false;
        }

        if (cropPrefab == null)
        {
            Debug.LogWarning("No crop prefab assigned for seed: " + seedData.itemName);
            return false;
        }

        // spawns crop
        currentCrop = Instantiate(cropPrefab, transform.position, Quaternion.identity, transform)
            .GetComponent<Crop>();

        // assign crop data
        currentCrop.cropData = seedData;

        Debug.Log("Seed planted: " + seedData.itemName);
        return true;
    }

    public void HarvestCrop(InventoryManagern inventory)
    {

        if (currentCrop == null)
            return;

        if (!currentCrop.IsFullyGrown())
        {
            Debug.Log("Crop not ready!");
            return;
        }

        ItemData harvestItem = currentCrop.GetHarvestItem();

        if (harvestItem != null)
        {
            int yieldAmount = Random.Range(currentCrop.cropData.minYield, currentCrop.cropData.maxYield + 1);

            for (int i = 0; i < yieldAmount; i++)
            {
                inventory.AddItem(harvestItem);
            }

            Debug.Log("Harvested: " + harvestItem.itemName + " x" + yieldAmount);
        }

        Destroy(currentCrop.gameObject);
        currentCrop = null;

        // reset water state after harvest
        isWatered = false;
        spriteRenderer.sprite = hoedSoil;
    }

    public void ResetWater()
    {
        isWatered = false;

        if (isHoed)
            spriteRenderer.sprite = hoedSoil;
        else
            spriteRenderer.sprite = normalSoil;
    }
}