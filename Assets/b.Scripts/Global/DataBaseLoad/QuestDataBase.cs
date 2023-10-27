using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;

public static partial class DataBase
{
    private class QuestDataBase : IDataLoad
    {
        public string DataFileName { get; private set; } = "QuestDataBase";
        // <Npc Id, NpcData>
        public static Dictionary<int, StructQuestData> Quests { get; private set; }

        public void Load()
        {
            string filePath = DataRootDirPath + DataFileName;

            StructQuestData[] questDataArray = Utils.JsonHelper.Read<StructQuestData>(filePath);
            for (int i = 0; i < questDataArray.Length; i++)
            {
                StructQuestData questData = questDataArray[i];
                for(int condIndex = 0; condIndex < questData.Conditions.Length; condIndex++)
                {
                    StructQuestCondition condition = questData.Conditions[condIndex];
                    condition.Type = Utils.StringToEnum<QuestConditionType>(condition.TypeString);
                    questData.Conditions[condIndex] = condition;
                }

                Quests.Add(questData.Id, questData);
                //Debug.Log(questData);
            }

            Debug.Log($"Loaded {Quests.Count}/{questDataArray.Length} of Quests from {DataFileName}");
        }

        public IDataLoad Initialize()
        {
            Quests = new();
            return this;
        }
    }
}