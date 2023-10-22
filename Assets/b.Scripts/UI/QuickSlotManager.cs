using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RPG.UI
{
    public class QuickSlotManager : MonoBehaviour
    {
        public List<QuickSlot> QuickSlots;

        void Awake()
        {
            QuickSlots = GetComponentsInChildren<QuickSlot>().ToList();
            int count = 1;
            foreach(var quickSlot in QuickSlots)
            {
                int key = count % 10;
                quickSlot.SetKey(key % 10);
                quickSlot.SetQuitckSlotManager(this);
                count++;
            }
        }

        public void UpdateSlots()
        {
            foreach (var quickSlot in QuickSlots)
            {
                quickSlot.UpdateItmeCount();
            }
        }

        public void TryClearSlotWidthSameSkill(int skillId)
        {
            foreach(var quickSlot in QuickSlots)
            {
                if (quickSlot.LinkedSkillId == skillId) quickSlot.ClearQuickSlot();
            }
        }

        public void TryClearSlotWidthSameItem(int inventorySlotIndex)
        {
            foreach (var quickSlot in QuickSlots)
            {
                if (quickSlot.LinkedInventorySlotIndex == inventorySlotIndex) quickSlot.ClearQuickSlot();
            }
        }
    }
}