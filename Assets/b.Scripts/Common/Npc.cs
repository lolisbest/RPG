using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Common
{
    public class Npc : InteractableObject
    {
        public int Id;
        public InGameNpcUI NameUI;

        public StructNpcData Data;

        private RPG.UI.UIManager _uiManager;
        public override void Interact()
        {
            if(_uiManager.TryOpenNpcServiceSelectionWindow(this))
            {
                IsUsing = true;
            }
        }

        public override void StopInteraction()
        {
            base.StopInteraction();
            if(_uiManager.IsOpenNpcServiceSelectionWindow) // Npc 기능 선택 창이 열려 있다면
            {
                _uiManager.CloseNpcServiceSelectionWindow();
            }
            else if(_uiManager.IsOpenNpcQuestDetailWindow) // 퀘스트 창이 열려 있다면
            {
                _uiManager.CloseNpcQuestDetailWindow();
            }
            else if (_uiManager.IsOpenQuestSelectionWindow) // 퀘스트 선택창이 열려 있다면
            {
                _uiManager.CloseQuestSelectionWindow();
            }
            else if (_uiManager.IsOpenDialogWindow) // 대화 창이 열려 있다면
            {
                _uiManager.CloseDialogWindow();
            }

        }

        // Start is called before the first frame update
        void Start()
        {
            gameObject.layer = LayerIndex;
            Data = DataBase.Npcs[Id];
            //Debug.Log($"{name} NameUI : " + NameUI);
            NameUI.SetName(Data.Name);
            _uiManager = RPG.UI.UIManager.Instance;
        }


        // Update is called once per frame
        void Update()
        {
            
        }
    }
}