using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;
using UnityEngine.AI;
using UnityEditor.Animations;

namespace RPG.Monster
{
    [RequireComponent(typeof(NavMeshAgent))]
    public partial class Monster : DamageableStatusMonoBehaviour
    {
        #region IDamageable Property Implements
        public override void OnDeath()
        {
            //Debug.Log($"{name} die");
            _questManager.CallbackQuestCondition(QuestConditionType.Kill, Id, 1);
            IsDie = true;
            gameObject.SetActive(false);

            _itemDropper.DropItemBox(Id, _dropStartPosition.position);
        }

        public override void OnDamage(StructAttackHit attackHit)
        {
            int realDamage = Utils.Calculate.RealDamage(attackHit.RawDamage, RealStatus.Def);

            Debug.Log($"{name} : {Hp} -> {Hp - realDamage}");

            //Debug.Log("DropStartPosition.position " + DropStartPosition.position);
            InGameUIManager.Instance.ShowDamageText(realDamage, _dropStartPosition.position);

            try
            {
                attackHit.AttackCollider.OnHitEffect(attackHit.HitPosition);
            }
            catch (System.Exception e)
            {
                Debug.Log($"attackHit.Attacker.name : {attackHit.Attacker.name}");
                Debug.Log($"attackHit.AttackCollider : {attackHit.AttackCollider}");
                throw e;
            }

            Hp -= realDamage;
            if (Hp <= 0f)
            {
                OnDeath();
            }
            else
            {
                if (!ToAttackTarget)
                {
                    Debug.Log("ToAttackTarget " + attackHit.Attacker.name);
                    // 현재 공격 중인 대상이 없고 받은 피해가 0보다 크면 공격 대상 설정
                    if (realDamage > 0) ToAttackTarget = attackHit.Attacker;
                }
            }
        }
        #endregion

        #region IStatus Override
        public override StructRealStatus RealStatus { get; protected set; }

        public override StructStatus Status
        {
            get => _status;
            protected set
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
        public override int Hp
        {
            get => _hp;
            protected set
            {
                _hp = value < 0 ? 0 : value;
                float rate = (float)_hp / RealStatus.MaxHp;
                MonsterUI.UpadteHp(rate);
                //Debug.Log("Hp Rate " + rate);
            }
        }

        public override int Mp
        {
            get => _mp;
            protected set
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
                attackCollider.SetAttacker(transform);
                attackCollider.SetDamage(baseDamage);
                attackCollider.SetKeep(true);
            }
        }
    }
}