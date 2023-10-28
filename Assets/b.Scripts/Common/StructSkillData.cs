using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace RPG.Common
{
    public enum EnumSkillType
    {
        None,
        Action,
        Point,
        Projectile,
    }

    [Serializable]
    public struct StructSkillData
    {
        public int Id;
        public string Name;
        public string IconPath;
        public Sprite Icon;
        public string Description;
        public int MpCost;
        
        public string[] WeaponTypeStrings;
        public EnumWeaponType WeaponType;

        public string TypeString;
        public EnumSkillType Type;

        public float CoolTime;
        public string PrefabPath;
        public GameObject Prefab;

        public override string ToString()
        {
            return $"Name : {Name}\n" +
                $"Id : {Id}\n" +
                $"IconPath : {IconPath}\n" +
                $"Icon : {Icon}\n" +
                $"Description : {Description}\n" +
                $"MpCost : {MpCost}\n" +
                $"WeaponTypeStrings : {string.Join("|", WeaponTypeStrings)}\n" +
                $"WeaponType : {WeaponType}\n" +
                $"CoolTime : {CoolTime}\n";
        }
    }

}