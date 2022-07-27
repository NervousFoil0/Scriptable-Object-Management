using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryBase : MonoBehaviour
{
    public InventoryGrid inventory;
    public bool isCustomSlot;
    public int customSlotIndex;
    public bool ValidDrop(InventoryItem droppedItem, InventorySlot slot)
    {
        if(!droppedItem) { return false; }

        foreach (ItemCollider col in droppedItem.itemColliders)
        {
            if (!col.inSlot)
            {
                //Debug.Log(col.name + " is not in slot");
                //Debug.Log("InvalidDrop");
                return false;
            }
        }
        for (int i = 0; i < droppedItem.itemColliders.Length; i++)
        {
            if(droppedItem.itemColliders[0].currentSlot.inventory != droppedItem.itemColliders[i].currentSlot.inventory)
            {
                Debug.Log("Item is hovered over 2 different grids(Invalid)");
                return false;
            }
        }
        if (slot.allowedItems.Count > 0)
        {
            if (!slot.allowedItems.Contains(droppedItem.thisItem.Type))
            {
                Debug.Log("Custom Slot doesn't have this type");
                return false;
            }
            else
            {
                switch (droppedItem.thisItem.Type)
                {
                    case BaseItem.itemType.PrimaryWeapon:

                        return WeaponStatCheck(droppedItem.thisItem);

                        break;

                    case BaseItem.itemType.SecondaryWeapon:

                        return WeaponStatCheck(droppedItem.thisItem);

                        break;

                    case BaseItem.itemType.Medicine:

                        //Debug.Log("THIS IS CUSTOM MED SLOT");

                        if(MainGUI.instance != null)
                        {
                            if (PlayerStatsManager.instance.playerData.inventoryList[0].inventoryLists[14] == null)
                            {
                                MainGUI.instance.UpdateMedicineDisplay(droppedItem);
                            }
                            else
                            {
                                return false;
                            }
                        }

                        break;

                    case BaseItem.itemType.Equipment:

                        //Debug.Log("THIS IS CUSTOM Equipment SLOT");

                        if (MainGUI.instance != null)
                        {
                            if(PlayerStatsManager.instance.playerData.inventoryList[0].inventoryLists[13] == null)
                            {
                                MainGUI.instance.UpdateEquipmentDisplay(droppedItem);
                            }
                            else
                            {
                                return false;
                            }
                        }

                        break;

                    case BaseItem.itemType.Trap:

                        //Debug.Log("THIS IS CUSTOM Equipment SLOT");

                        if (MainGUI.instance != null)
                        {
                            if (PlayerStatsManager.instance.playerData.inventoryList[0].inventoryLists[13] == null)
                            {
                                MainGUI.instance.UpdateEquipmentDisplay(droppedItem);
                            }
                            else
                            {
                                return false;
                            }
                        }

                        break;

                    case BaseItem.itemType.Crafting:

                        //Debug.Log("THIS IS A CRAFTING GRID SLOT");

                        Crafting.instance.AddCraftingMaterial((CraftingItem)droppedItem.thisItem);

                        break;
                }
            }
        }

        Vector2Int checkValue = Vector2Int.zero;

        checkValue = CheckItemOnPlayer(droppedItem, 0);

        if (checkValue.x == 1)
        {
            switch (droppedItem.thisItem.Type)
            {
                case BaseItem.itemType.Tactical_Vest:
                    return CheckBagState(slot, 1);
                    break;
                case BaseItem.itemType.Backpack:
                    return CheckBagState(slot, 2);
                    break;
                case BaseItem.itemType.Trousers:
                    return CheckBagState(slot, 3);
                    break;
            }
        }
        //Debug.Log("ValidDrop");
        return true;
    }
    bool WeaponStatCheck(BaseItem weapon)
    {
        Debug.Log("Working Wep Check!");
        bool isValid = false;
        WeaponItem wep = (WeaponItem)weapon;
        switch (wep.weaponType)
        {
            case WeaponItem.WeaponType.Melee:
                isValid = ReturnWeaponCheck(PlayerStatsManager.instance.playerData.melee, wep);
                break;
            case WeaponItem.WeaponType.MotorMelee:
                isValid = ReturnWeaponCheck(PlayerStatsManager.instance.playerData.melee, wep);
                break;
            case WeaponItem.WeaponType.Pistol:
                isValid = ReturnWeaponCheck(PlayerStatsManager.instance.playerData.pistol, wep);
                break;
            case WeaponItem.WeaponType.Rifle:
                isValid = ReturnWeaponCheck(PlayerStatsManager.instance.playerData.rifle, wep);
                break;
            case WeaponItem.WeaponType.AssaultRifle:
                isValid = ReturnWeaponCheck(PlayerStatsManager.instance.playerData.machinegun, wep);
                break;
            case WeaponItem.WeaponType.SubmachineGun:
                isValid = ReturnWeaponCheck(PlayerStatsManager.instance.playerData.machinegun, wep);
                break;
            case WeaponItem.WeaponType.Shotgun:
                isValid = ReturnWeaponCheck(PlayerStatsManager.instance.playerData.shotgun, wep);
                break;
            case WeaponItem.WeaponType.Explosive:
                isValid = ReturnWeaponCheck(PlayerStatsManager.instance.playerData.explosive, wep);
                break;
        }
        if (isValid)
        {
            if(PlayerMovement.instance.entityWeaponManager != null)
            {
                PlayerMovement.instance.entityWeaponManager.SwapWeapon();
            }
        }
        return isValid;
    }
    bool ReturnWeaponCheck(int playerWepStat, WeaponItem wep)
    {
        bool isValid = false;
        if (playerWepStat >= wep.requirements[0])
        {
            Debug.Log("Has Right Weapon Stats");
            if (PlayerStatsManager.instance.playerData.strength >= wep.requirements[1])
            {
                Debug.Log("Has Right Strength Stats!");
                isValid = true;
                return isValid;
            }
            else
            {
                Debug.Log("Has weapon stats, but not strength");
                return isValid;
            }
        }
        else
        {
            Debug.Log("Hasn't got weapon stats");
            return isValid;
        }
    }
    public void DroppedItem(InventoryItem droppedItem)
    {
        UIAudioManager.instance.PlayClip(null, UIAudioManager.instance.inventorySFX.successfulDrop);

        foreach (ItemCollider col in droppedItem.itemColliders)
        {
            col.currentSlot.slotImage.color = col.currentSlot.normal;
        }
        droppedItem.isDropped = true;

        if(droppedItem.inventory == inventory)
        {
            //Debug.Log("SameInventory");
            droppedItem.transform.SetParent(droppedItem.inventoryParent);

            if (droppedItem.inventory == InventoryManager.instance.inventoryGrids[8])
            {
                Crafting.instance.RemoveCraftingMaterial((CraftingItem)droppedItem.thisItem);
            }
        }
        else
        {
            //Debug.Log("Different Inventory");
            if (isCustomSlot)
            {
                #region Icon Activation
                if (droppedItem.inventory.inventoryIcon != null)
                {
                    droppedItem.inventory.inventoryIcon.enabled = true;
                }
                if(inventory.inventoryIcon != null)
                {
                    inventory.inventoryIcon.enabled = false;
                }
                #endregion

                CustomSlotDrop(droppedItem);
            }
            else
            {
                #region Icon Activation
                if (droppedItem.inventory.inventoryIcon != null)
                {
                    droppedItem.inventory.inventoryIcon.enabled = true;
                }
                #endregion

                NonCustomSlotDrop(droppedItem);
            }
            droppedItem.transform.SetParent(inventory.itemParent);
            droppedItem.inventory.inventoryItems.Remove(droppedItem);
            inventory.inventoryItems.Add(droppedItem);
            droppedItem.inventoryParent = inventory.itemParent;
            droppedItem.inventory = inventory;
            Debug.Log(droppedItem.thisItem.itemID);
            PlayerStatsManager.instance.cosmeticSystem.ChangeCosmetic(droppedItem.thisItem.Type);
        }
        //Debug.Log(droppedItem.itemColliders[0].currentSlot.name);
        droppedItem.RectTransform.anchoredPosition = droppedItem.itemColliders[0].currentSlot.GetComponent<RectTransform>().anchoredPosition + new Vector2(-25, 25);
        droppedItem.thisItem.itemInventoryPos = droppedItem.itemColliders[0].currentSlot.slotCoord;
    }
    void CustomSlotDrop(InventoryItem droppedItem)
    {
        Debug.Log("InventoryType[" + inventory.inventoryType + "], index[" + customSlotIndex + "]");
        Vector2Int check = CheckItemOnPlayer(droppedItem, 0);
        if (check.x == 1)
        {
            //Debug.Log("x = 1(1)");
            //inventory.inventoryIcon.enabled = true;
            Debug.LogError("This is where the custom slot drop is triggered from loot 1");
            PlayerStatsManager.instance.playerData.inventoryList[0].inventoryLists[check.y] = null;
            if (InventoryManager.instance.bagTypes.Contains(droppedItem.thisItem.Type))
            {
                //Debug.Log("x = 1(1) working!");
                Debug.Log("This is what is triggering? 1");
                droppedItem.inventory.ClearSlots();
            }
        }
        else
        {
            #region unique inventory swap tasks

            #endregion
            //inventory.inventoryIcon.enabled = false;
            Debug.LogError("This is where the custom slot drop is triggered from loot 0"); 
            if (droppedItem.inventory == InventoryManager.instance.inventoryGrids[7])
            {
                LootingManager.instance.currentLootSpot.data.lootableItems.Remove(droppedItem.thisItem);
            }
            else
            {
                PlayerStatsManager.instance.playerData.inventoryList[droppedItem.inventory.inventoryType].inventoryLists.Remove(droppedItem.thisItem);
            }

            if (InventoryManager.instance.bagTypes.Contains(droppedItem.thisItem.Type))
            {
                //Debug.Log("Loading New Bag");
                Debug.Log("This is what is triggering? 2");
                inventory.childInventory.InitInventorySlots(((BagItem)droppedItem.thisItem).bagSize);
            }
            droppedItem.thisItem.inventoryIndex = customSlotIndex;
        }

        PlayerStatsManager.instance.playerData.inventoryList[inventory.inventoryType].inventoryLists[customSlotIndex] = droppedItem.thisItem;
    }
    void NonCustomSlotDrop(InventoryItem droppedItem)
    {
        Vector2Int check = CheckItemOnPlayer(droppedItem, 0);
        PlayerStatsManager.instance.playerData.inventoryList[inventory.inventoryType].inventoryLists.Add(droppedItem.thisItem);
        if (check.x == 1)
        {
            //Debug.Log("x = 1(2)");
            if (InventoryManager.instance.bagTypes.Contains(droppedItem.thisItem.Type))
            {
                //Debug.Log("This is working 2!");
                //Debug.Log("Inventory Clear: " + droppedItem.inventory.name);
                Debug.Log("Non Custom Slot Drop Clearing bag!");
                droppedItem.inventory.ClearSlots();
            }
            //inventory.inventoryIcon.enabled = true;
            PlayerStatsManager.instance.playerData.inventoryList[0].inventoryLists[check.y] = null;
        }
        else
        {
            #region unique inventory swap tasks
            //This handles unique slot > normal slot (Example Crafting Grid > Inventory)
            //We want to remove items from the crafting section so we will execute the code here.
            if (droppedItem.inventory == InventoryManager.instance.inventoryGrids[7])
            {
                Debug.Log("This is item display container...");
                LootingManager.instance.currentLootSpot.data.lootableItems.Remove(droppedItem.thisItem);
            }
            else
            {
                PlayerStatsManager.instance.playerData.inventoryList[droppedItem.inventory.inventoryType].inventoryLists.Remove(droppedItem.thisItem);
            }
            if (droppedItem.inventory == InventoryManager.instance.inventoryGrids[8])
            {
                Crafting.instance.RemoveCraftingMaterial((CraftingItem)droppedItem.thisItem);
            }
            #endregion
        }
    }

    /// <summary>
    /// This function will return a vector 2 value with x being 1 or 0 (bool check) and y being the index of the item within the list. This is used for bag checks on player.
    /// </summary>
    /// <param name="droppedItem"></param>
    /// <param name="inventoryType"></param>
    /// <returns></returns>
    Vector2Int CheckItemOnPlayer(InventoryItem droppedItem, int inventoryType)
    {
        //x = bool check, y = index check
        if (droppedItem.inventory.inventoryType == inventoryType)
        {
            for (int i = 0; i < PlayerStatsManager.instance.playerData.inventoryList[inventoryType].inventoryLists.Count; i++)
            {
                if (PlayerStatsManager.instance.playerData.inventoryList[inventoryType].inventoryLists[i] == droppedItem.thisItem)
                {
                    Debug.Log("Found Item on player! We will set to null at : " + i);
                    Vector2Int check = new Vector2Int(1, i);
                    return check;
                }
            }
        }
        return Vector2Int.zero;
    }
    bool CheckBagState(InventorySlot slot, int inventoryType)
    {
        //If this is having issues check the item type on the bag items
        if (PlayerStatsManager.instance.playerData.inventoryList[inventoryType].inventoryLists.Count > 0)
        {
            Debug.Log("bag(" + inventoryType + ") has items can't move this.");
            return false;
        }
        else
        {
            if (slot.inventory.inventoryType == inventoryType)
            {
                Debug.Log("Can't drop bag("+ inventoryType+") on itself");
                return false;
            }
        }
        return true;
    }
}
