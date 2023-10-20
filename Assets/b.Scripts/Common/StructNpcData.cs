using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPG.Common
{
    [Flags]
    public enum NpcService
    {
        Talk = 1,
        Quest = 2,
        Shop = 4,
        Rest = 8,
    }

    [Serializable]
    public struct StructNpcData
    {
        public int Id;
        public string Name;
        public string[] ServiceStrings;
        public int[] QuestIds;
        public int[] StartDialogIds;

        public NpcService Services;

        public int[] SellingItems;

        public override string ToString()
        {
            return $"----- {Name} ------\n" +
                $"Id : {Id}\n" +
                $"ServiceStrings : {string.Join(", ", ServiceStrings)}\n" +
                $"Services : {Services}\n" +
                $"QuestIds : {string.Join(", ", QuestIds)}\n" +
                $"--------------------";
        }
    }
}