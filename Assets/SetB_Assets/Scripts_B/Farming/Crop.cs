using UnityEngine;

public class Crop : MonoBehaviour
{
    public ItemData cropData;

    private int growthStage = 0;
    private float growthTimer = 0f;

    private bool isWatered = false;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (cropData != null && cropData.growthSprites.Length > 0)
        {
            spriteRenderer.sprite = cropData.growthSprites[0];
        }
    }

    void Update()
    {
        if (!isWatered) return;
        if (IsFullyGrown()) return;

        growthTimer += Time.deltaTime;

        if (growthTimer >= cropData.timeBetweenGrowth)
        {
            Grow();
            growthTimer = 0f;
            isWatered = false; // requires watering again
        }
    }

    public void Water()
    {
        if (IsFullyGrown()) return;

        isWatered = true;
        Debug.Log("Crop watered: " + cropData.itemName);
    }

    void Grow()
    {
        if (growthStage < cropData.maxGrowthStage)
        {
            growthStage++;

            if (cropData.growthSprites.Length > growthStage)
            {
                spriteRenderer.sprite = cropData.growthSprites[growthStage];
            }

            Debug.Log(cropData.itemName + " grew to stage " + growthStage);
        }
    }

    public bool IsFullyGrown()
    {
        return growthStage >= cropData.maxGrowthStage;
    }

    public ItemData GetHarvestItem()
    {
        if (cropData.harvestItem != null)
            return cropData.harvestItem;

        Debug.LogWarning("No harvest item set for " + cropData.itemName);
        return null;
    }
}