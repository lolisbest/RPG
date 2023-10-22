using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;

public class Defence : MonoBehaviour
{
    public Vector3 TakenImpactDirection { get; private set; }
    public bool HasBlocked { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        AttackCollider attackCollider = other.GetComponent<AttackCollider>();
        if (attackCollider != null)
        {
            //Debug.Log($"OnTriggerEnter {other.name}");

            if (attackCollider.AttackerType == EnumAttackerType.Player)
                return;

            Vector3 defenceToAttack = attackCollider.transform.position - transform.position;
            if(Vector3.Dot(defenceToAttack, transform.forward) > 0)
            {
                Debug.Log("Denfence Ok");
                HasBlocked = true;
                attackCollider.OnBlocked();
            }
            else
            {
                Debug.Log("Denfence Fail");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        AttackCollider attackCollider = other.GetComponent<AttackCollider>();
        if (attackCollider != null)
        {
            Debug.Log($"OnTriggerStay {other.name}");

            if (attackCollider.AttackerType == EnumAttackerType.Player)
                return;

            //Vector3 defenceToAttack = (attackCollider.transform.up - transform.position).normalized;
            float dot = Vector3.Dot(attackCollider.transform.up, transform.forward);

            if (Mathf.Abs(dot) > 0.05f)
            {
                Debug.Log("제대로 막음");
                HasBlocked = true;
                attackCollider.OnBlocked();
            }
            else
            {
                Debug.Log("막지 못 함");
            }
        }
    }

    private void OnEnable()
    {
        HasBlocked = false;
    }

    public void LateUpdate()
    {
        HasBlocked = false;
    }
}
