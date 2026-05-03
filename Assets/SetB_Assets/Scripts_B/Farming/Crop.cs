using UnityEngine;

public class Crop : MonoBehaviour
{
    public Vector3Int plantedCell;
    public ItemData cropData;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private int growthStage = 0;
    private bool watered = false;
    private float growthTimer = 0f;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        ApplyVisual();
    }

    private void Update()
    {
        if (cropData == null)
            return;

        if (!watered)
            return;

        if (IsFullyGrown())
            return;

        growthTimer += Time.deltaTime;

        if (growthTimer >= cropData.timeBetweenGrowth)
        {
            growthTimer = 0f;
            growthStage++;
            watered = false;
            ApplyVisual();

            Debug.Log("Crop grew to stage " + growthStage);
        }
    }

    public void Water()
    {
        watered = true;
    }

    public bool IsFullyGrown()
    {
        if (cropData == null)
            return false;

        return growthStage >= cropData.maxGrowthStage;
    }

    public ItemData GetHarvestItem()
    {
        if (cropData == null)
            return null;

        return cropData.harvestItem;
    }

    public int GetGrowthStage()
    {
        return growthStage;
    }

    public bool IsWatered()
    {
        return watered;
    }

    public string GetCropItemName()
    {
        if (cropData == null)
            return "";

        return cropData.itemName;
    }

    public void LoadCropState(int loadedGrowthStage, bool loadedWatered)
    {
        growthStage = Mathf.Max(0, loadedGrowthStage);
        watered = loadedWatered;
        growthTimer = 0f;

        ApplyVisual();
    }

    private void ApplyVisual()
    {
        if (spriteRenderer == null)
            return;

        if (cropData == null)
            return;

        if (cropData.growthSprites == null || cropData.growthSprites.Length == 0)
            return;

        int spriteIndex = Mathf.Clamp(growthStage, 0, cropData.growthSprites.Length - 1);
        spriteRenderer.sprite = cropData.growthSprites[spriteIndex];
    }
}