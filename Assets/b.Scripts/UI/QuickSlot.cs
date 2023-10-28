using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using RPG.Common;
using TMPro;

namespace RPG.UI
{

    public class QuickSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Image Icon; // 퀵슬롯 버튼에 표시할 아이템 아이콘 이미지

        public int Key { get; private set; }
        public TextMeshProUGUI KeyText;

        public int LinkedInventorySlotIndex;
        public int LinkedSkillId;

        public TextMeshProUGUI ItemCountText;

        private RectTransform _iconRectTransform;

        [SerializeField] private Image _coolTimeIndicator;

        [SerializeField] private float _leftCoolTime;
        public float LeftCoolTime
        {
            get => _leftCoolTime;
            private set
            {
                _coolTimeIndicator.gameObject.SetActive(false);
                _leftCoolTime = value;

                if (LinkedSkillId <= 0) return;

                if (_leftCoolTime <= 0) return;

                StructSkillData skillData = DataBase.Skills[LinkedSkillId];
                _coolTimeIndicator.fillAmount = Mathf.Clamp01(_leftCoolTime / skillData.CoolTime);
                _coolTimeIndicator.gameObject.SetActive(true);
            }
        }

        [SerializeField] private QuickSlotManager _quickSlotManager;

        public void SetQuitckSlotManager(QuickSlotManager quickSlotManager)
        {
            _quickSlotManager = quickSlotManager;
        }

        private void AddItemToQuickSlot(Sprite itemSprite, int linkedInventoryslotIndex)
        {
            _quickSlotManager.TryClearSlotWidthSameItem(linkedInventoryslotIndex);

            if (!IsEmpty())
            {
                ClearQuickSlot();
            }


            LinkedInventorySlotIndex = linkedInventoryslotIndex;
            Icon.sprite = itemSprite;

            Debug.Log("Quick LinkedInventorySlotIndex : " + linkedInventoryslotIndex);
            Debug.Log("Quick itemSprite : " + itemSprite);

            UpdateItmeCount();

            if (itemSprite != null)
                Icon.enabled = true;
        }

        private void AddSkillToQuickSlot(Sprite skillSprite, int skillId)
        {
            _quickSlotManager.TryClearSlotWidthSameSkill(skillId);

            if (!IsEmpty())
            {
                ClearQuickSlot();
            }

            //throw new System.NotImplementedException("Add Skill To QuickSlot");

            LinkedSkillId = skillId;
            Icon.sprite = skillSprite;

            if (skillSprite != null)
                Icon.enabled = true;
        }

        /// <summary>
        /// Icon disable, LinkedInventorySlotIndex = -1, LinkedSkillId = -1
        /// </summary>
        public void ClearQuickSlot()
        {
            Icon.enabled = false;
            Icon.raycastTarget = false;
            LinkedInventorySlotIndex = -1;
            LinkedSkillId = -1;
            ItemCountText.text = null;
        }

        public bool IsEmpty()
        {
            return LinkedSkillId == -1 && LinkedSkillId == -1 && Icon.sprite == null;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null)
                return;

            // Image Icon Small
            IconItemSlot itemSlot = eventData.pointerDrag.GetComponent<IconItemSlot>();
            if (itemSlot != null)
            {
                itemSlot.SetFloatingIconSize(_iconRectTransform.rect.size);
                return;
            }
            else
            {
                SkillSlot skillSlot = eventData.pointerDrag.GetComponent<SkillSlot>();
                if (skillSlot != null)
                {
                    skillSlot.SetFloatingIconSize(_iconRectTransform.rect.size);
                    return;
                }
            }
        }

        public void AddElement(GameObject elmentObject)
        {
            Debug.Log("AddElement");

            IconItemSlot itemSlot = elmentObject.GetComponent<IconItemSlot>();
            if (itemSlot != null)
            {
                if (itemSlot.ItemType != EnumItemType.Consumable)
                    return;

                Debug.Log("Register Item");
                AddItemToQuickSlot(itemSlot.ItemIcon.sprite, itemSlot.SlotIndex);
                LinkedSkillId = -1;
                return;
            }
            else
            {
                SkillSlot skillSlot = elmentObject.GetComponent<SkillSlot>();
                if (skillSlot != null)
                {
                    Debug.Log("Register Skill");
                    AddSkillToQuickSlot(skillSlot.SkillIcon.sprite, skillSlot.SkillId);
                    LinkedInventorySlotIndex = -1;
                    return;
                }
            }
        }

        public void SetKey(int key)
        {
            Key = key;
            KeyText.text = key.ToString();
        }

        void Awake()
        {
            ClearQuickSlot();
            _iconRectTransform = Icon.rectTransform;
        }

        void Update()
        {
            if (LinkedSkillId > 0 && LeftCoolTime > 0)
            {
                LeftCoolTime -= Time.deltaTime;
            }
        }

        public void Use()
        {
            if(LinkedInventorySlotIndex != -1)
            {
                int leftItemCount = Player.Instance.ConsumeItem(LinkedInventorySlotIndex);

                if (leftItemCount == 0)
                {
                    ClearQuickSlot();
                }
                else if (leftItemCount > 0)
                {
                    // 개수 표시 업데이트
                    UpdateItmeCount();
                }
            }
            else if(LinkedSkillId != -1)
            {
                Debug.Log($"Act Skill #{LinkedSkillId}, _coolTimeIndicator: {_coolTimeIndicator.gameObject.activeSelf}");
                if (_coolTimeIndicator.gameObject.activeSelf) return;

                if (Player.Instance.LoadSkill(LinkedSkillId) == ResultType.SkillSuccess)
                {
                    Debug.Log("Act Skill Success");
                    StructSkillData skillData = DataBase.Skills[LinkedSkillId];
                    LeftCoolTime = skillData.CoolTime;
                }
            }
        }

        /// <summary>
        /// LinkedInventorySlotIndex를 이용하여 ItemCountText를 업데이트
        /// </summary>
        public void UpdateItmeCount()
        {
            if (LinkedInventorySlotIndex == -1)
            {
                ItemCountText.text = "";
            }
            else
            {
                StructInventorySlot slot = Player.Instance.Items[LinkedInventorySlotIndex];
                if (slot.ItemId == -1 || slot.ItemCount == 0)
                {
                    ClearQuickSlot();
                }
                else
                {
                    int itemLeftNumber = slot.ItemCount;
                    ItemCountText.text = itemLeftNumber.ToString();
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null)
                return;

            IconItemSlot itemSlot = eventData.pointerDrag.GetComponent<IconItemSlot>();
            if (itemSlot != null)
            {
                itemSlot.ResetFloatingIconSize();
                return;
            }
            else
            {
                SkillSlot skillSlot = eventData.pointerDrag.GetComponent<SkillSlot>();
                if (skillSlot != null)
                {
                    skillSlot.ResetFloatingIconSize();
                    return;
                }
            }
        }
    }
}