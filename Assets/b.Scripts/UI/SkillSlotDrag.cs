using RPG.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace RPG.UI
{
    public partial class SkillSlot : Slot<StructSkillData>, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public bool CanDrag { get; private set; } = true;
        [SerializeField]
        private Image _tempCopyImage;

        [SerializeField] private RectTransform _slotRectTransform;

        public void SetDragable(bool can)
        {
            CanDrag = can;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("SkillSlot Drag Start");
            // Drag Start
            if (SkillId == -1) return;

            if (!CanDrag) return;

            GameObject obj = new();
            _tempCopyImage = obj.AddComponent<Image>();
            _tempCopyImage.transform.position = eventData.position;
            _tempCopyImage.sprite = SkillIcon.sprite;
            _tempCopyImage.raycastTarget = false;

            ResetFloatingIconSize();

            _tempCopyImage.transform.SetParent(InGameUIManager.Instance.transform);
        }

        public void OnDrag(PointerEventData eventData)
        {
            //Debug.Log("OnDrag");
            if (_tempCopyImage == null)
                return;

            _tempCopyImage.transform.position = eventData.position;
            _tempCopyImage.sprite = SkillIcon.sprite;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_tempCopyImage == null)
                return;

            RaycastResult result = eventData.pointerCurrentRaycast;
            //Debug.Log("result : " + result);
            if (result.isValid)
            {
                QuickSlot quickSlot = result.gameObject.GetComponent<QuickSlot>();
                if (quickSlot != null)
                {
                    quickSlot.AddElement(this.gameObject);
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

    }
}
