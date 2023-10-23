using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RPG.Common;

namespace RPG.UI
{

    public class PlayerCreationWindow : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private IntroSceneUIManager _introUIManager;

        [SerializeField] private GameObject[] _characters;

        private int _currentCharacterIndex;

        void Start()
        {
            _currentCharacterIndex = 0;
            Clear();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OffCharacters()
        {
            foreach(var character in _characters)
            {
                character.SetActive(false);
            }
        }

        private void ShowCharacter(int characterIndex)
        {
            if (characterIndex < 0) return;
            if (characterIndex >= _characters.Length) return;
            
            OffCharacters();

            _characters[characterIndex].SetActive(true);
        }

        public void CreateCharacter()
        {
            StructPlayerData newPlayerData = StructPlayerData.GetTempData();
            newPlayerData.Status.Name = _inputField.text;
            Debug.Log(newPlayerData);
            GameManager.Instance.SetCurrentPlayerData(newPlayerData);
            GameManager.Instance.LoadInGameScene();
        }

        public void Open()
        {
            gameObject.SetActive(true);
            _currentCharacterIndex = 0;
            ShowCharacter(_currentCharacterIndex);
        }

        public void Close()
        {
            gameObject.SetActive(false);
            Clear();
        }

        private void Clear()
        {
            _inputField.text = string.Empty;
        }

        public void ShowNextCharacter()
        {
            _currentCharacterIndex += 1;

            if (_currentCharacterIndex >= _characters.Length)
            {
                _currentCharacterIndex = _characters.Length - 1;
            }

            ShowCharacter(_currentCharacterIndex);
        }

        public void ShowPreviousCharacter()
        {
            _currentCharacterIndex -= 1;

            if (_currentCharacterIndex < 0)
            {
                _currentCharacterIndex = 0;
            }

            ShowCharacter(_currentCharacterIndex);
        }
    }
}