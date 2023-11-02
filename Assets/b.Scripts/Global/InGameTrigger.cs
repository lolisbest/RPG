using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.UI;

public class InGameTrigger : MonoBehaviour
{
    void Start()
    {
        UIManager.Instance.InitializeInGame();
        UIManager.Instance.SwitchToInGame();
    }
}
