using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace RPG.Input
{
	public class CustomStarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;

		public bool noMouseRotation;
		public bool jump;

		public bool sprint;

		// attack
		public bool attack;
		// adv attack
		public bool advAttack;

		public bool block;

		public bool interaction;

		public bool npcTalk;
		public bool npcTalkNext;

		public bool npcQuest;
		public bool npcQuestAccept;
		public bool npcRest;
		public bool npcShop;

		public bool lootAll;
		public bool inventory;

		public bool quit;

		public bool slot1;
		public bool slot2;
		public bool slot3;
		public bool slot4;
		public bool slot5;
		public bool slot6;
		public bool slot7;
		public bool slot8;


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


		public void OnNoMouseRotation(InputValue value)
		{
			float i = value.Get<float>();
			if(i > 0)
            {
				noMouseRotation = true;
			}
			else
            {
				noMouseRotation = false;
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

		public void OnAdvancedAttack(InputValue value)
		{
			advAttack = value.isPressed;
		}

		public void OnInteraction(InputValue value)
        {
			interaction = value.isPressed;
        }

		public void OnNpcTalk(InputValue value)
        {
			npcTalk = value.isPressed;
		}

		public void OnNpcTalkNext(InputValue value)
		{
			npcTalkNext = value.isPressed;
		}

		public void OnNpcQuest(InputValue value)
		{
			npcQuest = value.isPressed;
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

		public void ClearInputsOnIntraction()
        {
			// 오브젝트와 상호 작용 중일 때 사용 못 하게 하기 위해
			//move = Vector2.zero;
            look = Vector2.zero;
			attack = false;
			advAttack = false;

			interaction = false;
			//lootAll = false;
			//inventory = false;

			//quit = false;

			slot1 = false;
			slot2 = false;
			slot3 = false;
			slot4 = false;
			slot5 = false;
			slot6 = false;
			slot7 = false;
			slot8 = false;
		}

		/// <summary>
		/// sprint, block 은 제외
		/// </summary>
		public void ClearBoolInputs()
        {
			jump = false;
			attack = false;
			advAttack = false;

			interaction = false;

			npcTalk = false;
			npcTalkNext = false;
			npcQuest = false;
			npcQuestAccept = false;
			npcRest = false;
			npcShop = false;

			lootAll = false;
			inventory = false;

			quit = false;

			slot1 = false;
			slot2 = false;
			slot3 = false;
			slot4 = false;
			slot5 = false;
			slot6 = false;
			slot7 = false;
			slot8 = false;
		}
	}

}