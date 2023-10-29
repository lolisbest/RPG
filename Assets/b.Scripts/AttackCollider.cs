using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RPG.Common;


public class AttackCollider : MonoBehaviour
{
    [SerializeField] private int _damage;
    public int Damage { get => _damage; }

    /// <summary>
    /// 공격자의 위치. Monster의 Target으로 적용
    /// </summary>
    [SerializeField] private Transform _attacker;

    public Transform Attacker { get => _attacker; }

    [SerializeField] private float _damageMultiply;

    [SerializeField] private GameObject _hitEffectPrefab;

    [SerializeField] private Rigidbody _rigidbody;

    /// <summary>
    /// 충돌할 경우 파괴할지 말지
    /// </summary>
    [SerializeField] protected bool _isKeepOnHit;

    public void SetDamage(int damage)
    {
        _damage = (int)(damage * _damageMultiply);
    }

    public void SetAttacker(Transform attacker)
    {
        _attacker = attacker;
    }

    /// <summary>
    /// HitEffect 생성
    /// </summary>
    public void OnHitEffect()
    {
        OnHitEffect(transform.position);
    }

    /// <summary>
    /// HitEffect 생성
    /// </summary>
    /// <param name="hitPosition"></param>
    public void OnHitEffect(Vector3 hitPosition)
    {
        if (!_hitEffectPrefab) { Debug.LogWarning($"{Attacker.name}-{name} _hitEffectPrefab is null"); }
        else
        {
            GameObject hitEffect = Instantiate(_hitEffectPrefab);
            hitEffect.transform.position = hitPosition;
        }

        if (!_isKeepOnHit) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{name} OnTriggerEnter. ohter:{other.GetType()} + {other.gameObject.name}");

        // 공격 충돌 대상이 자기 자신이면
        if (other.gameObject == Attacker.gameObject)
        {
            return;
        }

        // 충돌 대상의 콜라이더 위의 위치 중에서 가장 가까운 위치
        Vector3 closestPoint = other.ClosestPoint(transform.position);

        // 공격 충돌 대상이 배경(지형)이 아니라면
        if (other.gameObject.layer != LayerMask.NameToLayer("Background"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                StructAttackHit attackHit = new StructAttackHit()
                {
                    AttackCollider = this,
                    AttackScriptId = this.GetHashCode(),
                    IsBlocked = false,
                    IsApplied = false,
                    RawDamage = this.Damage,
                    HitPosition = closestPoint,
                    Attacker = this.Attacker
                };

                // IDamageable에 공격 충돌 데이터 추가
                damageable.AddAttackHit(attackHit);
                Debug.Log($"{name} Try Add AttackHit to {other.gameObject.name}");
            }
        }
        else
        {
            // 공격 충돌 대상이 지형이면 히트 이펙트 켜기
            // _isKeepOnHit 가 false이면 파괴. 스킬일 경우, 경우에따라 부모 오브젝트는 살아 있을 수 있으나 Skill은 _duration 후에 자동 파괴 됨.
            OnHitEffect(closestPoint);
            if (!_isKeepOnHit) Destroy(gameObject);
        }
    }

    public void SetKeep(bool isKeepOnHit)
    {
        _isKeepOnHit = isKeepOnHit;
    }

    public void SetVelocity(Vector3 velocity)
    {
        if (!_rigidbody) { Debug.LogWarning($"{name}._rigidbody is null"); return; }
        _rigidbody.velocity = velocity;
    }
}
