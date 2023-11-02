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

        [SerializeField] private GameObject _inGameUIRoot;

        //[SerializeField] private GameObject _introSceneUIRoot;
        //[SerializeField] private GameObject _inGameSceneUIRoot;

        public bool ShouldUnlockMouse
        {
            get
            {
                return IsOpenNpcServiceSelectionWindow || IsOpenQuestSelectionWindow || IsOpenNpcQuestDetailWindow ||
                    IsOpenCurrentQuestDetailWindow || IsOpenInventoryWindow || IsOpenInventoryItemInfoWindow ||
                    IsOpenItemBoxWindow || IsOpenSkillsWindow || IsOpenEscWindow || IsOpenOnDeathWindow ||
                    IsOpenMainSelectionWindow || IsOpenSavedGamesWindow || IsOpenPlayerCreationWindow ||
                    IsOpenCurrentQuestsWindow || IsOpenDialogWindow || IsOpenStatusWindow || IsOpenShopWindow ||
                    IsOpenTransactionWindow;
            }
        }

        public override void Initialize()
        {
            ;
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

        private void FixedUpdate()
        {
            //Debug.Log("IsOpenMainSelectionWindow " + IsOpenMainSelectionWindow);
            //Debug.Log("IsOpenSavedGamesWindow " + IsOpenSavedGamesWindow);
            //Debug.Log("IsOpenPlayerCreationWindow " + IsOpenPlayerCreationWindow);

            if (ShouldUnlockMouse) 
            {
                //Debug.Log("MouseUnlock");
                GameManager.Instance.MouseUnlock();
            }
            else
            {
                //Debug.Log("MouseLockOn");
                GameManager.Instance.MouseLockOn();
            }
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
            OpenMainSelectionWindow();
            CloseInGameWindows();
        }

        public void SwitchToInGame()
        {
            Debug.Log("SwitchToInGame");
            CloseIntroWindows();
            OpenInGameWindow();
        }
    }
}