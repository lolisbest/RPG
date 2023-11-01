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
                quickSlot.UpdateItemCount();
            }
        }

        public void TryClearSlotWidthSameSkill(int skillId)
        {
            foreach(var quickSlot in QuickSlots)
            {
                if (quickSlot.Link == skillId) quickSlot.ClearQuickSlot();
            }
        }

        public void TryClearSlotWidthSameItem(int inventorySlotIndex)
        {
            foreach (var quickSlot in QuickSlots)
            {
                if (quickSlot.Link == inventorySlotIndex) quickSlot.ClearQuickSlot();
            }
        }

        public (string[], int[]) GetQuickSlotLinkes()
        {
            int[] linkes = new int[QuickSlots.Count];
            string[] slotTypes = new string[QuickSlots.Count];

            for (int i = 0; i < QuickSlots.Count; i++)
            {
                slotTypes[i] = QuickSlots[i].Type.ToString();
                linkes[i] = QuickSlots[i].Link;
            }

            return (slotTypes, linkes);
        }

        public void SetQuickSlots(string[] types, int[] linkes)
        {
            for (int i = 0; i < QuickSlots.Count; i++)
            {
                QuickSlots[i].AddElement(Utils.StringToEnum<RPG.Common.EnumQuickSlotType>(types[i]), linkes[i]);
            }
        }
    }
}