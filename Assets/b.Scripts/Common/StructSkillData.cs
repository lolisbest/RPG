using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace RPG.Common
{
    public enum EnumLocationType
    {
        None,
        FixedOnPlayer,
        Moveable,
        SpecificPoint,
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

        public string LocationTypeString;
        /// <summary>
        /// 스킬의 위치 유형. 1) 캐릭터 주위. 2) 이동형. 3) 특정 지점
        /// </summary>
        public EnumLocationType LocationType;

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