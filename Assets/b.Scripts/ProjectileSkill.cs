using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileSkill : Skill
{
    [SerializeField] private Rigidbody _rigidbody;
    
    [SerializeField] float _speed;

    public Vector3 Direction { get; private set; }

    public void SetDirection(Vector3 direction)
    {
        Direction = direction.normalized;
    }

    public void SetDirection(Transform target)
    {
        Vector3 targetVector = target.position - transform.position;
        SetDirection(targetVector);
    }

    void Start()
    {
        SetVelocity(Direction * _speed);
    }
}
