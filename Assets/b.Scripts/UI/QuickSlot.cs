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

        public EnumQuickSlotType Type { get; private set; }

        //public int LinkedInventorySlotIndex;
        //public int LinkedSkillId;

        /// <summary>
        /// Skill이라면 Skill Id, 아이템이라면 Inventory slot Index
        /// </summary>
        private int _link;
        public int Link
        {
            get => _link;
            private set
            {
                _link = value;
            }
        }

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

                // _link가 스킬이 아니라면
                if (Link == -1 && Type != EnumQuickSlotType.Skill) return;

                if (_leftCoolTime <= 0) return;

                StructSkillData skillData = DataBase.Skills[Link];
                _coolTimeIndicator.fillAmount = Mathf.Clamp01(_leftCoolTime / skillData.CoolTime);
                _coolTimeIndicator.gameObject.SetActive(true);
            }
        }

        [SerializeField] private QuickSlotManager _quickSlotManager;

        public void SetQuitckSlotManager(QuickSlotManager quickSlotManager)
        {
            _quickSlotManager = quickSlotManager;
        }

        private void RegisterItem(Sprite itemSprite, int linkedInventoryslotIndex)
        {
            _quickSlotManager.TryClearSlotWidthSameItem(linkedInventoryslotIndex);

            if (!IsEmpty())
            {
                ClearQuickSlot();
            }


            Link = linkedInventoryslotIndex;
            Type = EnumQuickSlotType.Item;
            Icon.sprite = itemSprite;

            Debug.Log("Quick LinkedInventorySlotIndex : " + linkedInventoryslotIndex);
            Debug.Log("Quick itemSprite : " + itemSprite);

            UpdateItemCount();

            if (itemSprite != null)
                Icon.enabled = true;
        }

        private void RegisterSkill(Sprite skillSprite, int skillId)
        {
            _quickSlotManager.TryClearSlotWidthSameSkill(skillId);

            if (!IsEmpty())
            {
                ClearQuickSlot();
            }

            //throw new System.NotImplementedException("Add Skill To QuickSlot");

            Link = skillId;
            Icon.sprite = skillSprite;
            Type = EnumQuickSlotType.Skill;

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
            Link = -1;
            Type = EnumQuickSlotType.None;
            ItemCountText.text = null;
        }

        public bool IsEmpty()
        {
            if (
                (Link != -1 && Type == EnumQuickSlotType.None) ||
                (Link == -1 && Type != EnumQuickSlotType.None)
                )
            {
                throw new System.Exception("Link and Type is Wrong");
            }

            return Link == -1 && Type == EnumQuickSlotType.None;
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
            //Debug.Log("AddElement");

            IconItemSlot itemSlot = elmentObject.GetComponent<IconItemSlot>();
            if (itemSlot != null)
            {
                // 소모성 아이템이 아니라면 추가 되지 않음
                if (itemSlot.ItemType != EnumItemType.Consumable)
                    return;

                Debug.Log("Register Item");
                RegisterItem(itemSlot.ItemIcon.sprite, itemSlot.SlotIndex);
                return;
            }
            else
            {
                SkillSlot skillSlot = elmentObject.GetComponent<SkillSlot>();
                if (skillSlot != null)
                {
                    Debug.Log("Register Skill");
                    RegisterSkill(skillSlot.SkillIcon.sprite, skillSlot.SkillId);
                    return;
                }
            }
        }

        public void AddElement(EnumQuickSlotType type, int link)
        {
            //Debug.Log("AddElement");
            if (type == EnumQuickSlotType.Item)
            {
                StructInventorySlot inventorySlot = GameManager.Instance.Player.Items[link];
                StructItemData itemData = DataBase.Items[inventorySlot.ItemId];

                Debug.Log("Register Item");
                RegisterItem(itemData.Sprite, link);
                return;
            }
            else if (type == EnumQuickSlotType.Skill)
            {
                StructSkillData skillData = DataBase.Skills[link];
                Debug.Log("Register Skill");
                RegisterSkill(skillData.Icon, link);
                return;
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

        void FixedUpdate()
        {
            if (Type == EnumQuickSlotType.Skill && Link > 0 && LeftCoolTime > 0)
            {
                LeftCoolTime -= Time.deltaTime;
            }
        }

        public void Use()
        {
            if (Type == EnumQuickSlotType.Item)
            {
                if (Link != -1)
                {
                    int leftItemCount = GameManager.Instance.Player.ConsumeItem(Link);

                    if (leftItemCount == 0)
                    {
                        ClearQuickSlot();
                    }
                    else if (leftItemCount > 0)
                    {
                        // 개수 표시 업데이트
                        UpdateItemCount();
                    }
                }
            }
            else if (Type == EnumQuickSlotType.Skill)
            {
                if (Link != -1)
                {
                    Debug.Log($"Act Skill #{Link}, _coolTimeIndicator: {_coolTimeIndicator.gameObject.activeSelf}");
                    if (_coolTimeIndicator.gameObject.activeSelf) return;
                    ResultType loadResult = GameManager.Instance.Player.LoadSkill(Link);
                    if (loadResult == ResultType.SkillSuccess)
                    {
                        Debug.Log("Act Skill Success");
                        StructSkillData skillData = DataBase.Skills[Link];
                        LeftCoolTime = skillData.CoolTime;
                    }
                    else
                    {
                        Debug.Log("LoadSkill Fail " + loadResult);
                    }
                }
            }
        }

        /// <summary>
        /// LinkedInventorySlotIndex를 이용하여 ItemCountText를 업데이트
        /// </summary>
        public void UpdateItemCount()
        {
            if (Type == EnumQuickSlotType.Item)
            {
                StructInventorySlot slot = GameManager.Instance.Player.Items[Link];
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
            else if (Type == EnumQuickSlotType.Skill)
            {
                ItemCountText.text = "";
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