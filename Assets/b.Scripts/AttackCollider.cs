using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RPG.Common;


public enum Axis
{
    X = 0,
    Y,
    Z
}

public class AttackCollider : MonoBehaviour
{
    [SerializeField] private int _damage;
    public int Damage { get => _damage; }

    public EnumAttackerType AttackerType;

    /// <summary>
    /// 공격자의 위치. Monster의 Target으로 적용
    /// </summary>
    [SerializeField] private Transform _body;

    public Transform Body { get => _body; }

    [SerializeField] private float _damageMultiply;

    public void SetDamage(int damage)
    {
        _damage = (int)(damage * _damageMultiply);
    }

    public void SetBody(Transform body)
    {
        _body = body;
    }
}
