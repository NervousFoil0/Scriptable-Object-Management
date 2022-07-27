using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    #region Singleton
    public static InventoryManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    GameObject loadResource(string path)
    {
        return Resources.Load<GameObject>(path);    
    }

    public int numberOfInventories;

    public List<InventoryGrid> playerInventoryGrids = new List<InventoryGrid>();
    public List<InventoryGrid> inventoryGrids = new List<InventoryGrid>();

    public GameObject inventorySlot;
    public GameObject inventoryItem;
    public GameObject inventoryItemCollider;

    [Header("Item Type Vars")]
    public List<BaseItem.itemType> bagTypes = new List<BaseItem.itemType>();

    //Called in Singleton, I know you'll forget anders...
    void Init()
    {
        for (int i = 0; i < numberOfInventories; i++)
        {
            inventoryGrids.Add(null);
        }

        inventorySlot = loadResource("UI/Inventory/Slot");
        inventoryItem = loadResource("UI/Inventory/Item");
        inventoryItemCollider = loadResource("UI/Inventory/Collider");
    }
    public void TriggerInventories(int inventType, int inventLocation, int inventIndex)
    {
        //Debug.Log("Inventory Item = " + PlayerStatsManager.instance.playerData.inventoryList[inventLocation].inventoryLists[inventIndex].name);

        BagItem bag = (BagItem)PlayerStatsManager.instance.playerData.inventoryList[inventLocation].inventoryLists[inventIndex];
        if(bag != null)
        {
            inventoryGrids[inventType].InitInventorySlots(bag.bagSize);
        }
        //Debug.Log("Bag is :" + bag.name + "inventory type trigger is : " + inventType);
        //Debug.Log(inventoryGrids[inventType].name);
    }
    public void TriggerCustomInventories(InventoryGrid customInventory)
    {
        customInventory.inventorySlots = customInventory.slotParent.GetComponentsInChildren<InventorySlot>();
        customInventory.InitInventoryItems(customInventory.inventorySlots[0].customSlotIndex, true);      
    }
    public void InitInventories()
    {
        PlayerStatsManager.instance.SetInventoriesSize();
    }
}
