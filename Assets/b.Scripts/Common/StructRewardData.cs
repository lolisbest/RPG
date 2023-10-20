using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPG.Common
{
    [Serializable]
    public struct StructIdCount
    {
        public int Id;
        public int Count;

        public override string ToString()
        {
            return $"Id:{Id}, Count:{Count}";
        }
    }
}