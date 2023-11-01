using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Common;
using TMPro;
using System;

namespace RPG.UI
{
    public class InventoryWindow : MonoBehaviour
    {
        [SerializeField] private UIManager _uiManager;
        public RectTransform SlotsRoot;
        [Header("인밴토리 슬롯 UI 목록")]
        public List<IconItemSlot> Slots;
        public GameObject SlotUIPrefab;
        public TextMeshProUGUI MoneyText;
        public Player @Player;

        public EquipSlot HeadSlot;
        public EquipSlot ChestSlot;
        public EquipSlot HandSlot0;
        public EquipSlot HandSlot1;
        public EquipSlot FootSlot;

        public void Initialize()
        {
            Slots = new();
            ClearEquipSlots();
        }

        public void Open()
        {
            gameObject.SetActive(true);
            UpdateItemAll();
        }

        public void SetPlayerInstance(Player player)
        {
            @Player = player;
        }
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
                slot.SetDragable(true);
                if (slot != null)
                {
                    slot.SlotIndex = Slots.Count;
                    slot.name = $"ItemSlotUI[{Slots.Count}]";
                    slotUIObject.transform.SetParent(SlotsRoot.transform);
                    Slots.Add(slot);
                    slot.Button.onClick.AddListener(slot.OnSelected);
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
            ClearEquipSlots();

            StructInventorySlot[] playerItemSlots = @Player.Items;
            // 부족한 공간 수
            int addSlotLength = playerItemSlots.Length - Slots.Count;
            if(addSlotLength > 0)
            {
                // 공간이 부족하면. 확장
                AppendSlotUIs(addSlotLength);
            }

            for (int slotIndex = 0; slotIndex < Slots.Count; slotIndex++)
            {
                StructInventorySlot playerSlot = @Player.Items[slotIndex];

                //Debug.Log($"UpdateItemAll : {playerSlot} playerSlot.ItemCount : {playerSlot.ItemCount}");
                Slots[slotIndex].SetItemInfo(playerSlot.ItemId, playerSlot.ItemCount, playerSlot.IsOnEquip);

                if(playerSlot.IsOnEquip)
                {
                    // 착용 중이라면 장비 슬롯에 적용
                    UpdateEquipSlots(playerSlot.ItemId);
                }

                //if (playerSlot.ItemId != -1)
                //{
                //    // 아이템이 있는 슬롯
                //    //Debug.Log($"Existing Item Slot : Slots:{Slots.Count}, slotIndex:{slotIndex}");
                //    //Slots[slotIndex].SetItemInfo(playerSlot.ItemId, playerSlot.ItemCount);
                //    Slots[slotIndex].On();
                //}
                //else
                //{
                //    // 아이템이 없는 슬롯
                //    //Debug.Log($"Empty : Slots:{Slots.Count}, slotIndex:{slotIndex}");
                //    Slots[slotIndex].Off();
                //}

            }

            SetMoney(@Player.Money);
            _uiManager.UpdateInventoryItemInfoWindow();
        }

        void Update()
        {
            if (!@Player) return;

            if(@Player.IsChangedInventory)
            {
                UpdateItemAll();
                _uiManager.UpdateQuickSlots();
                @Player.IsChangedInventory = false;
                
            }
        }

        private void UpdateEquipSlots(int itemId)
        {
            StructItemData itemData = DataBase.Items[itemId];
            switch (itemData.EquipType)
            {
                case EnumEquipType.Head:
                    HeadSlot.Equip(itemData.Id);
                    break;
                case EnumEquipType.Chest:
                    ChestSlot.Equip(itemData.Id);
                    break;
                case EnumEquipType.Hand0:
                    HandSlot0.Equip(itemData.Id);
                    break;
                case EnumEquipType.Hand1:
                    HandSlot1.Equip(itemData.Id);
                    break;
                case EnumEquipType.Foot:
                    FootSlot.Equip(itemData.Id);
                    break;
                default:
                    throw new Exception($"UpdateEquipSlots :: Unknown EquipType. item Name : {itemData.Name}, EquipType : {itemData.EquipType}");
            }
        }

        private void ClearEquipSlots()
        {
            HeadSlot.Unequip();
            ChestSlot.Unequip();
            HandSlot0.Unequip();
            HandSlot1.Unequip();
            FootSlot.Unequip();
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        private void SetMoney(int newMoney)
        {
            MoneyText.text = newMoney.ToString();
        }

    }
}
