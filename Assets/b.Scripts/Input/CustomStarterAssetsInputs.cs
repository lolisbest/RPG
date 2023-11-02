using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

using RPG.Common;

namespace RPG.Input
{
	public class CustomStarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;

		public bool jump;

		public bool sprint;

		// attack
		public bool attack;

		public bool block;

		public bool interact;

		public bool npcTalk;
		public bool npcTalkNext;

		public bool quest;
		public bool npcQuestAccept;
		public bool npcRest;
		public bool npcShop;

		public bool lootAll;
		public bool inventory;
		public bool skill;

		public bool quit;

		public bool slot1;
		public bool slot2;
		public bool slot3;
		public bool slot4;
		public bool slot5;
		public bool slot6;
		public bool slot7;
		public bool slot8;
		public bool slot9;
		public bool slot0;

		public bool esc;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if (cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

        public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnAttack(InputValue value)
        {
			attack = value.isPressed;
        }

		public void OnInteract(InputValue value)
        {
			interact = value.isPressed;
        }

		public void OnNpcTalk(InputValue value)
        {
			npcTalk = value.isPressed;
		}

		public void OnNpcTalkNext(InputValue value)
		{
			npcTalkNext = value.isPressed;
		}

		public void OnQuest(InputValue value)
		{
			quest = value.isPressed;
		}

		public void OnNpcQuestAccept(InputValue value)
		{
			npcQuestAccept = value.isPressed;
		}

		public void OnNpcShop(InputValue value)
		{
			npcShop = value.isPressed;
		}

		public void OnNpcRest(InputValue value)
		{
			npcRest = value.isPressed;
		}

		public void OnBlock(InputValue value)
        {

			if(value.Get<float>() > 0f)
            {
				block = true;
            }
			else
            {
				block = false;
            }
			
        }

		public void OnLootAll(InputValue value)
		{
			lootAll = value.isPressed;
		}

		public void OnInventory(InputValue value)
		{
			//Debug.Log("OnInventory");
			inventory = value.isPressed;
		}

		public void OnSkill(InputValue value)
		{
			//Debug.Log("OnInventory");
			skill = value.isPressed;
		}

		public void OnQuit(InputValue value)
		{
			quit = value.isPressed;
		}

		public void OnSlot1(InputValue value)
		{
			slot1 = value.isPressed;
		}

		public void OnSlot2(InputValue value)
		{
			slot2 = value.isPressed;
		}

		public void OnSlot3(InputValue value)
		{
			slot3 = value.isPressed;
		}

		public void OnSlot4(InputValue value)
		{
			slot4 = value.isPressed;
		}

		public void OnSlot5(InputValue value)
		{
			slot5 = value.isPressed;
		}

		public void OnSlot6(InputValue value)
		{
			slot6 = value.isPressed;
		}

		public void OnSlot7(InputValue value)
		{
			slot7 = value.isPressed;
		}

		public void OnSlot8(InputValue value)
		{
			slot8 = value.isPressed;
		}

		public void OnSlot9(InputValue value)
		{
			slot9 = value.isPressed;
		}

		public void OnSlot0(InputValue value)
		{
			slot0 = value.isPressed;
		}

		public void OnEsc(InputValue value)
		{
			esc = value.isPressed;
		}
#endif

		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

		/// <summary>
		/// 오브젝트와 상호 작용 중일 때 사용 못 하게 하기 위해. 공격, 회전, 또 다른 상호작용, 인벤토리 창, 스킬 창, 퀵슬롯, Esc 키 입력 버림
		/// </summary>
		public void ClearInputsOnIntraction()
        {
            // 오브젝트와 상호 작용 중일 때 사용 못 하게 하기 위해
            attack = false;
            look = Vector2.zero;
            interact = false;
            inventory = false;
			skill = false;

            slot1 = false;
            slot2 = false;
            slot3 = false;
            slot4 = false;
            slot5 = false;
            slot6 = false;
            slot7 = false;
            slot8 = false;
            slot9 = false;
            slot0 = false;

            esc = false;
        }

        /// <summary>
        /// sprint, block 은 제외
        /// </summary>
        public void ClearBoolInputs()
        {
			// 트리거 형식으로 쓰기 위해 리셋
			jump = false;
			attack = false;

			interact = false;

			npcTalk = false;
			npcTalkNext = false;
			quest = false;
			npcQuestAccept = false;
			npcRest = false;
			npcShop = false;

			lootAll = false;
			inventory = false;
			skill = false;

			quit = false;

			slot1 = false;
			slot2 = false;
			slot3 = false;
			slot4 = false;
			slot5 = false;
			slot6 = false;
			slot7 = false;
			slot8 = false;
			slot9 = false;
			slot0 = false;

			esc = false;
		}

		public StructInput GetInputs()
		{
			StructInput input = new();
			input.Move = move;
			input.Look = look;
			input.Sprint = sprint;
			input.Attack = attack;
			input.Block = block;
			input.Interact = interact;
			input.NpcTalk = npcTalk;
			input.NpcTalkNext = npcTalkNext;
			input.Quest = quest;
			input.NpcQuestAccept = npcQuestAccept;
			input.NpcRest = npcRest;
			input.NpcShop = npcShop;
			input.LootAll = lootAll;
			input.Inventory = inventory;
			input.Skill = skill;
			input.Quit = quit;
			input.Slot1 = slot1;
			input.Slot2 = slot2;
			input.Slot3 = slot3;
			input.Slot4 = slot4;
			input.Slot5 = slot5;
			input.Slot6 = slot6;
			input.Slot7 = slot7;
			input.Slot8 = slot8;
			input.Slot9 = slot9;
			input.Slot0 = slot0;
			input.Esc = esc;

			return input;
		}
	}
}