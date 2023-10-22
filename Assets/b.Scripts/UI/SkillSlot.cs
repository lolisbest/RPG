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
        [SerializeField] private TextMeshProUGUI _skillNameText;
        [SerializeField] private TextMeshProUGUI _skillMpCostText;
        [SerializeField] private Image _skillIcon;

        public Image SkillIcon { get => _skillIcon; }
        public int SkillId { get => SlotData.Id; }

        public override void SetSlotData(StructSkillData skillData)
        {
            base.SetSlotData(skillData);
            _skillNameText.text = skillData.Name;
            _skillMpCostText.text = skillData.MpCost.ToString();
            _skillIcon.sprite = skillData.Icon;
        }
    }
}
