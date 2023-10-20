using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RPG.Monster
{
    public class InGameMonsterUI : MonoBehaviour
    {
        public Image HpBar;
        public TextMeshProUGUI NameText;

        public void InitHp()
        {
            HpBar.fillAmount = 1f;
        }

        public void UpadteHp(float rate)
        {
            HpBar.fillAmount = rate;
        }

        void Update()
        {
            Vector3 targetPostition = new Vector3(Camera.main.transform.position.x,
                                                   transform.position.y,
                                                   Camera.main.transform.position.z);
            transform.LookAt(targetPostition);
        }

        public void SetName(string name)
        {
            NameText.text = name;
        }
    }
}