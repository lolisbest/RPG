using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C : ITestVirtual
{
    public void A()
    {
        Debug.Log("C.A");
    }

    public void B()
    {
        Debug.Log("C.B");
    }
}

public class ITest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        C c = new();
        c.A();
        c.B();

        (c as ITestVirtual).A();
        (c as ITestVirtual).B();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
