using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;

public static partial class DataBase
{
    private class PlayerDataBase : IDataLoad
    {
        public string DataFileName { get; private set; } = "PlayerDataBase";

        public static Dictionary<int, StructPlayerData> Players;
        public IDataLoad Initialize()
        {
            Players = new();
            return this;
        }

        public void Load()
        {
            string filePath = DataRootDirPath + DataFileName;

            StructPlayerData[] playerDataArray = RPG.Utils.ReadJson.Read<StructPlayerData>(filePath);

            for (int i = 0; i < playerDataArray.Length; i++)
            {
                StructPlayerData playerData = playerDataArray[i];
                Players.Add(playerData.DataId, playerData);
                Debug.Log($"playerData : {playerData}");
            }
            
            Debug.Log($"Loaded {Players.Count}/{playerDataArray.Length} of Players from {DataFileName}");
        }
    }
}

