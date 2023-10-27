using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Common
{
    public interface IStatus
    {
        public StructRealStatus RealStatus { get; }
        public StructStatus Status { get; }

        public static StructRealStatus UpdateRealStatus(StructStatus newStatus, int[] equipIds)
        {
            StructStatus copyStatus = newStatus;

            int totalStr = copyStatus.Str;
            int totalEnd = copyStatus.End;
            int totalSta = copyStatus.Sta;
            int totalMag = copyStatus.Mag;

            int equipAtk = 0;
            int equipDef = 0;
            int equipMaxHp = 0;
            int equipMaxMp = 0;

            for (int i = 0; i < equipIds.Length; i++)
            {
                int itemId = equipIds[i];

                if (itemId == -1)
                    continue;

                StructItemData equipData = DataBase.Items[itemId];

                totalStr += equipData.Str;
                totalEnd += equipData.End;
                totalSta += equipData.Sta;
                totalMag += equipData.Mag;

                equipAtk += equipData.Attack;
                equipDef += equipData.Defence;
                equipMaxHp += equipData.MaxHp;
                equipMaxMp += equipData.MaxMp;
            }

            int newAtk;
            int newDef;
            int newMaxHp;
            int newMaxMp;

            (newAtk, newDef, newMaxHp, newMaxMp) = Utils.Calculate.RealStatus(
                totalStr, totalEnd, totalSta, totalMag
            );

            newAtk += equipAtk;
            newDef += equipDef;
            newMaxHp += equipMaxHp;
            newMaxMp += equipMaxMp;

            return new StructRealStatus(
                totalStr, totalEnd, totalSta, totalMag,
                newAtk, newDef, newMaxHp, newMaxMp
                );
        }

        public static StructRealStatus UpdateRealStatus(StructStatus newStatus)
        {
            StructStatus copyStatus = newStatus;

            int totalStr = copyStatus.Str;
            int totalEnd = copyStatus.End;
            int totalSta = copyStatus.Sta;
            int totalMag = copyStatus.Mag;

            int newAtk;
            int newDef;
            int newMaxHp;
            int newMaxMp;

            (newAtk, newDef, newMaxHp, newMaxMp) = Utils.Calculate.RealStatus(
                totalStr, totalEnd, totalSta, totalMag
            );

            return new StructRealStatus(
                totalStr, totalEnd, totalSta, totalMag,
                newAtk, newDef, newMaxHp, newMaxMp
                );
        }
    }
}