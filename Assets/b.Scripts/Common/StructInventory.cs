using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPG.Common
{
    [Serializable]
    public struct StructInventory
    {
        public StructInventorySlot[] Items;
        public int InventorySlotNumber;
        public int Money;
        public int MaxMoney;
        public override string ToString()
        {
            return $"Items : [{string.Join(", ", Items)}, Money:{Money}]\n" +
                $"InventorySlotNumber : {InventorySlotNumber}, MaxMoney : {MaxMoney}";
        }

        public static StructInventory GetTempData()
        {
            StructInventory inventory = new();
            inventory.InventorySlotNumber = 36;
            inventory.Items = new StructInventorySlot[0];
            inventory.Money = 0;
            inventory.MaxMoney = 99999;

            return inventory;
        }
    }
}