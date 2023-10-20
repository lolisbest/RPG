using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;


public static partial class DataBase
{
    private class MonsterDataBase : IDataLoad
    {
        public string DataFileName { get; private set; } = "MonsterDataBase";
        // <Monster Id, Struct Monster Data>
        public static readonly string MonsterPrefabDirPath = "Prefabs/Monster";
        public static Dictionary<int, StructMonsterData> Monsters;
        public static Dictionary<int, GameObject> MonsterPrefabs;

        public IDataLoad Initialize()
        {
            Monsters = new();
            MonsterPrefabs = new();
            return this;
        }

        public void Load()
        {
            string filePath = DataRootDirPath + DataFileName;
            StructMonsterData[] monsterDataArray = RPG.Utils.ReadJson.Read<StructMonsterData>(filePath);

            for (int i = 0; i < monsterDataArray.Length; i++)
            {
                StructMonsterData monsterData = monsterDataArray[i];
                //Debug.Log("Add " + monsterData);
                Monsters.Add(monsterData.Id, monsterData);
                GameObject prefab = Resources.Load<GameObject>(MonsterPrefabDirPath + "/" + monsterData.Status.Name);
                MonsterPrefabs.Add(monsterData.Id, prefab);
                Debug.Log(monsterData);
            }

            Debug.Log($"Loaded {Monsters.Count}/{monsterDataArray.Length} of Monsters from {DataFileName}");

        }
    }
}