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
    public int Damage { get; private set; }

    public EnumAttackerType AttackerType;

    /// <summary>
    /// 공격자의 위치. Monster의 Target으로 적용
    /// </summary>
    [SerializeField] private Transform _body;

    public Transform Body { get => _body; }

    public void SetDamage(int damage)
    {
        Damage = damage;
    }
}
