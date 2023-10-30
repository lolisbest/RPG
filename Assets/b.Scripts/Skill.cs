using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public int SkillId { get; protected set; }
    [SerializeField] protected float _durationTime;
    [SerializeField] protected AttackCollider[] _attackColliders;
    [SerializeField] protected Vector3 _startPosition;

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

    public void SetAttacker(Transform attacker)
    {
        for (int i = 0; i < _attackColliders.Length; i++)
        {
            _attackColliders[i].SetAttacker(attacker);
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
        SetFormerPosition(startPoint.position);
        transform.position = startPoint.position;
        transform.rotation = startPoint.rotation;
        _startPosition = startPoint.position;
    }

    private void SetFormerPosition(Vector3 initPosition)
    {
        for (int i = 0; i < _attackColliders.Length; i++)
        {
            _attackColliders[i].InitFormerPosition(initPosition);
        }
    }

    protected void SetVelocity(Vector3 velocity)
    {
        for (int i = 0; i < _attackColliders.Length; i++)
        {
            _attackColliders[i].SetVelocity(velocity);
        }
    }
}
