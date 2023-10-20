using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Common
{
    public interface IDamageable
    {
        public void OnDamage(int damageAmount);
        public void OnDeath();
    }
}