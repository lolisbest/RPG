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
        [SerializeField] private SavedGamesWindow _savedGamesWindow;
        [SerializeField] private PlayerCreationWindow _playerCreationWindow;

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
    }
}