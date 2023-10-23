using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Common
{
    public struct StructAttackHit
    {
        public int AttackScriptId;
        public bool IsBlocked;
        public bool IsApplied;
        public int RawDamage;
        public AttackCollider AttackCollider;
    }
}
