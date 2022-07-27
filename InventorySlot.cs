using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : InventoryBase, IDropHandler
{
    public bool isFilled;
    public Vector2 slotCoord;
    public Color normal;
    public Color highlighted;
    public Image slotImage;

    public enum SpecialSlot {None, CosmeticStatBoost, MasterCosmeticStatBoost, ScrapSlot, RecycleSlot};
    public SpecialSlot slotType;

    /// <summary>
    /// If there is no items allowed then it means that there is no restrictions on this slot.
    /// </summary>
    public List<BaseItem.itemType> allowedItems = new List<BaseItem.itemType>();
    private void Start()
    {
        slotImage = GetComponent<Image>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log(eventData.pointerDrag.name);
        if (ValidDrop(eventData.pointerDrag.GetComponent<InventoryItem>(), this))
        {
            InventoryItem droppedItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            if(slotType == SpecialSlot.None)
            {
                DroppedItem(droppedItem);
            }
            else
            {
                switch (slotType)
                {
                    #region Yard System Here!
                    case SpecialSlot.CosmeticStatBoost:
                        YardSystem.instance.CosmeticStatBoost(droppedItem);
                        break;

                    case SpecialSlot.ScrapSlot:
                        Debug.Log("Scrapping Item!");
                        YardSystem.instance.ScrapItem(droppedItem);
                        break;


                    case SpecialSlot.RecycleSlot:

                        StartCoroutine(YardSystem.instance.RecycleItem(droppedItem));

                        break;

                    #endregion
                }
            }
        }
        else
        {
            UIAudioManager.instance.PlayClip(null, UIAudioManager.instance.inventorySFX.failedDrop);
        }
    }
}
