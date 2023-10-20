using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;
using System.Linq;

namespace RPG.UI
{
    public class QuestSelectionWindow : MonoBehaviour
    {
        public GameObject QuestSlotPrefab;
        public List<NpcQuestSlot> Slots;
        public RectTransform SlotsRoot;
        private readonly int _defaultCount = 10;

        // Awake 를 구현하지 않음. Initialize가 있기 때문에 Slots이나 QuestIds 초기화를 2번 할 수도 있음

        public void Initialize()
        {
            Slots = new();

            for (int i = 0; i < _defaultCount; i++)
            {
                AppendQuestSlot();
            }
        }

        private void AppendQuestSlot()
        {
            //Debug.Log("AppendQuestSlot");
            GameObject questSlotObject = Instantiate(QuestSlotPrefab);
            NpcQuestSlot questSlot = questSlotObject.GetComponent<NpcQuestSlot>();
            questSlotObject.transform.SetParent(SlotsRoot);
            Slots.Add(questSlot);
            questSlotObject.SetActive(false);
        }

        /// <summary>
        /// Slot.ClearOff();
        /// </summary>
        private void ClearQuestSlot()
        {
            foreach(var slot in Slots)
            {
                slot.ClearOff();
            }
        }

        public void UpdateQuest(int[] questIds)
        {
            if (questIds.Length > Slots.Count)
                ExtendSlots(questIds.Length - Slots.Count);

            ClearQuestSlot();

            int slotIndex = 0;
            for (int questIndex = 0; questIndex < questIds.Length; questIndex++)
            {
                StructQuestData questData = DataBase.Quests[questIds[questIndex]];
                bool canStart = true;

                foreach (var questId in questData.RequiredQuestIds)
                {
                    if (!DataBase.Quests[questId].IsClear)
                    {
                        canStart = false;
                        break;
                    }
                }

                if(canStart)
                {
                    Slots[slotIndex].SetInfo(questData.Id, questData.Title);
                    Slots[slotIndex].On();
                    slotIndex++;
                }
            }
        }

        private void ExtendSlots(int number)
        {
            throw new System.NotImplementedException("ExtendSlots is not implemented");
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            for (int slotIndex = 0; slotIndex < Slots.Count; slotIndex++)
            {
                Slots[slotIndex].Off();
            }

            gameObject.SetActive(false);
        }
    }
}