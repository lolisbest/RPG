using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RPG.Item;
using RPG.Common;

namespace RPG.UI
{
    public class InventoryItemInfoWindow : MonoBehaviour
    {
        public TextMeshProUGUI ItemNameText;
        public Image ItemIcon;
        public Image ItemGrade;

        public GameObject StatusOptionsRoot;

        public TextMeshProUGUI AtkOptionText;
        public TextMeshProUGUI DefOptionText;
        public TextMeshProUGUI MaxHpOptionText;
        public TextMeshProUGUI MaxMpOptionText;
        public TextMeshProUGUI ItemDescriptionText;

        public GameObject EquipButton;
        public GameObject UnequipButton;

        public GameObject ConsumeButton;

        private readonly string AtkOptionBaseText = "Attack + {0}";
        private readonly string DefOptionBaseText = "Def + {0}";
        private readonly string MaxHpOptionBaseText = "MaxHp + {0}";
        private readonly string MaxMpOptionBaseText = "MaxMp + {0}";

        [SerializeField] private UIManager _uiManager;

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            _uiManager.SelectInventorySlot(-1);
            gameObject.SetActive(false);
        }

        public void SetItemInfo(int itemId, bool isEquipped)
        {
            StructItemData itemData = DataBase.Items[itemId];
            SetItemName(itemData.Name);
            SetItemIcon(itemData.Sprite);
            SetItemGrade(itemData.GradeColor);
            SetItemDescription(itemData.Description);
            SetStatusOption(itemData.Attack, itemData.Defence, itemData.MaxHp, itemData.MaxMp);

            if (itemData.ItemType == EnumItemType.Equipment)
            {
                ToggleStatusOptions(true);

                ToggleEquipButton(!isEquipped);
                ToggleUnequipButton(isEquipped);

                ToggleConsumeButton(false);

            }
            else if(itemData.ItemType == EnumItemType.Consumable)
            {
                ToggleStatusOptions(false);

                ToggleEquipButton(false);
                ToggleUnequipButton(false);

                ToggleConsumeButton(true);
            }
            else
            {
                ToggleStatusOptions(false);

                ToggleEquipButton(false);
                ToggleUnequipButton(false);

                ToggleConsumeButton(false);
            }    
        }

        public void Equip()
        {
            Debug.Log($"Equip : {ItemNameText.text}");
            GameManager.Instance.Player.EquipItem(_uiManager.CurrentSelectedSlotIndex);
        }

        public void Unequip()
        {
            Debug.Log($"{name} Unequip CurrentSelectedSlotIndex : {_uiManager.CurrentSelectedSlotIndex}");
            Debug.Log($"{name} Unequip : {ItemNameText.text}");
            GameManager.Instance.Player.UnequipItem(_uiManager.CurrentSelectedSlotIndex);
        }

        public void UseItem()
        {
            Debug.Log($"Use : {ItemNameText.text}");
            GameManager.Instance.Player.ConsumeItem(_uiManager.CurrentSelectedSlotIndex);
        }

        private void SetStatusOption(int attack, int def, int maxHp, int maxMp)
        {
            AtkOptionText.text = string.Format(AtkOptionBaseText, attack);
            DefOptionText.text = string.Format(DefOptionBaseText, def);
            MaxHpOptionText.text = string.Format(MaxHpOptionBaseText, maxHp);
            MaxMpOptionText.text = string.Format(MaxMpOptionBaseText, maxMp);
        }

        private void SetItemIcon(Sprite sprite)
        {
            ItemIcon.sprite = sprite;
        }

        private void SetItemName(string itemName)
        {
            ItemNameText.text = itemName;
        }

        private void SetItemGrade(Color color)
        {
            ItemGrade.color = color;
        }

        private void SetItemDescription(string description)
        {
            ItemDescriptionText.text = description;
        }

        private void ToggleStatusOptions(bool open)
        {
            StatusOptionsRoot.SetActive(open);
        }

        private void ToggleEquipButton(bool open)
        {
            EquipButton.SetActive(open);
        }


        private void ToggleUnequipButton(bool open)
        {
            UnequipButton.SetActive(open);
        }

        private void ToggleConsumeButton(bool open)
        {
            ConsumeButton.SetActive(open);
        }
    }
}