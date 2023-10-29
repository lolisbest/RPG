using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Common
{
    public enum InteractionType
    {
        Open = 0,
        Talk,
    }

    public abstract class InteractableObject : MonoBehaviour, IInteractable
    {
        //public Color OutlineColor;
        
        public static readonly string DetectedLayerName = "DetectedInteractableObject";
        public static int DetectedLayerIndex { get; protected set; }

        public static readonly string LayerName = "InteractableObject";
        public static int LayerIndex { get; protected set; }
        public bool IsUsing { get; protected set; } = false;

        /// <summary>
        /// use for when change layer
        /// </summary>
        [SerializeField] private GameObject _meshObject;
        public abstract void Interact();
        /// <summary>
        /// IsUsing = false;
        /// </summary>
        public virtual void StopInteraction()
        {
            IsUsing = false;
        }

        public InteractionType Type;

        /// <summary>
        /// Outline과 관련된 레이어마스크 값 미리 설정
        /// </summary>
        public static void SetLayerMaskValue()
        {
            Debug.Log("SetLayerMaskValue");
            DetectedLayerIndex = LayerMask.NameToLayer(DetectedLayerName);
            LayerIndex = LayerMask.NameToLayer(LayerName);
        }

        public virtual void ActivateDetectedOutline()
        {
            if (DetectedLayerIndex == 0 || LayerIndex == 0) throw new System.Exception("InteractableObject.SetLayerMaskValue Call??");
            SetLayers(DetectedLayerIndex);
        }

        public virtual void DeactivateOutLine()
        {
            SetLayers(LayerIndex);
        }

        private void SetLayers(int newLayer)
        {
            _meshObject.layer = newLayer;
            int childCount = _meshObject.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = _meshObject.transform.GetChild(i);
                child.gameObject.layer = newLayer;
            }
        }
    }
}