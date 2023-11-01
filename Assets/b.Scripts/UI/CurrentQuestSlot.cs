using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RPG.Common;

namespace RPG.UI
{
    public class CurrentQuestSlot : ClickableObject
    {
        public TextMeshProUGUI Text;
        public int QuestId;
        [SerializeField] private UIManager _uiManager;

        void Awake()
        {
            base.Initialize();
            Clear();
            _uiManager = UIManager.Instance;
        }

        public void SetInfo(int questId, string questTitle)
        {
            Debug.Log($"{name} SetInfo #{questId}.{questTitle}");
            Text.text = questTitle;
            QuestId = questId;
            //gameObject.SetActive(true);
        }

        public void Clear()
        {
            //Debug.Log($"{name} Init Text");
            Text.text = string.Empty;
            QuestId = -1;
            Off();
        }

        protected override void OnClicked()
        {
            //InitializeText();
            //gameObject.SetActive(false);
            _uiManager.OpenCurrentQuestDetailWindow(QuestId);

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
