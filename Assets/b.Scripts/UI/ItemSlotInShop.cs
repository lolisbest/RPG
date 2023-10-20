using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RPG.Common;

namespace RPG.UI
{
    public class ItemSlotInShop : MonoBehaviour
    {
        public TextMeshProUGUI ItemNameText;

        public Image GradeBorder;
        public Image ItemIcon;

        public TextMeshProUGUI UnitPriceTextText;

        private readonly string _baseSellPriceString = "판매 가격";
        private readonly string _baseBuyPriceString = "구매 가격";

        public GameObject HoldingItemCountPanel;
        public TextMeshProUGUI HoldingItemCountText;

        private readonly string _baseHoldingItemCountString = "보유 수 : {0}";

        public TextMeshProUGUI ItemPriceNumberText;

        [SerializeField] private int _linkedInventorySlotIndex;
        public int ItemId { get; private set; }


        public TransactionMode Mode { get; private set; }

        [SerializeField] private StructItemData _currentItemData;

        [SerializeField] private Button _button;

        private void SetItemInfo(int itemId)
        {
            StructItemData itemData = DataBase.Items[itemId];
            _currentItemData = itemData;

            ItemId = _currentItemData.Id;
            ItemNameText.text = _currentItemData.Name;
            GradeBorder.color = _currentItemData.GradeColor;
            ItemIcon.sprite = _currentItemData.Sprite;
            if(Mode == TransactionMode.Player)
            {
                ItemPriceNumberText.text = _currentItemData.SellPrice.ToString();
            }
            else if(Mode == TransactionMode.ShopKeeper)
            {
                ItemPriceNumberText.text = _currentItemData.BuyPrice.ToString();
            }
        }

        private void ClearSlot()
        {
            ItemNameText.text = string.Empty;
            GradeBorder.color = Color.white;
            ItemIcon.sprite = null;
            ItemPriceNumberText.text = string.Empty;
            HoldingItemCountPanel.SetActive(false);
            ItemPriceNumberText.text = string.Empty;
        }

        public void Off()
        {
            ClearSlot();
            gameObject.SetActive(false);
        }

        public void On(int itemId)
        {
            SetItemInfo(itemId);
            gameObject.SetActive(true);
        }

        public void On(StructInventorySlot inventorySlot)
        {
            SetItemInfo(inventorySlot.ItemId);
            if (Mode == TransactionMode.Player)
            {
                HoldingItemCountPanel.SetActive(true);
                HoldingItemCountText.text = string.Format(_baseHoldingItemCountString, inventorySlot.ItemCount.ToString());
            }
            _linkedInventorySlotIndex = inventorySlot.SlotIndex;
            gameObject.SetActive(true);
        }

        public void OpenTransactionWindow()
        {
            //Debug.Log($"OpenTransactionWindow Mode : {Mode}");
            if(Mode == TransactionMode.ShopKeeper)
            {
                InGameUIManager.Instance.OpenItemShopKeeperModeTransactionWindow(_currentItemData);
            }
            else if (Mode == TransactionMode.Player)
            {
                InGameUIManager.Instance.OpenItemPlayerModeTransactionWindow(_linkedInventorySlotIndex);
            }
        }

        public void SetMode(TransactionMode mode)
        {
            Mode = mode;
            if(Mode == TransactionMode.Player)
            {
                UnitPriceTextText.text = _baseSellPriceString;
            }
            else if(Mode == TransactionMode.ShopKeeper)
            {
                UnitPriceTextText.text = _baseBuyPriceString;
            }
        }

        //public void AddListenerOnClicked(UnityEngine.Events.UnityAction call)
        //{
        //    _button.onClick.AddListener(call);
        //}
    }
}