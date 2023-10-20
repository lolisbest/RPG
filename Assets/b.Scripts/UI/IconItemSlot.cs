using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Item;
using TMPro;
using RPG.Common;

namespace RPG.UI
{
    public partial class IconItemSlot : MonoBehaviour
    {
        public int SlotIndex { get; set; }
        public Button @Button;
        private int _currentItemId;

        public GameObject EquipMark;

        public Image GradeBorder;

        public bool IsEquipped { get; private set; }
     
        public int ItemId
        {
            get => _currentItemId;
            private set
            {
                _currentItemId = value;

                if (_currentItemId != -1)
                {
                    StructItemData itemData = DataBase.Items[_currentItemId];
                    GradeBorder.color = itemData.GradeColor;

                    ItemIcon.sprite = DataBase.Items[value].Sprite;
                    ItemIcon.gameObject.SetActive(true);
                    return;
                }

                GradeBorder.color = Color.white;
                ItemIcon.sprite = null;
                ItemIcon.gameObject.SetActive(false);
                return;
            }
        }

        public Image ItemIcon;

        private int _count;
        public int ItemCount
        {
            get => _count;
            private set
            {
                _count = value;

                ItemCountText.text = value.ToString();
                ItemCountText.transform.parent.gameObject.SetActive(value > 1);
                return;
            }
        }

        public TextMeshProUGUI ItemCountText;

        // On - Empty
        // On - Filled
        // Off

        public void SetItemInfo(int itemId, int itemCount, bool isEquipped)
        {
            ItemId = itemId;
            ItemCount = itemCount;
            IsEquipped = isEquipped;
            ToggleEquipMark(IsEquipped);
        }

        /// <summary>
        /// SetItemInfo(-1, 0); Icon, CountText are disable
        /// </summary>
        public void Clear()
        {
            SetItemInfo(-1, 0, false);
        }

        public void On()
        {
            gameObject.SetActive(true);
        }


        /// <summary>
        /// gameObject.SetActive(false);
        /// Clear();
        /// </summary>
        public void Off()
        {
            gameObject.SetActive(false);
            Clear();
        }

        public void OnSelected()
        {
            InGameUIManager.Instance.SelectInventorySlot(SlotIndex);
        }

        public void ToggleEquipMark(bool on)
        {
            EquipMark.SetActive(on);
        }

        void Awake()
        {
            // _rectTransform 를 드래그에서만 사용한다면, 드래그를 한다는 건 그 이전에 Awake() 됐다는 뜻.
            _slotRectTransform = GetComponent<RectTransform>();
        }
    }
}