using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwakeTest : MonoBehaviour
{
    void Awake()
    {
        Debug.Log($"{name}.Awake()");
    }

    private void OnEnable()
    {
        Debug.Log($"{name}.OnEnable()");
    }

    private void OnDisable()
    {
        Debug.Log($"{name}.OnDisable()");
    }
}
