using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class ClickableObject : MonoBehaviour
    {
        private bool _isDoubleClickMode = false; // Single Click or Double Click
        private float _lastClickTime;
        protected float _doubleClickInterval = 0.185f;
        public Button @Button;

        /// <summary>
        /// ��ư ã��, ��ư�� �޼��� ����, _lastClickTime �ʱ�ȭ
        /// </summary>
        protected void Initialize()
        {
            @Button = GetComponent<Button>();
            @Button.onClick.AddListener(OnDoubleClicked);
            @Button.onClick.AddListener(OnSingleClicked);
            _lastClickTime = Time.time;
        }

        protected void OnDoubleClicked()
        {
            if (!_isDoubleClickMode)
                return;

            if (Time.time - _lastClickTime < _doubleClickInterval)
            {
                OnClicked();
            }
            else
            {
                _lastClickTime = Time.time;
            }
        }

        protected void OnSingleClicked()
        {
            if (_isDoubleClickMode)
                return;

            OnClicked();
        }
        
        protected virtual void OnClicked()
        {
            ;
        }
    }
}