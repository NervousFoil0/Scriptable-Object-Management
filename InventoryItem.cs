using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class InventoryItem : InventoryBase, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [SerializeField]
    private Canvas Canvas;
    [SerializeField]
    private CanvasGroup CanvasGroup;

    public RectTransform RectTransform;

    public Transform inventoryParent;

    public Vector3 startPosition;

    public bool isDropped;

    public BaseItem thisItem;

    public TextMeshProUGUI stackDisplay;
    private void Start()
    {
        Canvas = GameObject.FindGameObjectWithTag("MainGUI").GetComponent<Canvas>();
        CanvasGroup = GetComponent<CanvasGroup>();
        RectTransform = GetComponent<RectTransform>();
        startPosition = RectTransform.anchoredPosition; //Sets the start position when we go to drag the item.
        isDropped = true;
        inventoryParent = transform.parent;
        Invoke("InitItemData", 0.1f); //Seriously, no idea what's happened here. I am guessing it needs to trigger the next frame?  
    }
    public void InitItemData()
    {
        RectTransform.sizeDelta = new Vector2(50f * thisItem.itemSize.x, 50f * thisItem.itemSize.y);
        for(int x = 0; x < thisItem.itemSize.x; x++)
        {
            for(int y = 0; y < thisItem.itemSize.y; y++)
            {
                RectTransform collider = Instantiate(collidersPrefab, transform).GetComponent<RectTransform>();
                collider.localPosition = new Vector2(50f * x, -50f * y);
            }
        }
        itemColliders = GetComponentsInChildren<ItemCollider>();
        if(thisItem != null)
        {
            GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/ItemIcons/Tier" + ((int)thisItem.itemTier[0]+1) +"/"+thisItem.Type+"/"+thisItem.itemID);// thisItem.itemIcon;
            GetComponent<Image>().color = Color.white;
        }
        else
        {
            GetComponent<Image>().sprite = null;
            GetComponent<Image>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }
    }

    public void Update()
    {
        //if (!isDropped)
        //{
        //    if (Input.GetButtonDown("Horizontal"))
        //    {
        //        RectTransform.localEulerAngles = new Vector3(0, 0, RectTransform.rotation.z + (90 * Input.GetAxisRaw("Horizontal")));
        //    }
        //}
    }

    #region Event System
    public void OnPointerEnter(PointerEventData eventData)
    {

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ////Delete item info here.
        //Destroy(DataTransfer.Instance.CurrentIndicator);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        //Left Mouse = 0, Right Mouse = 1 , Middle = 2
        if (Input.GetMouseButton(0))
        {
            startPosition = RectTransform.anchoredPosition; //Sets the start position when we go to drag the item.
            isDropped = false;
            foreach (ItemCollider col in itemColliders)
            {
                if(col != null)
                {
                    col.inSlot = false;
                    if(col.currentSlot != null)
                    {
                        col.currentSlot.isFilled = false;
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            transform.SetParent(GameObject.FindGameObjectWithTag("MainGUI").transform);
            ItemOptionManager.instance.selectedItem = this;
            ItemOptionManager.instance.SpawnItemOptions(Scene_Manager.instance.currentScene, RectTransform);
            transform.SetParent(inventoryParent);
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //No clue why I put there here musta had a reason.
        CanvasGroup.blocksRaycasts = false;
        transform.SetParent(Canvas.transform);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDropped)
        {
            RectTransform.anchoredPosition += eventData.delta / Canvas.scaleFactor; //Moving the item to the mouse position.
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDropped)
        {
            //Debug.Log("Ending Drag");
            transform.SetParent(inventoryParent);
            RectTransform.anchoredPosition = startPosition; //If we don't drop onto a correct slot it will reset to start position.
            foreach (ItemCollider col in itemColliders)
            {
                if(col.currentSlot != null)
                {
                    col.currentSlot.slotImage.color = col.currentSlot.normal;
                    col.currentSlot.isFilled = false;
                }
                col.inSlot = false;
                col.overItem = false;
            }
        }
        CanvasGroup.blocksRaycasts = true;
    }
    public void OnDrop(PointerEventData eventData)
    {
        //Can't drop on items now.
        UIAudioManager.instance.PlayClip(null, UIAudioManager.instance.inventorySFX.failedDrop);
        Debug.Log("Drop");
    }
    #endregion

    #region Collision System
    [SerializeField]
    public ItemCollider[] itemColliders;
    public GameObject collidersPrefab;
    #endregion
    
    public void DestroyInventoryItem()
    {
        if(inventory.isCustomInventory)
        {
            PlayerStatsManager.instance.playerData.inventoryList[0].inventoryLists[thisItem.inventoryIndex] = null;
        }
        else
        {
            PlayerStatsManager.instance.playerData.inventoryList[inventory.inventoryType].inventoryLists.Remove(thisItem);
            if (inventory == InventoryManager.instance.inventoryGrids[7])
            {
                LootingManager.instance.currentLootSpot.data.lootableItems.Remove(thisItem);
            }
            foreach (ItemCollider itemSlots in itemColliders)
            {
                if(itemSlots != null)
                {
                    if(itemSlots.currentSlot != null)
                    {
                        itemSlots.currentSlot.isFilled = false;
                    }
                }
            }
        }
        ClearCustomSlotData();
        inventory.inventoryItems.Remove(this);
        Destroy(gameObject);
    }

    void ClearCustomSlotData()
    {
        if(inventory.inventoryType == 0)
        {
            if (InventoryManager.instance.bagTypes.Contains(thisItem.Type))
            {
                Debug.Log("Non Custom Slot Drop Clearing bag!");
                inventory.ClearSlots();
            }
        }
    }

    public void UpdateStackDisplay()
    {
        #region Display Stack Size
        if (thisItem.currentStack > 1)
        {
            Debug.Log(thisItem.itemID + " stack : " + thisItem.currentStack.ToString());
            //Is stackable therefore display the stack size.
            stackDisplay.text = thisItem.currentStack.ToString();
        }
        else
        {
            stackDisplay.text = "";
        }
        #endregion
    }
}
