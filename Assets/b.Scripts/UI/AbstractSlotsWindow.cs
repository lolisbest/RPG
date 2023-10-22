using RPG.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI
{
    public abstract class SlotsWindow<T> : MonoBehaviour where T : struct
    {
        [SerializeField] private List<Slot<T>> _slots;
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private Transform _slotsRoot;

        [SerializeField] private RectTransform _slotRateRef;


        public RectTransform SlotRateReference { get => _slotRateRef ? _slotRateRef : null; }

        public virtual void Open()
        {
            gameObject.SetActive(true);
            if (_slots == null)
                _slots = new();
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }

        public virtual void LoadDataIntoSlots(T[] dataArray)
        {
            //Debug.Log("LoadDataIntoSlots");
            if (dataArray.Length > _slots.Count)
            {
                int requiredNumber = dataArray.Length - _slots.Count;
                CreateSavedGameSlots(requiredNumber);
                //Debug.Log("requiredNumber " + requiredNumber);
            }

            OffSlots();

            for (int i = 0; i < dataArray.Length; i++)
            {
                _slots[i].SetSlotData(dataArray[i]);
                _slots[i].On();
            }
        }

        public virtual void LoadDataIntoSlots(string dataString)
        {
            //Debug.Log("dataString " + dataString);
            T[] playerDataArray = JsonHelper.FromJson<T>(dataString);
            LoadDataIntoSlots(playerDataArray);
        }


        protected virtual void CreateSavedGameSlots(int addNumber)
        {
            for (int i = 0; i < addNumber; i++)
            {
                Slot<T> slot = CreateSavedGameSlot();
                _slots.Add(slot);
            }
        }

        protected virtual Slot<T> CreateSavedGameSlot()
        {
            GameObject slotObject = Instantiate(_slotPrefab);
            slotObject.name = $"Slot[{_slots.Count+1}]";
            Slot<T> slot = slotObject.GetComponent<Slot<T>>();
            AspectRatioKeeper ratioKeeper = slotObject.GetComponent<AspectRatioKeeper>();
            if (ratioKeeper && SlotRateReference)
            {
                ratioKeeper.SetOther(SlotRateReference);
            }
            slotObject.transform.SetParent(_slotsRoot);
            return slot;
        }

        protected virtual void OffSlots()
        {
            foreach (var slot in _slots)
            {
                slot.Off();
            }
        }
    }
}