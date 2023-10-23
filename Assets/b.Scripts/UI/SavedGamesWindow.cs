using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;
using RPG.UI;
using UnityEngine.UI;
using RPG.Utils;

namespace RPG.UI
{
    public class SavedGamesWindow : SlotsWindow<StructPlayerData>
    {
        [SerializeField] private SavedGameSlot _selectedSlot;

        public void StartGame()
        {
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