using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NpcId = System.Int32;
using System.IO;
using System.Linq;
using RPG.UI;

namespace RPG.Common
{
    public class Npc : InteractableObject
    {
        public int Id;
        public InGameNpcUI NameUI;

        public StructNpcData Data;

        public InGameUIManager @UIManager;
        public override void Interact()
        {
            if(@UIManager.TryOpenNpcServiceSelectionWindow(this))
            {
                IsUsing = true;
            }
        }

        public override void StopInteraction()
        {
            base.StopInteraction();
            if(@UIManager.IsOpenNpcServiceSelectionWindow) // Npc 기능 선택 창이 열려 있다면
            {
                @UIManager.CloseNpcServiceSelectionWindow();
            }
            else if(@UIManager.IsOpenNpcQuestDetailWindow) // 퀘스트 창이 열려 있다면
            {
                @UIManager.CloseNpcQuestDetailWindow();
            }
            else if (@UIManager.IsOpenQuestSelectionWindow) // 퀘스트 선택창이 열려 있다면
            {
                @UIManager.CloseQuestSelectionWindow();
            }
            else if (@UIManager.IsOpenDialogWindow) // 대화 창이 열려 있다면
            {
                @UIManager.CloseDialogWindow();
            }

        }

        // Start is called before the first frame update
        void Start()
        {
            gameObject.layer = LayerIndex;
            Data = DataBase.Npcs[Id];
            //Debug.Log($"{name} NameUI : " + NameUI);
            NameUI.SetName(Data.Name);
            @UIManager = InGameUIManager.Instance;
        }


        // Update is called once per frame
        void Update()
        {
            
        }
    }
}