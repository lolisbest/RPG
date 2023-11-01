using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI
{
    public class InGameTrigger : MonoBehaviour
    {
        void Awake()
        {
            UIManager.Instance.InitializeInGame();
        }
    }
}