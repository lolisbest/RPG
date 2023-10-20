using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;

public static partial class DataBase
{
    private class NpcDataBase : IDataLoad
    {
        public string DataFileName { get; private set; } = "NpcDataBase";
        // <Npc Id, NpcData>
        public static Dictionary<int, StructNpcData> Npcs { get; private set; }

        public void Load()
        {
            string filePath = DataRootDirPath + DataFileName;

            StructNpcData[] npcDataArray = RPG.Utils.ReadJson.Read<StructNpcData>(filePath);
            for (int i = 0; i < npcDataArray.Length; i++)
            {
                StructNpcData npcData = npcDataArray[i];
                foreach (var valueString in npcData.ServiceStrings)
                {
                    npcData.Services |= RPG.Utils.EnumParse.StringToEnum<NpcService>(valueString);
                }

                Npcs.Add(npcData.Id, npcData);
            }

            Debug.Log($"Loaded {Npcs.Count}/{npcDataArray.Length} of Npcs from {DataFileName}");
        }

        public IDataLoad Initialize()
        {
            Npcs = new();
            return this;
        }
    }
}
