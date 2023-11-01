using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RPG.UI
{
    public class DamageText : MonoBehaviour
    {
        public DamageTextDrawer _drawer;

        public TextMeshProUGUI Text;
        public RectTransform TextRect;

        [Header("폰트 기본 크기")]
        public float DamageTextFontSize = 36f;

        [Header("위로 떠오르는 속도")]
        public float UpSpeed = 35f;

        [Header("커지는 속도")]
        [Range(-2f, 2f)]
        public float ScaleSpeed;

        [Header("지속 시간")]
        public float LastingTime = 1f;

        private float _leftTime = 0f;

        public bool IsActive { get => gameObject.activeSelf; }

        // Update is called once per frame
        void Update()
        {
            //return;

            if (_leftTime > 0f)
            {
                _leftTime -= Time.deltaTime;
                TextRect.localScale += Vector3.one * Time.deltaTime * ScaleSpeed;
                Vector2 position = TextRect.anchoredPosition;
                position.y += Time.deltaTime * UpSpeed;
                TextRect.anchoredPosition = position;
            }
            else
            {
                Deactivate();
            }
        }

        public void Show(int damage, Vector2 position)
        {
            gameObject.SetActive(true);
            Text.text = damage.ToString();

            TextRect.anchoredPosition = position;
            _leftTime = LastingTime;
            TextRect.localScale = Vector3.one;
        }

        public void Init(DamageTextDrawer damageTextDrawer)
        {
            _drawer = damageTextDrawer;
            TextRect.anchorMin = Vector2.zero;
            TextRect.anchorMax = Vector2.zero;
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            _drawer.RemoveFromPresents(this);
            _drawer.JointIdleQue(this);
        }

        public void AddTextSize(float size)
        {
            Text.fontSize += size;
        }
    }
}