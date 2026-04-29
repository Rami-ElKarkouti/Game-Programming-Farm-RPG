using UnityEngine;

public class DemoScript : MonoBehaviour
{
   public InventoryManagern inventoryManager;
   public ItemData[] itemsToPickup;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            inventoryManager.DeleteSelectedItem();
        }
    }

   public void PickupItem(int id)
    {
        bool result = inventoryManager.AddItem(itemsToPickup[id]);
        if (result)
            Debug.Log("Picked up " + itemsToPickup[id].itemName);
        else
            Debug.Log("Failed to pick up " + itemsToPickup[id].itemName);
    }

    public void GetSelectedItem()
    {
        ItemData recievedItem = inventoryManager.GetSelectedItem(false);
        if (recievedItem != null)
        {
            Debug.Log("Selected item: " + recievedItem.itemName);
        }
        else
        {
            Debug.Log("No item selected");
        }
    }

    public void UseSelectedItem()
    {
        ItemData recievedItem = inventoryManager.GetSelectedItem(true);
        if (recievedItem != null)
        {
            Debug.Log("Used item: " + recievedItem.itemName);
        }
        else
        {
            Debug.Log("No item used");
        }
    }

    void DeleteSelectedItem()
    {
        inventoryManager.DeleteSelectedItem();
    }

}
