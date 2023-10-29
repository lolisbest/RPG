using RPG.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.UI;
using RPG.Input;
using UnityEngine.SceneManagement;
using System;

public partial class Player : DamageableStatusMonoBehaviour
{
    #region IStatus Override
    public override StructRealStatus RealStatus { get; protected set; }

    /// <summary>
    /// IsChangedStatus = true;
    /// </summary>
    public override StructStatus Status
    {
        get => _status;
        protected set
        {
            _status = value;

            int[] equipIds = CurrentEquips();

            IsChangedStatus = true;
            RealStatus = IStatus.UpdateRealStatus(_status, equipIds);
            _baseSlashAttackCollider.SetDamage(RealStatus.Atk);
            _baseSlashAttackCollider.SetKeep(true);
        }
    }
    #endregion

    #region IDamageable Override
    public override void OnDamage(StructAttackHit attackHit)
    {
        int realDamage = Utils.Calculate.RealDamage(attackHit.RawDamage, RealStatus.Def);

        if (attackHit.IsBlocked)
        {
            // 막은 경우, 데미지 경감, 방어 이펙트,  넉백
            Hp -= realDamage / 5;
            _defence.OnBlockSuccess(attackHit.HitPosition);
            StartKnockback();
        }
        else
        {
            // 막지 못 한 경우 히트 애니메이션 재생
            Hp -= realDamage;
            _inputController.SetHitAnimation();
        }

        if (Hp <= 0f)
        {
            OnDeath();
        }
    }

    public override void OnDeath()
    {
        _inputController.SetDeath();
        IsDie = true;
    }
    #endregion

    #region Hp, Mp Override
    public override int Hp
    {
        get => _hp;
        protected set
        {
            //Debug.Log($"_hp {_hp} -> {value}");
            _hp = value < 0 ? 0 : value;
            float rate = (float)_hp / RealStatus.MaxHp;
            InGameUIManager.Instance.UpdateHpGauge(rate);
        }
    }

    public override int Mp
    {
        get => _mp;
        protected set
        {
            _mp = value < 0 ? 0 : value;
            float rate = (float)_mp / RealStatus.MaxMp;
            InGameUIManager.Instance.UpdateMpGauge(rate);
        }
    }
    #endregion


    public override Dictionary<int, StructAttackHit> TakenHits { get; protected set; }

    public override void SetStatus(StructStatus status)
    {
        Status = status;
        IsChangedStatus = true;
    }
}
