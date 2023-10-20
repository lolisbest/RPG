using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabLoad : MonoBehaviour
{
    public GameObject Prefab;
    public GameObject New;

    void Start()
    {
        New = Instantiate(Prefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
