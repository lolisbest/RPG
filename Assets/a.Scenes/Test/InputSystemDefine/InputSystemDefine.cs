using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystemDefine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if ENABLE_INPUT_SYSTEM
        Debug.Log("ENABLE_INPUT_SYSTEM");
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        Debug.Log("ENABLE_LEGACY_INPUT_MANAGER");
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
