using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Common;
using TMPro;
using RPG.UI;
using RPG.Item;


namespace RPG.UI
{
    public class CurrentQuestDetailWindow : MonoBehaviour
    {
        public TextMeshProUGUI Title;
        public TextMeshProUGUI Description;
        [Header("퀘스트 조건 창")]
        public RectTransform ConditionsRoot;
        public GameObject QuestConditioinSlotPrefab;
        public List<QuestConditionSlot> ConditionSlots;

        [Header("보상 창")]
        public RectTransform RewardsRoot;
        public GameObject RewardSlotPrefab;
        public List<IconItemSlot> RewardItemSlots;

        public TextMeshProUGUI RewardExpText;
        private readonly string RewardExpTextBase = "Reward Exp : {0}";

        //public Npc NpcInCharge;
        public int QuestId;
        private int _defaultCount = 10;

        public Button FinishButton;

        public void Initialize()
        {
            RewardItemSlots = new();
            ConditionSlots = new();

            for(int i = 0; i < _defaultCount; i++)
            {
                AppendConditionSlot();

                AppendRewardSlot();
            }

            OffFinishButton();
        }

        private void AppendConditionSlot()
        {
            GameObject condSlotObject = Instantiate(QuestConditioinSlotPrefab);
            QuestConditionSlot condSlot = condSlotObject.GetComponent<QuestConditionSlot>();
            condSlotObject.transform.SetParent(ConditionsRoot);
            ConditionSlots.Add(condSlot);
            condSlotObject.SetActive(false);
        }

        private void AppendRewardSlot()
        {
            GameObject rewardSlotObject = Instantiate(RewardSlotPrefab);
            IconItemSlot rewardSlot = rewardSlotObject.GetComponent<IconItemSlot>();
            rewardSlot.SetDragable(false);
            rewardSlot.Off();
            rewardSlotObject.transform.SetParent(RewardsRoot);
            RewardItemSlots.Add(rewardSlot);
        }

        public void OpenDetailWindow(StructQuestData questData)
        {
            SetConditions(questData.Conditions);
            SetRewards(questData.RewardItems);
            QuestId = questData.Id;
            Title.text = questData.Title;
            Description.text = questData.Description;
            RewardExpText.text = string.Format(RewardExpTextBase, questData.RewardExp);

            gameObject.SetActive(true);
        }

        private void SetConditions(StructQuestCondition[] conditions)
        {
            if (conditions.Length > ConditionSlots.Count)
                throw new System.NotImplementedException("conditions.Length > ConditionSlots.Coun");

            //ClearConditionSlots();
            OffConditionSlots();

            int passedConditionCount = 0;

            for(int i = 0; i < conditions.Length; i++)
            {
                StructQuestCondition condition = conditions[i];
                QuestConditionSlot slot = ConditionSlots[i];
                slot.On();
                slot.SetInfoCurrent(condition);
                if(condition.CurrentCount >= condition.ObjectiveCount)
                {
                    passedConditionCount += 1;

                }
            }

            if(passedConditionCount == conditions.Length)
            {
                OnFinishButton();
            }
        }

        private void SetRewards(StructIdCount[] rewardItems)
        {
            OffRewardSlots();

            if (rewardItems.Length > RewardItemSlots.Count)
                throw new System.NotImplementedException("rewardItemIds.Length > RewardItemSlots.Coun");

            for (int i = 0; i < rewardItems.Length; i++)
            {
                int itemId = rewardItems[i].Id;
                int itemCount = rewardItems[i].Count;

                RewardItemSlots[i].On();
                RewardItemSlots[i].SetItemInfo(itemId, itemCount, false);
            }
        }

        private void Close()
        {
            QuestId = -1;

            Title.text = string.Empty;
            Description.text = string.Empty;

            gameObject.SetActive(false);

            OffFinishButton();
        }

        public void Finsh()
        {
            InGameUIManager.Instance.FinishQuest(QuestId);
        }

        public void Quit()
        {
            Close();
        }

        private void OffFinishButton()
        {
            FinishButton.gameObject.SetActive(false);
        }

        private void OnFinishButton()
        {
            FinishButton.gameObject.SetActive(true);
        }

        private void ClearConditionSlots()
        {
            foreach (var conditionSlot in ConditionSlots)
            {
                conditionSlot.Clear();
            }
        }

        private void OffConditionSlots()
        {
            foreach (var conditionSlot in ConditionSlots)
            {
                conditionSlot.Off();
            }
        }

        private void OffRewardSlots()
        {
            foreach (var rewardItemSlot in RewardItemSlots)
            {
                rewardItemSlot.Off();
            }
        }
    }
}