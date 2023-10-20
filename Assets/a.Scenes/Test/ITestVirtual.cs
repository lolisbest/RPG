using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITestVirtual
{
    public void A()
    {
        Debug.Log("ITestVirtual.A");
    }

    public virtual void B()
    {
        Debug.Log("ITestVirtual.B");
    }
}
