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

    //private static PlayerDataBase _playerDataBase;
    //public static Dictionary<int, StructPlayerData> Players { get => PlayerDataBase.Players; }

    private static MonsterDataBase _monsterDataBase;

    public static Dictionary<int, StructMonsterData> Monsters { get => MonsterDataBase.Monsters; }
    public static Dictionary<int, GameObject> MonsterPrefabs { get => MonsterDataBase.MonsterPrefabs; }

    private static DialogDataBase _dialogDataBase;

    public static readonly int StatusPointsPerLevelUp = 3;
    public static Dictionary<int, StructDialogData> Dialogs { get => DialogDataBase.Dialogs; }
    public static void Load()
    {
        _itemDataBase.Load();
        _npcDataBase.Load();
        _questDataBase.Load();
        //_playerDataBase.Load();
        _monsterDataBase.Load();
        _dialogDataBase.Load();

        //ShowReservedPath();
    }

    public static void Initialize()
    {
        _itemDataBase = (ItemDataBase)(new ItemDataBase().Initialize());
        _npcDataBase = (NpcDataBase)(new NpcDataBase().Initialize());
        _questDataBase = (QuestDataBase)(new QuestDataBase().Initialize());
        //_playerDataBase = (PlayerDataBase)(new PlayerDataBase().Initialize());
        _monsterDataBase = (MonsterDataBase)(new MonsterDataBase().Initialize());
        _dialogDataBase = (DialogDataBase)(new DialogDataBase().Initialize());
    }

    public static (int, int) ExpTable(int level, int exp)
    {
        if(level * 100 > exp)
        {
            return (level, exp);
        }

        int newLevel = level;
        int newExp = exp;
        while(newExp >= newLevel * 100)
        {
            newExp -= newLevel * 100;
            newLevel++;
        }

        return (newLevel, newExp);
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

