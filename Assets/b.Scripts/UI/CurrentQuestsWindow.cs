using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;

namespace RPG.UI
{
    public class CurrentQuestsWindow : MonoBehaviour
    {
        public GameObject QuestSlotPrefab;
        public List<CurrentQuestSlot> Slots;
        public RectTransform SlotsRoot;
        private int _currentQuestsNumber;
        private readonly int _defaultCount = 10;


        public void Initialize()
        {
            Slots = new();
            _currentQuestsNumber = 0;

            for (int i = 0; i < SlotsRoot.transform.childCount; i++)
            {
                Destroy(SlotsRoot.transform.GetChild(i));
            }

            for (int i = 0; i < _defaultCount; i++)
            {
                AppendQuestSlot();
            }
        }

        private CurrentQuestSlot AppendQuestSlot()
        {
            //Debug.Log("AppendCurrentQuestSlot");
            GameObject questSlotObject = Instantiate(QuestSlotPrefab);
            CurrentQuestSlot questSlot = questSlotObject.GetComponent<CurrentQuestSlot>();
            questSlotObject.transform.SetParent(SlotsRoot);
            Slots.Add(questSlot);
            questSlotObject.SetActive(false);
            return questSlot;
        }

        public bool DeleteQuest(int questId)
        {
            for (int slotIndex = 0; slotIndex < Slots.Count; slotIndex++)
            {
                //Debug.Log($"DeleteQuest: {Slots[slotIndex].QuestId}:{Slots[slotIndex].Text.text}");
                if(Slots[slotIndex].QuestId == questId)
                {
                    Slots[slotIndex].Clear();
                    return true;
                }
            }

            return false;
        }

        public void AddQuest(int questId)
        {
            foreach(var slot in Slots)
            {
                if(slot.QuestId == questId)
                {
                    throw new System.Exception($"Already Existing QuestId:{questId} in CurrentQuestsInProgress");
                }
            }

            bool add = false;
            StructQuestData newQuestData = DataBase.Quests[questId];

            for (int slotIndex = 0; slotIndex < Slots.Count; slotIndex++)
            {
                Debug.Log($"CurrQuestSlot[{slotIndex}] : {Slots[slotIndex].gameObject.activeSelf}");
                if(Slots[slotIndex].gameObject.activeSelf == false)
                {
                    Slots[slotIndex].SetInfo(newQuestData.Id, newQuestData.Title);
                    Slots[slotIndex].On();
                    add = true;
                    _currentQuestsNumber += 1;
                    break;
                }
            }

            if(!add)
            {
                CurrentQuestSlot newSlot = AppendQuestSlot();
                newSlot.SetInfo(newQuestData.Id, newQuestData.Title);
                newSlot.On();
                _currentQuestsNumber += 1;
            }

            return;
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
            gameObject.SetActive(false);
        }
    }
}