using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Common
{
    public interface IInteractable
    {
        public bool IsUsing { get; }

        public void Interact();

        public void StopInteraction();
    }


}