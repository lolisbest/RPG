using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPG.Common
{
    [Serializable]
    public struct StructMonsterData
    {
        //public static readonly string PrefabDirPath = "Prefabs/Monster/";

        public int Id;

        public StructStatus Status;

        public StructIdCount[] DropItems;

        public int DropExperience { get => Status.Experience; }
        public string PrefabPath;
        public GameObject Prefab;

        public float SpawnInterval;

        public override string ToString()
        {
            return $"Status : {Status}\n" +
                $"Id : {Id}\n" +
                "-------------------------------";
        }

    }
}