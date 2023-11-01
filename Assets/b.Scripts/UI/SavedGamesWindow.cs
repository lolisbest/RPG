using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;
using RPG.UI;
using UnityEngine.UI;

namespace RPG.UI
{
    public class SavedGamesWindow : SlotsWindow<StructPlayerData>
    {
        [SerializeField] private SavedGameSlot _selectedSlot;
        [SerializeField] UIManager _uiManager;

        public void StartGame()
        {
            Debug.Log("_uiManager.ReturnToMain()");
            //_uiManager.ReturnToMain();
            GameManager.Instance.SetCurrentPlayerData(_selectedSlot.SlotData);
            GameManager.Instance.LoadInGameScene();
            return;
        }

        public void DeletePlayerData()
        {
            GameManager.Instance.DeletePlayerData(_selectedSlot.PlayerDataId);
            _selectedSlot = null;
        }

        public void Select(SavedGameSlot newSeletedSlot)
        {
            if(_selectedSlot) _selectedSlot.HighLightOff();

            _selectedSlot = newSeletedSlot;

            if (_selectedSlot) _selectedSlot.HighLightOn();
        }

        public override void Open()
        {
            base.Open();
            _selectedSlot = null;
        }
    }
}