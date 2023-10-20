using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;
using UnityEngine.UI;
using RPG.UI;

namespace RPG.Item
{
    [RequireComponent(typeof(Rigidbody))]
    public class FieldItemBox : InteractableObject
    {
        private Rigidbody rb;
        public float _keepingTime = 5f;
        private float _startTime;

        //public List<StructIdCount> Items;

        public InGameUIManager @UIManager;

        public List<StructIdCount> Items { get; private set; }

        public int _defaultNumber = 15;

        public bool IsChangedInventory;

        public override void Interact()
        {
            //Debug.Log($"{name} Interact()");
            if (@UIManager.TryOpenItemBoxWindow(this))
            {
                IsUsing = true;
            }
        }

        public void Initialize()
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;
            Items = new();
            gameObject.layer = InitialLayer;

            _startTime = Time.time;

            @UIManager = InGameUIManager.Instance;

            //Debug.Log($"{name} layer : {gameObject.layer}");
        }

        void Update()
        {
            if(IsUsing)
            {
                _startTime = Time.time;
            }
            else if(Time.time - _startTime > _keepingTime)
            {
                Destroy(gameObject);
            }
            else if(Items.Count == 0)
            {
                Destroy(gameObject, 5f);
            }
        }
    
        public override void StopInteraction()
        {
            base.StopInteraction();
        }

        /// <summary>
        /// 초기 위치를 설정하고 회전시킴
        /// </summary>
        /// <param name="starPosition">생성 위치</param>
        public void Pop(Vector3 starPosition)
        {
            Vector3 forceDir = Vector3.up;
            Vector3 torqDir = Vector3.right + Vector3.forward;
            transform.position = starPosition;
            rb.AddForce(forceDir * 100);
            rb.AddTorque(torqDir * 200);
        }

        public void SetItems(StructIdCount[] items)
        {
            for(int i = 0; i < items.Length; i++)
            {
                Items.Add(items[i]);
            }

            IsChangedInventory = true;
        }

        public void RemoveItem(int itemId)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if(Items[i].Id == itemId)
                {
                    Items.RemoveAt(i);
                    IsChangedInventory = true;
                    return;
                }
            }
        }
    }
}
