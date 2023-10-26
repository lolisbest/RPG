using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : IKnockback
{
    public void StartKnockback()
    {
        //Debug.Log("Before " + transform.position);
        //_controller.Move(-transform.forward * multiply);
        //Debug.Log("After " + transform.position);
        //Debug.Log("delta " + -transform.forward * multiply);
        //Debug.DrawRay(transform.position + transform.up * 2f, -transform.forward * 1000f, Color.red, 0.5f);

        //Debug.Log("transform.forward " + transform.forward);
        //Debug.Log("transform.forward " + V);

        StartCoroutine(Knockback());
    }

    public IEnumerator Knockback()
    {
        float knockBackLastingTime = 0.3f;
        float lastingTime = 0f;

        while (lastingTime < knockBackLastingTime)
        {
            Vector3 reducedKnockBackSpeed = -transform.forward * _knockbackMultifly.Evaluate(lastingTime) * 10f;
            _inputController.Move(reducedKnockBackSpeed * Time.deltaTime);
            yield return new WaitForFixedUpdate();
            lastingTime += Time.deltaTime;
        }
    }

}
