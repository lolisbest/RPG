using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Common;

public class EquipSlot : MonoBehaviour
{
    public int ItemId;
    public Image ItemIcon;

    public void Equip(int itemId)
    {
        StructItemData itemData = DataBase.Items[itemId];
        if(itemData.ItemType != EnumItemType.Equipment)
        {
            Debug.Log($"Not Equipment : {itemId}");
        }

        ItemId = itemData.Id;
        ItemIcon.sprite = itemData.Sprite;
        ItemIcon.enabled = true;
    }

    public void Unequip()
    {
        ItemId = -1;
        ItemIcon.sprite = null;
        ItemIcon.enabled = false;
    }
}
