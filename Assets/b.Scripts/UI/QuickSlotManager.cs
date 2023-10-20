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
            int key = 1;
            foreach(var quickSlot in QuickSlots)
            {
                quickSlot.SetKey(key % 10);
                key++;
            }
        }

        public void UpdateSlots()
        {
            foreach (var quickSlot in QuickSlots)
            {
                quickSlot.UpdateItmeCount();
            }
        }
    }
}