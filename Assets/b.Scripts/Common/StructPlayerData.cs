using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPG.Common
{
    [Serializable]
    public struct StructStatus
    {
        public string Name;

        public int Str;

        public int End;

        public int Sta;

        public int Mag;

        public int Level;

        public int Experience;

        public int LeftStatusPoints;

        public int[] AvailableSkillIds;

        public override string ToString()
        {
            return $"Name : {Name}, Level : {Level}, Str : {Str}, Experience : {Experience}, LeftStatusPoints : {LeftStatusPoints}";
        }

        /// <summary>
        /// 반드시 반환 값을 받고 저장해야 함
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public StructStatus SetExperience(int exp)
        {
            Debug.Log("SetExperience1 newExp " + exp);
            (int newLevel, int newExp, int requiredExp) = DataBase.ExpTable(Level, exp);
            if(newLevel != Level)
            {
                int statusPoints = (newLevel - Level) * DataBase.StatusPointsPerLevelUp;
                LeftStatusPoints += statusPoints;
            }

            Level = newLevel;
            Experience = newExp;

            return this;
        }

        public static StructStatus GetTempData()
        {
            StructStatus status = new();
            status.Name = "TemporaryPlayer";
            status.Str = 10;
            status.End = 10;
            status.Sta = 10;
            status.Mag = 10;

            status.Level = 1;
            status.Experience = 0;
            status.LeftStatusPoints = 0;
            status.AvailableSkillIds = new int[] { 1, 2 };

            Debug.Log($"## GetTempData AvailableSkillIds : {string.Join(", ", status.AvailableSkillIds)}");
            return status;
        }
    }

    [Serializable]
    public struct StructHumanEquipSlots
    {
        public int HeadIndex;
        public int ChestIndex;
        public int HandIndex0;
        public int HandIndex1;
        public int FootIndex;

        public override string ToString()
        {
            return $"---- Player Equiments---\n" +
                $"HeadIndex : {HeadIndex}\n" +
                $"ChestIndex : {ChestIndex}\n" +
                $"HandIndex0: {HandIndex0}\n" +
                $"HandIndex1 : {HandIndex1}\n" +
                $"FootIndex : {FootIndex}";
        }

        public static StructHumanEquipSlots GetTempData()
        {
            StructHumanEquipSlots humanEquipSlots = new();
            humanEquipSlots.HeadIndex = -1;
            humanEquipSlots.ChestIndex = -1;
            humanEquipSlots.HandIndex0 = -1;
            humanEquipSlots.HandIndex1 = -1;
            humanEquipSlots.FootIndex = -1;
            return humanEquipSlots;
        }
    }

    /// <summary>
    /// Player Status, StructInventory
    /// </summary>
    [Serializable]
    public struct StructPlayerData
    {
        public int DataId;

        public StructStatus Status;

        public StructInventory Inventory;

        // 현재 착용 중인 장비 아이템 슬롯 번호 목록
        public StructHumanEquipSlots HumanEquipSlots;

        public float SpwanX;
        public float SpwanY;
        public float SpwanZ;

        public string SpawnPlaceId;

        public int[] ClearedQuestIds;

        public override string ToString()
        {

            return $"---------- {Status.Name} ----------\n" +
                $"Status : {Status}\n" +
                $"Inventory : {Inventory}\n" +
                $"{HumanEquipSlots}\n" +
                $"SpwanX {SpwanX}\n" +
                $"SpwanY {SpwanY}\n" +
                $"SpwanZ {SpwanZ}\n" +
                $"----------------------------";
        }

        public static StructPlayerData GetTempData()
        {
            StructPlayerData playerData = new();
            playerData.DataId = 1;
            playerData.Status = StructStatus.GetTempData();
            playerData.Inventory = StructInventory.GetTempData();
            playerData.HumanEquipSlots = StructHumanEquipSlots.GetTempData();
            playerData.SpwanX = 358.15f;
            playerData.SpwanY = 0.84f;
            playerData.SpwanZ = 101.78f;
            playerData.ClearedQuestIds = new int[] { };
            Debug.Log($"## GetTempData ClearedQuestIds : {string.Join(", ", playerData.ClearedQuestIds)}");
            return playerData;
        }
    }

    [Flags]
    public enum EnumWeaponType
    {
        None = 0,
        TwoHandedSword,
        OneHandedSword,
    }
}