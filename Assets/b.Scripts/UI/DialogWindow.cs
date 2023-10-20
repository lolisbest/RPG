using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

namespace RPG.UI
{
    public class DialogWindow : MonoBehaviour
    {
        public TextMeshProUGUI NpcNameText;
        public TextMeshProUGUI DialogText;

        public Button NextButton;
        public Button QuitButton;
        
        public int CurrentDialogId;
        public int NextDialogId;

        private float _characterInterval = 0.015f;
        private WaitForSeconds Delay;

        public void Initialize()
        {
            Delay = new WaitForSeconds(_characterInterval);
            QuitButton.gameObject.SetActive(false);
            NextButton.gameObject.SetActive(false);
        }
        
        public void Open(string npcName, int dialogId)
        {
            CurrentDialogId = dialogId;
            NextDialogId = DataBase.Dialogs[CurrentDialogId].NextId;

            NpcNameText.text = npcName;
            gameObject.SetActive(true);
            StartCoroutine(DisplayCharacterByCharacter(CurrentDialogId));
        }

        private IEnumerator DisplayCharacterByCharacter(int toDisplayDialogId)
        {
            QuitButton.gameObject.SetActive(false);
            NextButton.gameObject.SetActive(false);

            StringBuilder stringBuilder = new();
            string currentDialog = DataBase.Dialogs[toDisplayDialogId].Text;

            foreach(var character in currentDialog)
            {
                yield return Delay;
                stringBuilder.Append(character);
                UpdateDialog(stringBuilder.ToString());
            }

            yield return Delay;
            if(NextDialogId == -1)
            {
                QuitButton.gameObject.SetActive(true);
            }
            else
            {
                NextButton.gameObject.SetActive(true);
            }
        }

        private void UpdateDialog(string dialog)
        {
            DialogText.text = dialog;
        }

        public void Next()
        {
            CurrentDialogId = NextDialogId;
            NextDialogId = DataBase.Dialogs[CurrentDialogId].NextId;
            DialogText.text = string.Empty;
            Debug.Log($"Next(). CurrentDialogId:{CurrentDialogId}, NextDialogId:{NextDialogId}");
            StartCoroutine(DisplayCharacterByCharacter(CurrentDialogId));
        }

        public void Quit()
        {
            CurrentDialogId = -1;
            NextDialogId = -1;
            gameObject.SetActive(false);
            DialogText.text = string.Empty;
        }
    }
}