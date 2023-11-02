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
        [Header("------- 인트로 씬 -------")]
        [SerializeField] private GameObject _mainSelectionWindow;
        public bool IsOpenMainSelectionWindow { get => _mainSelectionWindow.gameObject.activeSelf; }

        [SerializeField] private SavedGamesWindow _savedGamesWindow;
        public bool IsOpenSavedGamesWindow { get => _savedGamesWindow.gameObject.activeSelf; }

        [SerializeField] private PlayerCreationWindow _playerCreationWindow;
        public bool IsOpenPlayerCreationWindow { get => _playerCreationWindow.gameObject.activeSelf; }

        public void OpenMainSelectionWindow()
        {
            ReturnToMain();
        }

        public void OpenSavedGamedsWindow()
        {
            string url = "http://localhost:9999/request/playerData";
            GameManager.Instance.TryHttpGet(url, _savedGamesWindow.LoadDataIntoSlots);
            _mainSelectionWindow.gameObject.SetActive(false);
            _savedGamesWindow.Open();
            _playerCreationWindow.Close();
        }

        public void OpenPlayerCreationWindow()
        {
            _mainSelectionWindow.gameObject.SetActive(false);
            _savedGamesWindow.Close();
            _playerCreationWindow.Open();
        }

        public void ReturnToMain()
        {
            _mainSelectionWindow.gameObject.SetActive(true);
            _savedGamesWindow.Close();
            _playerCreationWindow.Close();
        }

        public void CloseIntroWindows()
        {
            _mainSelectionWindow.gameObject.SetActive(false);
            _savedGamesWindow.Close();
            _playerCreationWindow.Close();
        }
    }
}