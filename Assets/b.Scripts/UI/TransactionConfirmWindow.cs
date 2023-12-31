using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RPG.Common;

namespace RPG.UI
{
    public class TransactionConfirmWindow : MonoBehaviour
    {
        #region UIs
        public Image ItemIcon; // CurrentItemData
        public Image GradeBorder; // CurrentItemData

        public TextMeshProUGUI ItemNameText; // CurrentItemData
        public TextMeshProUGUI ItemUnitPriceText;
        public TextMeshProUGUI ItemCountText; // ItemCount
        public TextMeshProUGUI TotalPriceText; // TotalPrice

        public Button BuyButton;
        public Button SellButton;
        #endregion

        [SerializeField] private int _toSellplayerInventorySlotIndx = -1;

        [SerializeField] private StructItemData _transactionItemData;
        public StructItemData TransactionItemData
        {
            get => _transactionItemData;
            private set
            {
                _transactionItemData = value;
                SetItemInfoUI(_transactionItemData);
            }
        }

        [SerializeField] private int _itemUnitPrice;

        [SerializeField] private int _itemCount;
        public int ItemCount
        {
            get => _itemCount;
            private set
            {
                Debug.Log($"{_itemCount} -> {value}");
                _itemCount = value;
                ItemCountText.text = _itemCount.ToString();
                TotalPrice = _itemCount * _itemUnitPrice;
            }
        }

        [SerializeField] private int _totalPrice;

        public int TotalPrice
        {
            get => _totalPrice;
            private set
            {
                _totalPrice = value;
                TotalPriceText.text = value.ToString();
            }
        }

        /// <summary>
        /// true면 상점측, false면 플레이어측
        /// </summary>
        [SerializeField] private TransactionMode _mode;
        public TransactionMode Mode
        {
            get => _mode;
            private set
            {
                _mode = value;
                SellButton.gameObject.SetActive(_mode == TransactionMode.Player);
                BuyButton.gameObject.SetActive(_mode == TransactionMode.ShopKeeper);
            }
        }

        public void Buy()
        {
            if (GameManager.Instance.Player.Money < TotalPrice)
                return;

            if (ItemCount == 0) return;

            if (ItemCount < 0) throw new System.Exception("Buy ItemCount < 0");

            ResultType result = GameManager.Instance.Player.AddItem(TransactionItemData.Id, ItemCount);
            Debug.Log("Buy result: " + result);

            if (result == ResultType.Success)
            {
                // 추가가 완료되면 골드 소비
                GameManager.Instance.Player.AddMoney(-TotalPrice);
                Close();
            }

            return;
        }

        public void Sell()
        {
            ResultType result = GameManager.Instance.Player.SellItem(_toSellplayerInventorySlotIndx, ItemCount);
            //Debug.Log("Sell result: " + result);

            if (result == ResultType.Success)
            {
                Close();
            }

            return;
        }

        public void OpenShopKeeperMode(StructItemData itemData)
        {
            // Mode 먼저 설정
            Mode = TransactionMode.ShopKeeper;
            // 이름, 테두리 색, 아이콘, 개당 가격 설정
            TransactionItemData = itemData;

            // 개수는 0부터
            ItemCount = 0;
            gameObject.SetActive(true);
            ItemIcon.gameObject.SetActive(true);
            ItemUnitPriceText.text = itemData.BuyPrice.ToString();
        }

        public void OpenPlayerMode(int toSellPlayerSlotIndex)
        {
            // Mode 먼저 설정
            Mode = TransactionMode.Player;
            _toSellplayerInventorySlotIndx = toSellPlayerSlotIndex;
            // 이름, 테두리 색, 아이콘, 개당 가격 설정
            StructInventorySlot playerSlot = GameManager.Instance.Player.Items[_toSellplayerInventorySlotIndx];
            StructItemData itemData = DataBase.Items[playerSlot.ItemId];
            TransactionItemData = itemData;
            // 개수는 1부터
            ItemCount = 1;
            gameObject.SetActive(true);
            ItemIcon.gameObject.SetActive(true);
            ItemUnitPriceText.text = itemData.SellPrice.ToString();
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void IncreaseCountX1()
        {
            Increase(1);
        }

        public void IncreaseCountX10()
        {
            Increase(10);
        }

        public void IncreaseCountX100()
        {
            Increase(100);
        }

        private void Increase(int delta)
        {
            if (Mode == TransactionMode.Player)
            {
                if (GameManager.Instance.Player.Items[_toSellplayerInventorySlotIndx].ItemCount < ItemCount + delta)
                {
                    int correctCount = GameManager.Instance.Player.Items[_toSellplayerInventorySlotIndx].ItemCount;
                    Debug.Log($"보유 개수 부족. 판매 개수 제한 {ItemCount + delta} -> {correctCount}");
                    ItemCount = correctCount;
                    return;
                }
            }
            else if (Mode == TransactionMode.ShopKeeper)
            {
                if (GameManager.Instance.Player.Money < _itemUnitPrice * (ItemCount + delta))
                {
                    int correctCount = (int)(GameManager.Instance.Player.Money / (float)_itemUnitPrice);
                    Debug.Log($"보유 골드 부족. 구매 개수 제한 {ItemCount + delta} -> {ItemCount}");
                    ItemCount = correctCount;
                    return;
                }
            }

            Debug.Log($"Increase delta: {delta}, Mode: {Mode}");
            ItemCount += delta;
        }


        public void DecreaseCountX1()
        {
            ItemCount = ItemCount - 1 <= 0 ? 0 : ItemCount - 1;
        }

        public void DecreaseCountX10()
        {
            ItemCount = ItemCount - 10 <= 0 ? 0 : ItemCount - 10;
        }

        public void DecreaseCountX100()
        {
            ItemCount = ItemCount - 100 <= 0 ? 0 : ItemCount - 100;
        }

        private void SetItemInfoUI(StructItemData itemData)
        {
            ItemNameText.text = itemData.Name;
            if (Mode == TransactionMode.Player)
            {
                _itemUnitPrice = itemData.SellPrice;
            }
            else if (Mode == TransactionMode.ShopKeeper)
            {
                _itemUnitPrice = itemData.BuyPrice;
            }
            TotalPrice = _itemCount * _itemUnitPrice;
            ItemIcon.sprite = itemData.Sprite;
            GradeBorder.color = itemData.GradeColor;
        }
    }
}