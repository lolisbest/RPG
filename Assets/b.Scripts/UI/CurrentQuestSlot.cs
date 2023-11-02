using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RPG.Common;

namespace RPG.UI
{
    public class CurrentQuestSlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _questTitleText;
        public int QuestId;
        [SerializeField] private UIManager _uiManager;

        void Awake()
        {
            Clear();
            _uiManager = UIManager.Instance;
            Unhighlight();
        }

        public void SetInfo(int questId, string questTitle)
        {
            Debug.Log($"{name} SetInfo #{questId}.{questTitle}");
            _questTitleText.text = questTitle;
            QuestId = questId;
            //gameObject.SetActive(true);
        }

        public void Clear()
        {
            //Debug.Log($"{name} Init Text");
            _questTitleText.text = string.Empty;
            QuestId = -1;
            Off();
        }

        public void OpenQuestDetailWindow()
        {
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

        public void Highlight()
        {
            _questTitleText.color = Color.yellow;
        }

        public void Unhighlight()
        {
            _questTitleText.color = Color.gray;
        }
    }
}
