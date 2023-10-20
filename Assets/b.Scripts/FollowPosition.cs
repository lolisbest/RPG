using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    // 각 값들이 true면 _target의 각 좌표 축의 값을 그대로 복사하여 사용
    [SerializeField] private bool _copyX, _copyY, _copyZ;

    [SerializeField] private Transform _target;

    void Update()
    {
        if (!_target) return;

        transform.position = new Vector3(
            (_copyX ? _target.position.x : transform.position.x),
            (_copyY ? _target.position.y : transform.position.y),
            (_copyZ ? _target.position.z : transform.position.z));
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
}
