using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;


public class QuestManager : Singleton<QuestManager>
{
    public List<StructQuestData> CurrentInProgressQuests;
    private RPG.UI.UIManager _uiManager;
    public Player @Player;

    public override void Initialize()
    {
        CurrentInProgressQuests = new();
        _uiManager = RPG.UI.UIManager.Instance;
        @Player = GameManager.Instance.Player;
    }

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    // CurrentInProgressQuests 에 퀘스트 추가
    public void AddQuest(int questId)
    {
        if (questId < 1) return;

        if (IsInProgress(questId))
            // 이미 진행 중인 퀘스트라면
            throw new System.Exception($"Already In Progress Quest. Id:{questId}, Title:{DataBase.Quests[questId].Title}");

        if (GameManager.Instance.Player.ClearedQuestIds.Contains(questId))
            throw new System.Exception($"Already Clear Id:{questId}, Title:{DataBase.Quests[questId].Title}");

        Debug.Log($"Add Quest to CurrentInProgressQuests : {DataBase.Quests[questId].Id}:{DataBase.Quests[questId].Title}");
        CurrentInProgressQuests.Add(DataBase.Quests[questId]);
        Debug.Log($"Add Quest {DataBase.Quests[questId].Title}:{questId}");
    }

    // 퀘스트 조건과 관련된 행위를 알림
    public void CallbackQuestCondition(QuestConditionType type, int targetId, int count)
    {
        for (int questIndex = 0; questIndex < CurrentInProgressQuests.Count; questIndex++)
        {
            // 퀘스트 정보 하나 가져오기
            StructQuestData questData = CurrentInProgressQuests[questIndex];
            StructQuestCondition[] conditions = questData.Conditions;

            bool isComplete = true;

            for (int condIndex = 0; condIndex < conditions.Length; condIndex++)
            {
                // 퀘스트 조건 유형과 목표 대상이 일치하는지
                if (conditions[condIndex].Type == type && conditions[condIndex].TargetId == targetId)
                {
                    // 해당 퀘스트 조건의 CurrentCount 셋팅
                    switch (type)
                    {
                        // 말을 걸었냐가 중요하기에 +나 += 차이 없음
                        case QuestConditionType.Talk:
                            //Debug.Log("CallbackQuestCondition QuestConditionType.Talk Increase");
                            conditions[condIndex].CurrentCount = count;
                            break;
                        // 처치 횟수가 중요하므로 += 연산자
                        case QuestConditionType.Kill:
                            conditions[condIndex].CurrentCount += count;
                            break;
                        // 이동해서 도달 했는냐가 중요하기에 +나 += 차이 없음
                        case QuestConditionType.Move:
                            conditions[condIndex].CurrentCount = count;
                            break;
                        // 아이템 보유가 중요하기 때문에 += 연산자 보다는 = 연산자
                        case QuestConditionType.Collect:
                            conditions[condIndex].CurrentCount = count;
                            break;
                        default:
                            throw new System.NotImplementedException($"{typeof(StructQuestCondition)}.{type}");
                    }
                }

                if (conditions[condIndex].ObjectiveCount > conditions[condIndex].CurrentCount)
                {
                    isComplete = false;
                }
            }

            if (isComplete)
            {
                _uiManager.HighlightQuest(questIndex);
            }
            else
            {
                _uiManager.UnhighlightQuest(questIndex);
            }    
        }
    }

    public bool IsInProgress(int questId)
    {
        foreach (var questData in CurrentInProgressQuests)
        {
            if (questData.Id == questId)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Remove Quest from CurrentInProgressQuests and add exp, money to player.
    /// </summary>
    /// <param name="questId"></param>
    /// <returns></returns>
    public bool FinishQuest(int questId)
    {
        for (int i = 0; i < CurrentInProgressQuests.Count; i++)
        {
            StructQuestData copyQuestData = CurrentInProgressQuests[i];
            if (copyQuestData.Id == questId)
            {
                StructQuestCondition[] copyQuestConditions = copyQuestData.Conditions;
                int passedConditionCount = 0;
                for (int conditionIndex = 0; conditionIndex < copyQuestConditions.Length; conditionIndex++)
                {
                    StructQuestCondition copyCondition = copyQuestConditions[conditionIndex];
                    if (copyCondition.CurrentCount >= copyCondition.ObjectiveCount)
                    {
                        passedConditionCount += 1;
                    }
                }

                if (passedConditionCount == copyQuestConditions.Length)
                {
                    CurrentInProgressQuests.RemoveAt(i);
                    Debug.Log($"Removed : {copyQuestData}");
                    GameManager.Instance.Player.AddClearedQuest(questId);
                    // DataBase Quest Updated
                    DataBase.Quests[questId] = copyQuestData;
                    Debug.Log("DataBase.Quets Updated: " + copyQuestData);
                    //Debug.Log("DataBase.Quests :" + DataBase.Quests[questId]);

                    //@Player.AddExperience(copyQuestData.RewardExp);
                    return true;
                }

            }
        }

        return false;
    }
}
