using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using RPG.Common;

namespace RPG.UI
{
    public partial class IconItemSlot : IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private RectTransform _slotRectTransform;

        public bool CanDrag { get; private set; }

        public EnumItemType ItemType
        {
            get
            {
                if (_currentItemId == -1)
                    return EnumItemType.None;

                return DataBase.Items[_currentItemId].ItemType;
            }
        }

        [SerializeField]
        private Image _tempCopyImage;

        public void OnBeginDrag(PointerEventData eventData)
        {
            // Drag Start
            if (_currentItemId == -1) return;

            if (!CanDrag) return;

            GameObject obj = new();
            _tempCopyImage = obj.AddComponent<Image>();
            _tempCopyImage.transform.position = eventData.position;
            _tempCopyImage.sprite = ItemIcon.sprite;
            _tempCopyImage.raycastTarget = false;

            ResetFloatingIconSize();

            _tempCopyImage.transform.SetParent(InGameUIManager.Instance.transform);
        }

        public void SetDragable(bool can)
        {
            CanDrag = can;
        }

        public void OnDrag(PointerEventData eventData)
        {
            //Debug.Log("OnDrag");
            if (_tempCopyImage == null)
                return;

            _tempCopyImage.transform.position = eventData.position;
            _tempCopyImage.sprite = ItemIcon.sprite;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_tempCopyImage == null)
                return;

            RaycastResult result = eventData.pointerCurrentRaycast;
            //Debug.Log("result : " + result);
            if(result.isValid)
            {
                QuickSlot quickSlot = result.gameObject.GetComponent<QuickSlot>();
                if (quickSlot != null)
                {
                    quickSlot.AddElement(this.gameObject);
                }
                else
                {
                    IconItemSlot otherItemSlot = result.gameObject.GetComponent<IconItemSlot>();
                    if(otherItemSlot != null)
                    {
                        InGameUIManager.Instance.CLoseInventoryItemInfoWindow();
                        Swap(otherItemSlot);
                    }
                }
            }

            Destroy(_tempCopyImage.gameObject);
            _tempCopyImage = null;
            //Debug.Log("_tempCopyImage:" + _tempCopyImage);
        }

        public void SetFloatingIconSize(Vector2 size)
        {
            _tempCopyImage.rectTransform.sizeDelta = size;
        }

        public void ResetFloatingIconSize()
        {
            Vector2 slotSize = _slotRectTransform.rect.size;
            _tempCopyImage.rectTransform.sizeDelta = slotSize * 1.5f;
        }

        public void Swap(IconItemSlot otherSlot)
        {
            //int otherSlotItemId = otherSlot.ItemId;
            //int otherSlotItemCount = otherSlot.ItemCount;
            //bool otherSlotIsEquipped = otherSlot.IsEquipped;

            //otherSlot.SetItemInfo(ItemId, ItemCount, IsEquipped);
            //SetItemInfo(otherSlotItemId, otherSlotItemCount, otherSlotIsEquipped);

            Player.Instance.SwapItemSlot(SlotIndex, otherSlot.SlotIndex);
        }
    } }