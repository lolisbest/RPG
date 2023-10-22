using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RPG.Utils;
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

    public bool IsBlocked { get; private set; }

    [SerializeField] private Collider _collider;


    [SerializeField] private Transform _body;
    public Transform Body { get => _body; }

    public void SetDamage(int damage)
    {
        Damage = damage;
    }

    public void OnBlocked()
    {
        IsBlocked = true;
        _collider.enabled = false;
    }

    private void OnEnable()
    {
        IsBlocked = false;
        _collider.enabled = true;
    }

    private void OnDisable()
    {
        IsBlocked = false;
        _collider.enabled = true;
    }
}
