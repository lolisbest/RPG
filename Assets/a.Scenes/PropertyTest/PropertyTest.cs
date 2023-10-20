using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public struct TestStruct
{
    public int Level;

    public void Increase()
    {
        Level += 1;
        Debug.Log("Increase " + Level);
    }

    public override string ToString()
    {
        return $"Level : {Level}";
    }
}

public class PropertyTest : MonoBehaviour
{
    private TestStruct _test;
    public TestStruct Test { get => _test; private set { _test = value; } }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Test);
        Test.Increase();
        Debug.Log(Test);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
