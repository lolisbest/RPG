using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableTest : MonoBehaviour
{
    public MapData mapData;

    void Start()
    {
        Debug.Log("mapData : " + mapData.ToString());
    }
}
