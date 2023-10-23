using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;
using System.Linq;

public interface IDataLoad
{
    public string DataFileName { get; }

    public IDataLoad Initialize();

    public void Load();
}

public static partial class DataBase
{
    // XXXDataBase는 IDataLoad를 상속 받기 때문에 static으로 선언할 수 없음
    // 따라서 _XXXDataBase 객체를 생성하여 Initialize, Load를 호출해야 함
    public static string DataRootDirPath { get; private set; } = "JsonData/";

    private static ItemDataBase _itemDataBase;
    // <Item Id, StructItemData>
    public static Dictionary<int, StructItemData> Items { get => ItemDataBase.Items; }

    private static NpcDataBase _npcDataBase;
    public static Dictionary<int, StructNpcData> Npcs { get => NpcDataBase.Npcs; }

    private static QuestDataBase _questDataBase;
    public static Dictionary<int, StructQuestData> Quests { get => QuestDataBase.Quests; }

    // Server..
    //private static PlayerDataBase _playerDataBase;
    //public static Dictionary<int, StructPlayerData> Players { get => PlayerDataBase.Players; }

    private static MonsterDataBase _monsterDataBase;

    public static Dictionary<int, StructMonsterData> Monsters { get => MonsterDataBase.Monsters; }
    public static Dictionary<int, GameObject> MonsterPrefabs { get => MonsterDataBase.MonsterPrefabs; }

    private static DialogDataBase _dialogDataBase;

    public static Dictionary<int, StructDialogData> Dialogs { get => DialogDataBase.Dialogs; }

    public static readonly int StatusPointsPerLevelUp = 3;

    private static SkillDataBase _skillDataBase;

    public static Dictionary<int, StructSkillData> Skills { get => SkillDataBase.Skills; }

    public static void Load()
    {
        _itemDataBase.Load();
        _npcDataBase.Load();
        _questDataBase.Load();
        _monsterDataBase.Load();
        _dialogDataBase.Load();
        _skillDataBase.Load();

        //ShowReservedPath();
    }

    public static void Initialize()
    {
        _itemDataBase = (ItemDataBase)(new ItemDataBase().Initialize());
        _npcDataBase = (NpcDataBase)(new NpcDataBase().Initialize());
        _questDataBase = (QuestDataBase)(new QuestDataBase().Initialize());
        _monsterDataBase = (MonsterDataBase)(new MonsterDataBase().Initialize());
        _dialogDataBase = (DialogDataBase)(new DialogDataBase().Initialize());
        _skillDataBase = (SkillDataBase)(new SkillDataBase().Initialize());

    }


    /// <summary>
    /// return (새로운 레벨, 캐릭터가 보유한 경험치량, 부족한 경험치량)
    /// </summary>
    /// <param name="currentLevel"></param>
    /// <param name="currentExp"></param>
    /// <returns></returns>
    public static (int, int, int) ExpTable(int currentLevel, int currentExp)
    {
        int requiredExp = currentLevel * 100;

        if (requiredExp > currentExp)
        {
            return (currentLevel, currentExp, requiredExp - currentExp);
        }

        int newLevel = currentLevel;
        int newExp = currentExp;

        while(newExp >= requiredExp)
        {
            newExp -= requiredExp;
            newLevel++;
            requiredExp = newLevel * 100;
        }

        return (newLevel, newExp, requiredExp - newExp);
    }

    public static void ShowReservedPath()
    {
        Debug.Log("Application.dataPath: " + Application.dataPath);
        Debug.Log("Application.temporaryCachePath: " + Application.temporaryCachePath);
        Debug.Log("Application.consoleLogPath: " + Application.consoleLogPath);
        Debug.Log("Application.persistentDataPath: " + Application.persistentDataPath);
        Debug.Log("Application.streamingAssetsPath: " + Application.streamingAssetsPath);
    }
}


