using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;
using UnityEngine.AI;
using UnityEditor.Animations;

namespace RPG.Monster
{
    [RequireComponent(typeof(NavMeshAgent))]
    public partial class Monster : MonoBehaviour, IStatus, IDamageable
    {
        private StructStatus _status;
        #region IStatus Implements
        public StructRealStatus RealStatus { get; private set; }
        public StructStatus Status
        {
            get => _status;
            private set
            {
                _status = value;

                RealStatus = IStatus.UpdateRealStatus(_status);
                name = $"{_status.Name}:{gameObject.GetHashCode()}";
                MonsterUI.SetName(_status.Name);
                SetAttackColliderDamage(RealStatus.Atk);
            }
        }

        #endregion

        #region Hp, Mp
        public int _hp;
        public int Hp
        {
            get => _hp;
            private set
            {
                _hp = value < 0 ? 0 : value;
                float rate = (float)_hp / RealStatus.MaxHp;
                MonsterUI.UpadteHp(rate);
                //Debug.Log("Hp Rate " + rate);
            }
        }

        public int _mp;
        public int Mp
        {
            get => _mp;
            private set
            {
                _mp = value < 0 ? 0 : value;
                float rate = (float)_mp / RealStatus.MaxMp;
            }
        }
        #endregion

        private void SetAttackColliderDamage(int baseDamage)
        {
            foreach (var attackCollider in _attackColliders)
            {
                attackCollider.SetDamage(baseDamage);
            }
        }

        public void SetMonsterDetails(int monsterId)
        {
            StructMonsterData monsterData = DataBase.Monsters[monsterId];
            Status = monsterData.Status;
            SpawnInterval = monsterData.SpawnInterval;
        }
    }
}