using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPG.Common
{
    [Serializable]
    public struct StructDialogData
    {
        public int Id;
        public string Text;
        public int NextId;
        public int[] RequiredNotClearedQuestIds;
    }
}