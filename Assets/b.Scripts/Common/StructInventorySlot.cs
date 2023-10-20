using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPG.Common
{
    [Serializable]
    public struct StructInventorySlot
    {
        public int ItemId;
        public int ItemCount;
        public int SlotIndex;
        public bool IsOnEquip;

        public StructInventorySlot(int id, int count, int index, bool equip)
        {
            ItemId = id;
            ItemCount = count;
            SlotIndex = index;
            IsOnEquip = equip;
        }

        public override string ToString()
        {
            return $"Item:: Id:{ItemId} Count:{ItemCount} Index:{SlotIndex} IsOnEquip:{IsOnEquip}";
        }

        public static StructInventorySlot GetEmpty()
        {
            return new StructInventorySlot(-1, 0, -1, false);
        }
    }
}