using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;
using System;


public class Defence : MonoBehaviour
{
    public Vector3 TakenImpactDirection { get; private set; }
    //public bool HasBlocked { get; private set; }

    [SerializeField] private GameObject _defenceEffectPrefab;

    public event Action<StructAttackHit> OnCollision;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"OnTriggerEnter {other.name}");
        TryDefence(other);
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log($"OnTriggerStay {other.name}");
        TryDefence(other);
    }

    private void TryDefence(Collider other)
    {
        AttackCollider attackCollider = other.GetComponent<AttackCollider>();
        if (attackCollider != null)
        {
            if (attackCollider.AttackerType == EnumAttackerType.Player)
                return;

            float dot = Vector3.Dot(attackCollider.transform.up, transform.forward);
            Debug.Log("dot " + dot);

            if (dot < 0f)
            {
                //Debug.Log("Denfence Ok");

                OnCollision?.Invoke(
                    new StructAttackHit {
                        AttackCollider = attackCollider,
                        AttackScriptId = attackCollider.GetHashCode(),
                        IsBlocked = true,
                        IsApplied = false,
                        RawDamage = attackCollider.Damage});
            }
            else
            {
                //Debug.Log("막지 못 함");
            }
        }
    }

    public void OnBlockSuccess()
    {
        GameObject effectObject = Instantiate(_defenceEffectPrefab);
        if (effectObject.TryGetComponent<ParticleSystem>(out ParticleSystem particleSystem))
        {
            //Debug.Log("Guard Effect Start");
            effectObject.SetActive(true);
            effectObject.transform.position = transform.position;
            particleSystem.Play();
        }
        else
        {
            Destroy(effectObject);
        }
    }
}
