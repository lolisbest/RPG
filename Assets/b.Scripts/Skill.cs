using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] protected float _durationTime;
    [SerializeField] protected AttackCollider[] _attackColliders;

    [SerializeField] private GameObject _hitEffectPrefab;
    [SerializeField] private bool _isKeepOnHit;
    protected virtual void OnTriggerEnter(Collider other)
    {
        TryHit(other);
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        TryHit(other);
    }

    private void TryHit(Collider other)
    {
        if (!other.gameObject.CompareTag(StringStatic.MonsterTag))
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Background")) return;
        }

        if (_isKeepOnHit) return;

        Debug.Log($"Hit Success {other.gameObject.name}");
        GameObject hitEffect = Instantiate(_hitEffectPrefab);
        hitEffect.transform.position = transform.position;
        Destroy(gameObject);
    }

    public virtual void On()
    {
        if (_durationTime <= 0f) Debug.LogWarning($"{name}._durationTime is equal or less then 0");
        gameObject.SetActive(true);
        Destroy(gameObject, _durationTime);
    }

    protected virtual void Awake()
    {
        name = $"{name}[{gameObject.GetHashCode()}]";
    }

    public void SetBody(Transform body)
    {
        for (int i = 0; i < _attackColliders.Length; i++)
        {
            _attackColliders[i].SetBody(body);
        }
    }

    public void SetDamage(int damageFactor)
    {
        for (int i = 0; i < _attackColliders.Length; i++)
        {
            _attackColliders[i].SetDamage(damageFactor);
        }
    }

    public void SetTransformState(Transform startPoint)
    {
        transform.position = startPoint.position;
        transform.rotation = startPoint.rotation;
    }
}
