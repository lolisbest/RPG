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

    [SerializeField] private Collider _collider;

    /// <summary>
    /// 1) Player.AddAttackHit
    /// </summary>
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

    //private void OnCollisionEnter(Collision collision)
    //{
    //    TryDefence(collision);
    //}

    //private void OnCollisionStay(Collision collision)
    //{
    //    TryDefence(collision);
    //}

    private void TryDefence(Collider otherCollider)
    {
        AttackCollider attackCollider = otherCollider.GetComponent<AttackCollider>();
        if (attackCollider != null)
        {
            if (attackCollider.AttackerType == EnumAttackerType.Player)
                return;

            float dot = Vector3.Dot(attackCollider.transform.up, transform.forward);
            //Debug.Log("dot " + dot);

            if (dot < 0f)
            {
                //Debug.Log("Denfence Ok");
                Vector3 closestPoint = otherCollider.ClosestPointOnBounds(transform.position);
                //hitPosition = closestPoint;

                OnCollision?.Invoke(
                    new StructAttackHit
                    {
                        AttackCollider = attackCollider,
                        AttackScriptId = attackCollider.GetHashCode(),
                        IsBlocked = true,
                        IsApplied = false,
                        RawDamage = attackCollider.Damage,
                        HitPosition = closestPoint
                    });
            }
            else
            {
                //Debug.Log("막지 못 함");
            }
        }
    }

    private void TryDefence(Collision collision)
    {
        AttackCollider attackCollider = collision.gameObject.GetComponent<AttackCollider>();
        if (attackCollider != null)
        {
            if (attackCollider.AttackerType == EnumAttackerType.Player)
                return;

            float dot = Vector3.Dot(attackCollider.transform.up, transform.forward);
            Debug.Log("dot " + dot);

            if (dot < 0f)
            {
                Vector3 sum = Vector3.zero;

                foreach(var contact in collision.contacts)
                {
                    sum += contact.point;
                }

                Vector3 averagePosition = sum / collision.contacts.Length;

                OnCollision?.Invoke(
                    new StructAttackHit
                    {
                        AttackCollider = attackCollider,
                        AttackScriptId = attackCollider.GetHashCode(),
                        IsBlocked = true,
                        IsApplied = false,
                        RawDamage = attackCollider.Damage,
                        HitPosition = averagePosition
                    });
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

    public void OnBlockSuccess(Vector3 hitPosition)
    {
        GameObject effectObject = Instantiate(_defenceEffectPrefab);
        if (effectObject.TryGetComponent<ParticleSystem>(out ParticleSystem particleSystem))
        {
            effectObject.name = $"GuardEffect[{Time.time}]";
            Debug.Log("Guard Effect Start " + effectObject.name);
            effectObject.SetActive(true);
            effectObject.transform.position = hitPosition;
            particleSystem.Play();
        }
        else
        {
            Destroy(effectObject);
        }
    }
}
