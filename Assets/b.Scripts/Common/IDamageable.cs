using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Common
{
    public interface IDamageable
    {
        public Dictionary<int, StructAttackHit> TakenHits { get; }

        public bool IsDie { get; }

        public void OnDamage(StructAttackHit attackHit);

        public void OnDeath();

        public void AddAttackHit(StructAttackHit attackHit);

        public void ApplyDamage();
    }
}