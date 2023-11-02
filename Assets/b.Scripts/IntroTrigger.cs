using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.UI;

public class IntroTrigger : MonoBehaviour
{
    void Start()
    {
        UIManager.Instance.SwitchToIntro();
    }
}
