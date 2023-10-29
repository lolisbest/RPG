using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;

public abstract class DamageableStatusMonoBehaviour : MonoBehaviour, IDamageable, IStatus
{
    #region IStatus Implements
    [SerializeField] protected StructStatus _status;

    public virtual StructRealStatus RealStatus { get; protected set; }

    /// <summary>
    /// IsChangedStatus = true;
    /// </summary>
    public virtual StructStatus Status
    {
        get => _status;
        protected set
        {
            _status = value;
            IsChangedStatus = true;
        }
    }

    public virtual void SetStatus(StructStatus status)
    {
        Status = status;
    }


    public virtual bool IsChangedStatus { get; set; }
    #endregion

    #region IDamageable Implements
    public virtual bool IsDie { get; protected set; }
    public virtual void OnDamage(StructAttackHit attackHit)
    {
        int realDamage = Utils.Calculate.RealDamage(attackHit.RawDamage, RealStatus.Def);

        Hp -= realDamage;

        if (Hp <= 0f)
        {
            OnDeath();
        }
    }

    public abstract void OnDeath();

    public virtual Dictionary<int, StructAttackHit> TakenHits { get; protected set; }

    // 공격 피해를 담아 줌. 아직 적용된 피해가 아니고 중복되는 경우 막은 여부 취합.
    public virtual void AddAttackHit(StructAttackHit attackHit)
    {
        //Debug.Log($"Add AttackHit {attackHit.AttackCollider.name} {attackHit.AttackCollider.gameObject.activeSelf}");
        if (TakenHits.ContainsKey(attackHit.AttackScriptId))
        {
            // 이미 존재하는 AttackCollider Id 라면
            StructAttackHit oldAttackHit = TakenHits[attackHit.AttackScriptId];

            // 이미 적용된 피해라면 변경 없이 메서드 종료
            if (oldAttackHit.IsApplied) return;

            // 막은 여부를 OR 연산 -> Player Collider와 Defence Collider가 동시에 닿았을 경우 방어 성공 판정
            oldAttackHit.IsBlocked |= attackHit.IsBlocked;
            oldAttackHit.HitPosition = oldAttackHit.HitPosition == Vector3.zero ? attackHit.HitPosition : oldAttackHit.HitPosition;
            TakenHits[oldAttackHit.AttackScriptId] = oldAttackHit;
        }
        else
        {
            // 새로운 공격 피해라면 추가
            TakenHits.Add(attackHit.AttackScriptId, attackHit);
        }
    }

    public virtual void ApplyDamage()
    {
        int[] attackHitIds = new int[TakenHits.Count];
        int copyStartIndex = 0;
        TakenHits.Keys.CopyTo(attackHitIds, copyStartIndex);

        for (int i = 0; i < attackHitIds.Length; i++)
        {
            int attackScriptId = attackHitIds[i];
            StructAttackHit attackHit = TakenHits[attackScriptId];

            //Debug.Log($"Attack " + attackHit.AttackCollider.name);

            // AttackCollider가 파괴되었다면 -> 스킬의 경우 파괴 됨
            if (attackHit.AttackCollider == null)
            {
                TakenHits.Remove(attackScriptId);
            }
            // AttackCollider가 비활성화되었다면 -> 기본 공격의 경우 파괴되지 않기 때문에
            else if (!attackHit.AttackCollider.gameObject.activeSelf)
            {
                // 공격 콜라이더 오브젝트가 비활성화 되었다면 제거
                //Debug.Log("WasApplied Damage " + TakenHits.Count);
                TakenHits.Remove(attackScriptId);
                //Debug.Log("Removed Damage " + TakenHits.Count);
            }
            else
            {
                if (!attackHit.IsApplied)
                {
                    // 아직 대미지 판정을 안 했다면
                    //Debug.Log("Now Apply Damage");

                    OnDamage(attackHit);

                    // 판정 끝남을 체크
                    attackHit.IsApplied = true;
                    TakenHits[attackScriptId] = attackHit;
                }
            }
        }
    }
    #endregion

    #region Hp, Mp
    [SerializeField] protected int _hp;

    public abstract int Hp { get; protected set; }

    [SerializeField] protected int _mp;

    public abstract int Mp { get; protected set; }
    #endregion
}
