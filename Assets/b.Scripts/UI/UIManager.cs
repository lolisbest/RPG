using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.UI;
using RPG.Common;

namespace RPG.UI
{
    public partial class UIManager : Singleton<UIManager>
    {
        [Space(10f)]
        [Header("------- 일반 -------")]
        [Header("통신 중일 때 진행 표시")]
        [SerializeField] private GameObject _progressIndicator;
        [Header("서버와의 통신 에러 창")]
        [SerializeField] private ServerErrorWindow _serverErrorWindow;

        [SerializeField] private GameObject _introSceneUIRoot;
        [SerializeField] private GameObject _inGameSceneUIRoot;

        public override void Initialize()
        {
            //LastInteractionType = InteractionType.Open;
            //if (InteractionKeyMessageTxt != null)
            //    InteractionKeyMessageTxt.text = string.Format(KeyPressMessage, LastInteractionType.ToString());

            //@NpcServiceSelectionWindow.Initialize();
            //@QuestSelectionWindow.Initialize();
            //@QuestManager = QuestManager.Instance;
            //@NpcQuestDetailWindow.Initialize();
            //@DamageTextDrawer.Initialize();

            //@InventoryWindow.Initialize();
            //@CurrentQuestsWindow.Initialize();
            //@DialogWindow.Initialize();
            //@CurrentQuestDetailWindow.Initialize();
            //@ShopWindow.Initialize();

            return;
            //DroppedBox.Initialize();
            //@Player = Player.Instance;
        }


        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        void Start()
        {
            GameManager.Instance.SetUIManager(this);
            _mainSelectionWindow.gameObject.SetActive(true);
        }

        void Update()
        {
            if (displayFps)
                UpdateFps();
        }

        public void ToggleProgressIndicator(bool active)
        {
            if (!_progressIndicator) return;

            _progressIndicator.SetActive(active);
        }

        public void OpenServerCommunicationErrorWindow(string message)
        {
            _serverErrorWindow.Open(message);
        }

        public void CloseServerCommunicationErrorWindow()
        {
            _serverErrorWindow.Close();
        }

        public void Test()
        {
            Debug.Log("IntroSceneUIManager.Test");
        }

        public void SwitchToIntro()
        {
            ReturnToMain();
            _introSceneUIRoot.SetActive(true);
            _inGameSceneUIRoot.SetActive(false);
        }

        public void SwitchToInGame()
        {
            Debug.Log("SwitchToInGame");
            _introSceneUIRoot.SetActive(false);
            _inGameSceneUIRoot.SetActive(true);
        }
    }
}