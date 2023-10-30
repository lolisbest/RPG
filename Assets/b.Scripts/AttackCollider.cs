using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RPG.Common;


public class AttackCollider : MonoBehaviour
{
    public static readonly string[] CollidableLayers = { "Monster", "Background" };
    private readonly float _defaultHitEffectLifeTime = 0.5f;

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

    private Vector3 _formerPosition0;
    private Vector3 _formerPosition1;
    private Vector3 _formerPosition2;

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

            Destroy(hitEffect, _defaultHitEffectLifeTime);
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

        //// 충돌 대상의 콜라이더 위의 위치 중에서 가장 가까운 위치
        //Vector3 hitPosition = other.ClosestPoint(transform.position);
        Vector3 hitPosition = transform.position;

        // fixedUpdate로 3회 이전의 위치에서 현재 위치까지의 Rayacast로 충돌 위치 비교적 정확하게 구하기
        if (GetHitPoint(_formerPosition2, transform.position, other, out Vector3 correctedPosition))
        {
            hitPosition = correctedPosition;
        }

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
                    HitPosition = hitPosition,
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
            OnHitEffect(hitPosition);
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

    private void FixedUpdate()
    {
        _formerPosition2 = _formerPosition1;
        _formerPosition1 = _formerPosition0;
        _formerPosition0 = transform.position;

        //Debug.Log("FixedUpdate _formerPosition0 : " + _formerPosition0);
        //Debug.Log("FixedUpdate _formerPosition1 : " + _formerPosition1);
        //Debug.Log("FixedUpdate _formerPosition2 : " + _formerPosition2);
    }

    public void InitFormerPosition(Vector3 startPosition)
    {
        _formerPosition0 = startPosition;
        _formerPosition1 = startPosition;
        _formerPosition2 = startPosition;
    }

    /// <summary>
    /// 스킬이 시작된 위치에서 목표 지점까지 Raycast를 실행하여 목표 콜라이더를 검출 여부 반환. 검출되면 충돌 위치 hitPoint에 저장
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <param name="targetCollider"></param>
    /// <param name="hitPoint"></param>
    /// <returns></returns>
    public bool GetHitPoint(Vector3 startPoint, Vector3 endPoint, Collider targetCollider, out Vector3 hitPoint)
    {
        //Debug.Log("_formerPosition0 : " + _formerPosition0);
        //Debug.Log("_formerPosition1 : " + _formerPosition1);
        //Debug.Log("_formerPosition2 : " + _formerPosition2);

        Vector3 vector = endPoint - startPoint;
        Ray ray = new Ray(startPoint, vector.normalized);
        
        Debug.DrawLine(startPoint, endPoint, Color.red, 60f);

        int layerMask = 0;
        foreach (var layerName in CollidableLayers)
        {
            layerMask += 1 << LayerMask.NameToLayer(layerName);
        }

        //Debug.Log("GetHitPoint layerMask " + layerMask);

        var hits = Physics.RaycastAll(ray, vector.magnitude, layerMask);
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                Debug.Log($"hit {hit.collider.gameObject.name}");
                if (hit.collider == targetCollider)
                {
                    hitPoint = hit.point;
                    return true;
                }
            }
        }

        hitPoint = Vector3.zero;
        return false;
    }
}
