using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RPG.Common;

namespace RPG.UI
{
    public class QuestConditionSlot : MonoBehaviour
    {
        public static readonly string NpcTalkDescription = "{0} 와 대화 하기";
        public static readonly string NpcKillDescription = "{0} {1} 마리 처치";
        public static readonly string NpcMoveDescription = "{0} 로 이동";
        public static readonly string NpcCollectDescription = "{0} {1} 개 수집";

        //public static readonly string AchievedTalkDescription = "<s>{0} 와 대화 하기</s>";
        //public static readonly string AchievedKillDescription = "<s>{0} {1} 마리 처치</s>";
        //public static readonly string AchievedMoveDescription = "<s>{0} 로 이동</s>";
        //public static readonly string AchievedCollectDescription = "<s>{0} {1} 개 수집</s>";

        public static readonly string CurrentTalkDescription = "{0} 와 대화 하기";
        public static readonly string CurrentKillDescription = "{0} {1} / {2} 마리 처치";
        public static readonly string CurrentMoveDescription = "{0} 로 이동";
        public static readonly string CurrentCollectDescription = "{0} {1} / {2} 개 수집";

        public static readonly string CurrentAchievedTalkDescription = "<s>{0} 와 대화 하기</s>";
        public static readonly string CurrentAchievedKillDescription = "<s>{0} {1} / {2} 마리 처치</s>";
        public static readonly string CurrentAchievedMoveDescription = "<s>{0} 로 이동</s>";
        public static readonly string CurrentAchievedCollectDescription = "<s>{0} {1} / {2} 개 수집</s>";


        public TextMeshProUGUI Text;

        public void SetInfoNpc(StructQuestCondition currentCondition)
        {
            string formattedString = string.Empty;
            switch (currentCondition.Type)
            {
                case QuestConditionType.Talk:
                    Text.text = string.Format(NpcTalkDescription, DataBase.Npcs[currentCondition.TargetId].Name);
                    break;
                case QuestConditionType.Kill:
                    Text.text = string.Format(NpcKillDescription, DataBase.Monsters[currentCondition.TargetId].Status.Name, currentCondition.ObjectiveCount);
                    break;
                case QuestConditionType.Move:
                    throw new System.NotImplementedException($"QuestConditioinSlot.SetInfo {currentCondition.Type}");
                case QuestConditionType.Collect:
                    throw new System.NotImplementedException($"QuestConditioinSlot.SetInfo {currentCondition.Type}");
                default:
                    throw new System.NotImplementedException($"QuestConditioinSlot.SetInfo {currentCondition.Type}");

            }
        }

        public void SetInfoCurrent(StructQuestCondition currentCondition)
        {
            string formattedString = string.Empty;
            switch (currentCondition.Type)
            {
                case QuestConditionType.Talk:
                    if (currentCondition.CurrentCount >= currentCondition.ObjectiveCount)
                    {
                        formattedString = CurrentAchievedTalkDescription;
                    }
                    else
                    {
                        formattedString = CurrentTalkDescription;
                    }
                    Text.text = string.Format(formattedString, DataBase.Npcs[currentCondition.TargetId].Name);
                    break;
                case QuestConditionType.Kill:
                    if (currentCondition.CurrentCount >= currentCondition.ObjectiveCount)
                    {
                        formattedString = CurrentAchievedKillDescription;
                    }
                    else
                    {
                        formattedString = CurrentKillDescription;
                    }
                    Text.text = string.Format(
                        formattedString,
                        DataBase.Monsters[currentCondition.TargetId].Status.Name,
                        currentCondition.CurrentCount, currentCondition.ObjectiveCount
                    );
                    break;
                case QuestConditionType.Move:
                    throw new System.NotImplementedException($"QuestConditioinSlot.SetInfo {currentCondition.Type}");
                case QuestConditionType.Collect:
                    throw new System.NotImplementedException($"QuestConditioinSlot.SetInfo {currentCondition.Type}");
                default:
                    throw new System.NotImplementedException($"QuestConditioinSlot.SetInfo {currentCondition.Type}");

            }
        }


        public void On()
        {
            gameObject.SetActive(true);
        }

        public void Off()
        {
            gameObject.SetActive(false);
        }

        public void Clear()
        {
            Text.text = string.Empty;
        }
    }
}