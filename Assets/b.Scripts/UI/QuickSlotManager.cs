using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RPG.Common;

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

        /// <summary>
        /// 동일한 요소가 다른 슬롯에 등록되어 있다면 기존에 등록된 요소를 없앰
        /// </summary>
        /// <param name="type"></param>
        /// <param name="link"></param>
        public void TryClearSlotWidthSameElement(EnumQuickSlotType type, int link)
        {
            foreach(var quickSlot in QuickSlots)
            {
                if (quickSlot.Type == type && quickSlot.Link == link) quickSlot.ClearQuickSlot();
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

        public void UpdateSlotItemLink(int oldLink, int newLink)
        {
            foreach(var slot in QuickSlots)
            {
                if (slot.Type == EnumQuickSlotType.Item && slot.Link == oldLink)
                {
                    slot.AddElement(EnumQuickSlotType.Item, newLink);
                }
            }
        }
    }
}