using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RPG.Common
{
    public class InGameNpcUI : MonoBehaviour
    {
        public TextMeshProUGUI NameText;

        public void SetName(string name)
        {
            NameText.text = name;
        }

        void Update()
        {
            Vector3 targetPostition = new Vector3(Camera.main.transform.position.x,
                                                   transform.position.y,
                                                   Camera.main.transform.position.z);
            transform.LookAt(targetPostition);
        }
    }
}