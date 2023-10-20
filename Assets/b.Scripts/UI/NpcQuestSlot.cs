using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RPG.Common;

namespace RPG.UI
{
    public class NpcQuestSlot : ClickableObject
    {
        public TextMeshProUGUI Text;
        public int QuestId;

        void Awake()
        {
            base.Initialize();
            ClearOff();
            
        }

        public void SetInfo(int questId, string questTitle)
        {
            //Debug.Log($"{name} SetInfo #{questId}.{questTitle}");
            Text.text = questTitle;
            QuestId = questId;
            //gameObject.SetActive(true);
        }

        public void ClearOff()
        {
            //Debug.Log($"{name} Init Text");
            Text.text = string.Empty;
            Off();
            //gameObject.SetActive(false);
        }

        protected override void OnClicked()
        {
            //InitializeText();
            //gameObject.SetActive(false);
            InGameUIManager.Instance.OpenNpcQuestDetailWindow(QuestId);

        }

        public void On()
        {
            gameObject.SetActive(true);
        }

        public void Off()
        {
            gameObject.SetActive(false);
        }
    }
}
