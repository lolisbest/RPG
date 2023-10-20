using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;

namespace RPG.Common
{
    public enum QuestConditionType
    {
        Talk = 0,
        Kill,
        Move,
        Collect,

    }

    [Serializable]
    public struct StructQuestCondition
    {
        public QuestConditionType Type;
        public string TypeString;
        // 대상 Id: Npc Id, Monster Id, Item Id, 특정 장소의 게임 오브젝트의 Id
        public int TargetId;
        // Kill 숫자 혹은 Collect 숫자, 대화 횟수, 특정 장소에 도착 횟수
        public int CurrentCount;
        public int ObjectiveCount;
        public string Extension;
        public override string ToString()
        {
            return $"{TypeString}:{Type}:TargetId:{TargetId}, CurrentCount:{CurrentCount}, ObjectiveCount:{ObjectiveCount}";
        }
    }

    [Serializable]
    public struct StructQuestData
    {
        public int Id;
        public string Title;
        public string Description;
        public int[] RequiredQuestIds;
        public bool IsClear;
        public StructQuestCondition[] Conditions;
        public StructIdCount[] RewardItems;
        public int RewardExp;
        public int RewardMoney;

        public override string ToString()
        {
            return $"----- {Title} ------\n" +
                $"Id : {Id}\n" +
                $"Description : {Description}\n" +
                $"RequiredQuestIds : {string.Join(", ", RequiredQuestIds)}\n" +
                $"IsClear : {IsClear}\n" +
                $"Conditions : {string.Join(" / ", Conditions)}\n" +
                $"Rewards : {RewardString(RewardItems)}\n" + 
                $"RewardExp : {RewardExp}\n" +
                $"RewardMoney : {RewardMoney}\n" +
                $"--------------------";
        }

        private string RewardString(StructIdCount[] rewardItems)
        {
            StringBuilder retString = new();

            for (int i = 0; i < rewardItems.Length; i++)
            {
                int rewardItemId = rewardItems[i].Id;
                int rewardItemCount = rewardItems[i].Count;

                retString.Append($"{DataBase.Items[rewardItemId].Name}:{rewardItemCount}");
                if(i != rewardItems.Length - 1)
                {
                    retString.Append(" / ");
                }
            }

            return retString.ToString();
        }

        public bool IsClearRequiredQuests(int questId)
        {
            if (DataBase.Quests == null)
            {
                throw new System.NotImplementedException("QuestDataLoad.QuestDataDict is null");
            }

            StructQuestData questData = DataBase.Quests[questId];
            if (!questData.IsClear)
            {
                foreach (var reqQuestId in questData.RequiredQuestIds)
                {
                    StructQuestData requiredQuestData = DataBase.Quests[reqQuestId];
                    if (!requiredQuestData.IsClear)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}