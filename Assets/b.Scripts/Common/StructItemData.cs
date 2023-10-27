using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPG.Common
{
    [Serializable]
    public struct StructItemData
    {
        public string Name;
        public string TypeString;
        public string IconPath;
        public int Id;
        public Sprite @Sprite;
        public EnumItemType ItemType;
        
        public string Description;

        public int Attack;
        public int Defence;
        public int MaxHp;
        public int MaxMp;

        public int Str;
        public int End;
        public int Sta;
        public int Mag;

        public int RecoveryHpAmount;
        public int RecoveryMpAmount;

        public int SkillId;

        public string EquipPartTypeString;
        public EnumEquipType EquipType;
        public EnumWeaponType WeaponType;

        public int Grade;
        public Color GradeColor;

        public int BuyPrice;
        public int SellPrice;

        public override string ToString()
        {
            return $"------- {Name} -------\n" +
                $"TypeString:{TypeString}\n" +
                $"IconPath:{IconPath}\n" +
                $"Id:{Id}\n" +
                $"Sprite:{@Sprite}\n" +
                $"ItemType:{ItemType}\n" +
                $"EquipPartTypeString:{EquipPartTypeString}\n" +
                $"EquipType:{EquipType}\n" +
                $"-------------------";
        }

        public StructItemData LoadSprite()
        {
            if (@Sprite == null)
            {
                @Sprite = Resources.Load<Sprite>(IconPath);
            }

            return this;
        }

        public StructItemData SetType()
        {
            ItemType = Utils.StringToEnum<EnumItemType>(TypeString);
            if(EquipPartTypeString == null)
            {
                EquipType = EnumEquipType.None;
            }
            else
            {
                EquipType = Utils.StringToEnum<EnumEquipType>(EquipPartTypeString);
            }

            return this;
        }
    }

    public enum EnumItemType
    {
        None = 0,
        Equipment,
        Consumable,
        Currency,
        Etc,
    }

    public enum EnumEquipType
    {
        None = 0,
        Head,
        Chest,
        Hand0,
        Hand1,
        Foot,
    }
}