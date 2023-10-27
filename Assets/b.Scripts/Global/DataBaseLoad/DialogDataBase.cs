using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;

public static partial class DataBase
{
    private class DialogDataBase : IDataLoad
    {
        public string DataFileName { get; private set; } = "DialogDataBase";
        // <Npc Id, NpcData>
        public static Dictionary<int, StructDialogData> Dialogs { get; private set; }

        public void Load()
        {
            string filePath = DataRootDirPath + DataFileName;

            StructDialogData[] dialogDataArray = Utils.JsonHelper.Read<StructDialogData>(filePath);
            for (int i = 0; i < dialogDataArray.Length; i++)
            {
                StructDialogData dialogData = dialogDataArray[i];
                Dialogs.Add(dialogData.Id, dialogData);
            }

            Debug.Log($"Loaded {Dialogs.Count}/{dialogDataArray.Length} of Dialogs from {DataFileName}");
        }

        public IDataLoad Initialize()
        {
            Dialogs = new();
            return this;
        }
    }
}