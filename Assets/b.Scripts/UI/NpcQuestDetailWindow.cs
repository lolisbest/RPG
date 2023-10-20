using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Common;
using TMPro;
using RPG.UI;
using RPG.Item;
//using RPG.Quest;

namespace RPG.UI
{
    public class NpcQuestDetailWindow : MonoBehaviour
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
        private readonly string RewardExpTextBase = "획득 경험치 : {0}";

        //public Npc NpcInCharge;
        public int QuestId;
        private int _defaultCount = 10;

        public void Initialize()
        {
            RewardItemSlots = new();
            ConditionSlots = new();

            for(int i = 0; i < _defaultCount; i++)
            {
                AppendConditionSlot();

                AppendRewardSlot();
            }
        }

        private void AppendConditionSlot()
        {
            GameObject condSlotObject = Instantiate(QuestConditioinSlotPrefab);
            QuestConditionSlot condSlot = condSlotObject.GetComponent<QuestConditionSlot>();

            condSlotObject.transform.SetParent(ConditionsRoot);
            condSlot.Off();
            condSlot.Clear();
            ConditionSlots.Add(condSlot);
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

        public void OpenDetailWindow(int questId)
        {
            StructQuestData data = DataBase.Quests[questId];
            Debug.Log(data);

            SetConditions(data.Conditions);

            Debug.Log("Rewards " + string.Join("|", data.RewardItems));
            SetRewards(data.RewardItems);
            
            QuestId = questId;
            Title.text = data.Title;
            Description.text = data.Description;
            RewardExpText.text = string.Format(RewardExpTextBase, data.RewardExp);

            gameObject.SetActive(true);
        }

        private void SetConditions(StructQuestCondition[] conditions)
        {
            if (conditions.Length > ConditionSlots.Count)
                throw new System.NotImplementedException("conditions.Length > ConditionSlots.Coun");

            ClearConditionSlots();
            OffConditionSlots();

            for (int i = 0; i < conditions.Length; i++)
            {
                //Debug.Log($"Set Condition #{i} {conditions[i]}");
                StructQuestCondition condition = conditions[i];
                //Debug.Log("condition : " + condition);
                QuestConditionSlot slot = ConditionSlots[i];
                slot.On();
                slot.SetInfoNpc(condition);
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

                RewardItemSlots[i].SetItemInfo(itemId, itemCount, false);
                RewardItemSlots[i].On();
            }
        }

        private void Close()
        {
            QuestId = -1;

            Title.text = string.Empty;
            Description.text = string.Empty;

            gameObject.SetActive(false);

            foreach(var conditionSlot in ConditionSlots)
            {
                conditionSlot.Off();
            }
        }

        public void Accept()
        {

            InGameUIManager.Instance.AddQuestToCurrentQuestsWindow(QuestId);
            Close();
        }

        public void Deny()
        {
            Close();
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