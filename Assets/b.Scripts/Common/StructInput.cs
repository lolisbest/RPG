using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Common
{
    public struct StructInput
    {
		public Vector2 Move;
		public Vector2 Look;

		//public bool NoMouseRotation;
		//public bool Jump;

		public bool Sprint;

		// attack
		public bool Attack;

		public bool Block;

		public bool Interact;

		public bool NpcTalk;
		public bool NpcTalkNext;

		public bool Quest;
		public bool NpcQuestAccept;
		public bool NpcRest;
		public bool NpcShop;

		public bool LootAll;
		public bool Inventory;
		public bool Skill;

		public bool Quit;

		public bool Slot1;
		public bool Slot2;
		public bool Slot3;
		public bool Slot4;
		public bool Slot5;
		public bool Slot6;
		public bool Slot7;
		public bool Slot8;
		public bool Slot9;
		public bool Slot0;

		public bool Esc;
	}
}