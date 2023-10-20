using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;
using UnityEngine.Networking;
using System;
using TMPro;

namespace RPG.UI
{
    public class ServerErrorWindow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _messageContentText;

        public void Open(string message = "")
        {
            _messageContentText.text = message;
            Open();
        }

        private void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
