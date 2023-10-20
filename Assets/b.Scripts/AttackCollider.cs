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

    public void SetDamage(int damage)
    {
        Damage = damage;
    }

    public void OnBlocked()
    {
        IsBlocked = true;
    }

    private void OnEnable()
    {
        IsBlocked = false;
    }

    private void OnDisable()
    {
        IsBlocked = false;
    }
}
