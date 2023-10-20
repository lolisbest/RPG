using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IKnockback
{
    public void StartKnockback();

    public IEnumerator Knockback();
}
