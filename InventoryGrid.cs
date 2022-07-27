using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryGrid : MonoBehaviour
{
    public int inventoryType;
    public List<BaseItem.itemType> inventoryItemType = new List<BaseItem.itemType>();
    public InventorySlot[] inventorySlots;
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();

    [Header("Custom Inventory Grid Vars")]
    public bool isCustomInventory;
    public bool isConsumableSlot;
    public Image inventoryIcon;
    public InventoryGrid childInventory;
    public Vector2Int inventoryGridSize;
    public bool nonPlayerCustomGrid;
    public float xExpandSize;
    public float yExpandSize;
    public bool expandableGridX;
    public bool expandableGridY;
    [Header("Object Containers")]
    public Transform slotParent;
    public Transform itemParent;

    private void Start()
    {
        InitInventoryGrid();
    }
    public void InitInventoryGrid()
    {
        if (!isConsumableSlot)
        {
            if (isCustomInventory)
            {
                InventoryManager.instance.playerInventoryGrids.Add(this);
            }
            else
            {
                InventoryManager.instance.inventoryGrids[inventoryType] = this;
            }
        }
        if (nonPlayerCustomGrid)
        {
            isCustomInventory = true;
        }

        if (slotParent == null)
        {
            slotParent = transform.GetChild(0);
        }
        if (itemParent == null)
        {
            itemParent = transform.GetChild(1);
        }
    }
    public void InitInventorySlots(Vector2Int inventorySize)
    {
        inventoryGridSize = inventorySize;
        //Debug.Log("InitSlots for: " + name);
        for (int x = 0; x < inventorySize.x; x++)
        {
            for (int y = 0; y < inventorySize.y; y++)
            {
                RectTransform slot = Instantiate(InventoryManager.instance.inventorySlot, slotParent).GetComponent<RectTransform>();
                slot.GetComponent<InventorySlot>().slotCoord = new Vector2(x, y);
                slot.GetComponent<InventorySlot>().inventory = this;
                slot.localPosition = new Vector2(25f,-25f) + new Vector2(50 * x, -50f * y);
                slot.name = "x:" + x + ", y: " + y;
                slot.GetComponent<InventorySlot>().allowedItems = inventoryItemType;
            }
        }
        RectTransform slotRect = slotParent.GetComponent<RectTransform>();
        RectTransform itemRect = itemParent.GetComponent<RectTransform>();
        inventorySlots = slotParent.GetComponentsInChildren<InventorySlot>();
        if (expandableGridY)
        {
            slotRect.sizeDelta = new Vector2(slotRect.sizeDelta.x, inventorySize.y * 50);
            itemRect.sizeDelta = new Vector2(itemRect.sizeDelta.x, inventorySize.y * 50);
        }
        if (expandableGridX)
        {
            slotRect.sizeDelta = new Vector2(inventorySize.x * 50, slotRect.sizeDelta.y);
            itemRect.sizeDelta = new Vector2(inventorySize.x * 50, itemRect.sizeDelta.y);
        }
        InitInventoryItems(0, false);
    }
    public Vector2Int AddInventorySlots(Vector2Int addGridSize, Vector2Int oldGridSize, Vector2Int gridOffset)
    {
        Debug.Log("new add : " + addGridSize + " old grid : " + oldGridSize);
        for (int x = oldGridSize.x; x < oldGridSize.x + addGridSize.x; x++)
        {
            Debug.Log("x loop is : " + x);

            for (int y = oldGridSize.y; y < oldGridSize.y + addGridSize.y; y++)
            {
                Debug.Log("y loop is : " + y);
                RectTransform slot = Instantiate(InventoryManager.instance.inventorySlot, slotParent).GetComponent<RectTransform>();
                slot.GetComponent<InventorySlot>().slotCoord = new Vector2((x - gridOffset.x), (y - gridOffset.y));
                slot.GetComponent<InventorySlot>().inventory = this;
                slot.localPosition = new Vector2(25f, -25f) + new Vector2(50 * (x - gridOffset.x), -50f * (y - gridOffset.y));
                slot.name = "x:" + (x - gridOffset.x) + ", y: " + (y - gridOffset.y);
                slot.GetComponent<InventorySlot>().allowedItems = inventoryItemType;
            }
        }
        RectTransform slotRect = slotParent.GetComponent<RectTransform>();
        RectTransform itemRect = itemParent.GetComponent<RectTransform>();
        if (expandableGridY)
        {
            slotRect.sizeDelta = new Vector2(slotRect.sizeDelta.x, (oldGridSize.y + addGridSize.y) * 50);
            itemRect.sizeDelta = new Vector2(itemRect.sizeDelta.x, (oldGridSize.y + addGridSize.y) * 50);
        }
        if (expandableGridX)
        {
            slotRect.sizeDelta = new Vector2((oldGridSize.x + addGridSize.x) * 50, slotRect.sizeDelta.y);
            itemRect.sizeDelta = new Vector2((oldGridSize.x + addGridSize.x) * 50, itemRect.sizeDelta.y);
        }
       return addGridSize;
    }
    public void InitInventoryItems(int itemIndex, bool isSingleItem)
    {
        if (!isSingleItem)
        {
            for (int i = 0; i < PlayerStatsManager.instance.playerData.inventoryList[inventoryType].inventoryLists.Count; i++)
            {
                BaseItem itemData = PlayerStatsManager.instance.playerData.inventoryList[inventoryType].inventoryLists[i];
                if (itemData != null)
                {
                    CreateItem(itemData);
                }
            }
        }
        else
        {
            BaseItem itemData = PlayerStatsManager.instance.playerData.inventoryList[inventoryType].inventoryLists[itemIndex];
            if(itemData != null)
            {
                itemData.itemInventoryPos = Vector2.zero;
                CreateItem(itemData);
                inventoryIcon.enabled = false;
            }
        }
    }
    public void CreateItem(BaseItem itemData)
    {
        //Debug.Log("CreatingItem:" + itemData.itemName);
        if (itemData != null)
        {
            GameObject newItem = Instantiate(InventoryManager.instance.inventoryItem, itemParent.transform);
            newItem.name = itemData.itemName;
            //Debug.Log("Vector2 = " + new Vector2(50 * itemData.itemInventoryPos.x, -50f * itemData.itemInventoryPos.y));
            newItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(50 * itemData.itemInventoryPos.x, -50f * itemData.itemInventoryPos.y);
            newItem.GetComponent<InventoryItem>().thisItem = itemData; // Instantiate(itemData);
            newItem.GetComponent<InventoryItem>().inventory = this;

            inventoryItems.Add(newItem.GetComponent<InventoryItem>());
            
            #region Display Stack Size
            if (itemData.currentStack > 1)
            {
                //Is stackable therefore display the stack size.
                inventoryItems[inventoryItems.Count - 1].stackDisplay.text = itemData.currentStack.ToString();
            }
            else
            {
                inventoryItems[inventoryItems.Count - 1].stackDisplay.text = "";
            }
            #endregion
        }
    }
    public void ClearSlots()
    {
        if (isCustomInventory)
        {
            foreach (InventorySlot slot in childInventory.inventorySlots)
            {
                Destroy(slot.gameObject);
            }
        }
        else
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                Destroy(slot.gameObject);
            }
        }
    }
    public void ClearItems()
    {
        for(int i = 0; i < itemParent.childCount; i++)
        {
            Destroy(itemParent.GetChild(i).gameObject);
        }

        inventoryItems.Clear();
    }
    public void UpdateItemStacks()
    {
        for(int i = 0; i < inventoryItems.Count; i++)
        {
            if(inventoryItems[i] != null)
            {
                #region Display Stack Size
                inventoryItems[i].UpdateStackDisplay();
                #endregion
            }
        }
    }

    //Spawn Items on Grid Voids
    public bool SpawnItem(BaseItem spawnItem)
    {
        Debug.Log("Spawning!");

        #region Check For Same Item
        bool createItem = false;
        if(spawnItem.maxStack > 1)
        {
            for(int i = 0; i < inventoryItems.Count; i++)
            {
                if(spawnItem.currentStack > 0)
                {
                    if (inventoryItems[i].thisItem.itemID == spawnItem.itemID)
                    {
                        int totalStackSize = inventoryItems[i].thisItem.currentStack + spawnItem.currentStack;
                        if (totalStackSize > inventoryItems[i].thisItem.maxStack)
                        {
                            createItem = true;
                            //150 + 210 = 360 / 300
                            int neededStack = inventoryItems[i].thisItem.maxStack - inventoryItems[i].thisItem.currentStack;
                            inventoryItems[i].thisItem.currentStack = inventoryItems[i].thisItem.maxStack;
                            spawnItem.currentStack -= neededStack;
                            Debug.Log("Stacking! create");
                            inventoryItems[i].UpdateStackDisplay();
                        }
                        else if (totalStackSize == inventoryItems[i].thisItem.maxStack)
                        {
                            inventoryItems[i].thisItem.currentStack = inventoryItems[i].thisItem.maxStack;
                            inventoryItems[i].UpdateStackDisplay();
                            Debug.Log("Stacking! Non-Create(Maxed)!");
                            break;
                        }
                        else
                        {
                            inventoryItems[i].thisItem.currentStack += spawnItem.currentStack;
                            inventoryItems[i].UpdateStackDisplay();
                            Debug.Log("Stacking! Non-Create(Non-Maxed)!");
                            break;
                        }
                    }
                }
            }

            if(spawnItem.currentStack > 0)
            {
                createItem = true;
            }
        }
        else
        {
            createItem = true;
        }
        #endregion

        if (createItem)
        {
            for (int y = 0; y < inventoryGridSize.y; y++)
            {
                for (int x = 0; x < inventoryGridSize.x; x++)
                {
                    string slotCoord = "x:" + x + ", y: " + y;
                    //Debug.Log(slotCoord);
                    InventorySlot slot = slotParent.Find(slotCoord).GetComponent<InventorySlot>();
                    if (!slot.isFilled)
                    {
                        //Debug.Log("(" + x + "," + y + ") is empty");
                        bool validDrop = true;
                        for (int a = y + 0; a < y + spawnItem.itemSize.y; a++)
                        {
                            //Debug.Log("a :" + a);
                            if (validDrop)
                            {
                                for (int b = x + 0; b < x + spawnItem.itemSize.x; b++)
                                {
                                    if (slot.isFilled)
                                    {
                                        Debug.Log("invalid spawn checking new slot");
                                        validDrop = false;
                                        break;
                                    }
                                    else
                                    {
                                        string checkSlotCoor = "x:" + (x + spawnItem.itemSize.x - 1) + ", y: " + (y + spawnItem.itemSize.y - 1);
                                        //Debug.Log("checkSlotCoor = " + checkSlotCoor);
                                        if (inventoryGridSize.x > (x + spawnItem.itemSize.x - 1) && inventoryGridSize.y > (y + spawnItem.itemSize.y - 1))
                                        {
                                            //Debug.Log("Within grid limits");
                                            InventorySlot checkSlot = slotParent.Find(checkSlotCoor).GetComponent<InventorySlot>();
                                            if (!checkSlot.isFilled)
                                            {
                                                if (a == (y + spawnItem.itemSize.y - 1) && b == (x + spawnItem.itemSize.x - 1))
                                                {
                                                    //Debug.Log("Valid Drop!");
                                                    //Debug.Log(spawnItem.itemID + " : dropped on :" + x + "," + y);
                                                    spawnItem.itemInventoryPos = new Vector2Int(x, y);
                                                    CreateItem(spawnItem);
                                                    return true;
                                                }
                                            }
                                            else
                                            {
                                                validDrop = InvalidDrop();
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            validDrop = InvalidDrop();
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        //Debug.Log("(" + x + "," + y + ") is filled moving on");
                    }
                }
            }
        }
        return false;
    }
    bool InvalidDrop()
    {
        Debug.Log("invalid spawn checking new slot");
        return false;
    }
}
