using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;
using RPG.UI;
using UnityEngine.UI;
using TMPro;

namespace RPG.UI
{
    public class SavedGameSlot : Slot<StructPlayerData>
    {
        [SerializeField] private SavedGamesWindow _savedGamesWindow;

        [SerializeField] private TextMeshProUGUI _playerNameText;
        [SerializeField] private TextMeshProUGUI _playerLevelText;

        private readonly string BaseNameString = "Name : {0}";
        private readonly string BaseLevelString = "Level : {0}";

        public int PlayerDataId { get; private set; }

        [SerializeField] private GameObject _highlightObject;

        public override void SetSlotData(StructPlayerData playerData)
        {
            Debug.Log("SavedGameSlot " + playerData);
            base.SetSlotData(playerData);
            PlayerDataId = playerData.DataId;
            _playerNameText.text = string.Format(BaseNameString, playerData.Status.Name);
            _playerLevelText.text = string.Format(BaseLevelString, playerData.Status.Level);
        }

        public void OnClicked()
        {
            Debug.Log("Saved CharacterSlot OnClicked");
            _savedGamesWindow.Select(this);
        }

        public void HighLightOn()
        {
            _highlightObject.SetActive(true);
        }

        public void HighLightOff()
        {
            _highlightObject.SetActive(false);
        }

        public override void Clear()
        {
            PlayerDataId = -1;
            _playerNameText.text = string.Empty;
            _playerLevelText.text = string.Empty;
            HighLightOff();
        }
    }
}