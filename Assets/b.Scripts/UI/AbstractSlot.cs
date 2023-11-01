using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI
{
    public abstract class Slot<T> : MonoBehaviour where T : struct
    {
        [SerializeField] protected UIManager _uiManager;
        public T SlotData { get; private set; }
        public virtual void SetSlotData(T data)
        {
            SlotData = data;
        }

        public virtual void Off()
        {
            gameObject.SetActive(false);
        }
        protected virtual void Awake()
        {
            _uiManager = UIManager.Instance;
        }

        public virtual void On()
        {
            gameObject.SetActive(true);
        }
    }
}