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
        /// 버튼 찾기, 버튼에 메서드 연결, _lastClickTime 초기화
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