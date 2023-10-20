using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using RPG.Item;
using RPG.Common;

namespace RPG.UI
{
    public class ItemBoxWindow : MonoBehaviour
    {
        public RectTransform SlotsRoot;

        [Header("아이템 박스 슬롯 UI 목록")]
        public List<IconItemSlot> Slots;
        public GameObject SlotUIPrefab;
        private FieldItemBox _linnkedItemBox;
        
        public void Initialize()
        {
            Slots = new();
        }

        public void Open()
        {
            Debug.Log($"{name}.Open");
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
            foreach (var slot in Slots)
            {
                slot.Off();
            }
        }

        public void Link(FieldItemBox itemBox)
        {
            _linnkedItemBox = itemBox;
            UpdateItemAll();
        }

        //public void UdpateAll(FieldItemBox itemBox)
        //{
        //    _linnkedItemBox = itemBox;
        //    UpdateItemAll();
        //}

        /// <summary>
        /// Append inactive Slot UIs
        /// </summary>
        /// <param name="inventoryLength"></param>
        private void AppendSlotUIs(int appendNumber)
        {
            void AddSlotUI()
            {
                GameObject slotUIObject = Instantiate(SlotUIPrefab);
                IconItemSlot slot = slotUIObject.GetComponent<IconItemSlot>();
                slot.SetDragable(false);
                if (slot != null)
                {
                    slot.SlotIndex = Slots.Count;
                    slot.name = $"ItemSlotUI[{Slots.Count}]";
                    slotUIObject.transform.SetParent(SlotsRoot.transform);
                    Slots.Add(slot);
                    slot.Clear();
                }
            }

            for (int i = 0; i < appendNumber; i++)
            {
                AddSlotUI();
            }
        }

        private void UpdateItemAll()
        {
            // 부족한 공간 수
            int addSlotLength = _linnkedItemBox.Items.Count - Slots.Count;
            //Debug.Log($"_linnkedItemBox.Items.Count : {_linnkedItemBox.Items.Count}");
            //Debug.Log($"Slots.Count : {Slots.Count}");
            //Debug.Log($"addSlotLength : {addSlotLength}");

            if(addSlotLength > 0)
            {
                // 공간이 부족하면. 확장
                AppendSlotUIs(addSlotLength);
            }

            for (int slotIndex = 0; slotIndex < Slots.Count; slotIndex++)
            {
                if(slotIndex < _linnkedItemBox.Items.Count)
                {
                    StructIdCount item = _linnkedItemBox.Items[slotIndex];
                    Slots[slotIndex].SetItemInfo(item.Id, item.Count, false);
                    Slots[slotIndex].On();
                }
                else
                {
                    Slots[slotIndex].Off();
                }
            }
        }

        private void Update()
        {
            if (!_linnkedItemBox) return;

            if(_linnkedItemBox.IsChangedInventory)
            {
                UpdateItemAll();
                _linnkedItemBox.IsChangedInventory = false;
            }
        }
    }
}
